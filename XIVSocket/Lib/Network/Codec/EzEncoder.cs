namespace XIVSocket.Lib.Network.Codec
{
    internal static class EzEncoding
    {
        public static readonly string[] Characters = new string[]
        {
        "", " ", "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".",
        "/", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", "<", "=", ">", "?", "@",
        "[", "]", "{", "}", "_", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
        "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
        };
    }

    internal class EzEncoder
    {
        private int word = 0;
        private int bitsUsed = 0;
        private byte[] buffer;
        private int bufferIndex = 0;
        private int bitSize;

        public EzEncoder(int bitSize, int bufferSize)
        {
            this.bitSize = bitSize;
            buffer = new byte[bufferSize];
        }

        public void Pack(int value)
        {
            word = word << bitSize | value & (1 << bitSize) - 1;
            bitsUsed += bitSize;

            while (bitsUsed >= 8)
            {
                buffer[bufferIndex++] = (byte)(word >> bitsUsed - 8 & 0xFF);
                bitsUsed -= 8;
            }
        }

        public byte[] FinalizeBuffer()
        {
            if (bitsUsed != 0)
            {
                buffer[bufferIndex] = (byte)(word << 8 - bitsUsed & 0xFF);
            }
            return buffer;
        }
    }
}
