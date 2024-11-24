using System;
using System.Text;

namespace XIVSocket.Lib.Network.Codec
{
    public static class EzCodec
    {
        public static string Decode(byte[] buffer)
        {
            var bitUnpacker = new EzDecoder(6, buffer);
            var decodedString = new StringBuilder();

            // Decode each 6-bit chunk from the buffer
            while (bitUnpacker.BufferIndex < buffer.Length)
            {
                var encodedValue = bitUnpacker.Unpack();
                decodedString.Append(EzEncoding.Characters[encodedValue]);
            }
            return decodedString.ToString();
        }

        public static byte[] Encode(string data)
        {
            var length = data.Length;
            var bufferSize = (int)Math.Ceiling(length * 6 / 8.0);
            var bitPacker = new EzEncoder(6, bufferSize);

            for (var i = 0; i < length; i++)
            {
                var charIndex = Math.Max(
                    0x00, Array.IndexOf(EzEncoding.Characters, data[i].ToString()
                ));
                bitPacker.Pack(charIndex & 0x3F);
            }

            return bitPacker.FinalizeBuffer();
        }
    }

}
