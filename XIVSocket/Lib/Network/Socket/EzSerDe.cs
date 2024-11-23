using System;
using System.Text;

namespace XIVSocket.Lib.Network.Socket
{
    internal struct EzDeserializedPacket
    {
        public uint id { get; set; }
        public uint flag { get; set; }
        public byte[] payload { get; set; }
    }

    internal static class EzSerDe
    {
        public static EzDeserializedPacket Deserialize(byte[] msg)
        {
            // These primitive types are represented by integers in C#
            int packetLength;
            int id;
            int flag;
            byte[] payload;

            // Get the first 16 bits
            {
                int shortValue = (msg[0] << 8) | msg[1];

                // Check if the first 6 bits match the fixed-length packet header
                if (((shortValue >> 10) & EzFlag.EZ) != EzFlag.EZ) {
                    throw new Exception("Malformed packet");
                }
            }

            {
                int shortValue = (msg[2] << 8) | msg[3];
                id = (shortValue >> 6) & 0x3FF;
                flag = shortValue & 0x3F;
                packetLength = shortValue & 0x3FF;
            }

            // Extract the payload
            payload = new byte[packetLength - 4];
            Array.Copy(msg, 4, payload, 0, packetLength - 4);

            Console.WriteLine("Received: " + id + ", " + flag + ", " + Encoding.UTF8.GetString(payload));

            return new EzDeserializedPacket
            {
                id = (uint)id,
                flag = (uint)flag,
                payload = payload
            };
        }

        public static byte[] Serialize(int routeFlag, byte[] data, int id = 0)
        {
            const int bHeader = 0x1E;
            int bId = id & 0x3FF;                                 // 10b 	- ID
            byte[] bPayload = Truncate1024B(data);                // 1024B 	- Payload truncated to 1024 bytes
            int bPacketLength = (4 + bPayload.Length) & 0x3FF;    // 10b 	- Packet length

            byte[] controlHeader = new byte[4];

            // Create the first 16-bit control header
            { 
                int s = ((bHeader << 10) | bPacketLength) & 0xFFFF;
                controlHeader[0] = (byte)((s >> 8) & 0xFF);
                controlHeader[1] = (byte)(s & 0xFF);
            }

            // Create the second 16-bit control header for ID and flag
            {
                int s = ((bId << 6) | routeFlag) & 0xFFFF;
                controlHeader[2] = (byte)((s >> 8) & 0xFF);
                controlHeader[3] = (byte)(s & 0xFF);
            }

            // Combine the control header and payload into a single array
            byte[] result = new byte[4 + bPayload.Length];
            Array.Copy(controlHeader, result, 4);
            Array.Copy(bPayload, 0, result, 4, bPayload.Length);

            return result;
        }

        public static byte[] Truncate1024B(byte[] data)
        {
            if (data.Length > 1024)
            {
                byte[] truncated = new byte[1024];
                Array.Copy(data, truncated, 1024);
                return truncated;
            }
            return data;
        }
    }
}
