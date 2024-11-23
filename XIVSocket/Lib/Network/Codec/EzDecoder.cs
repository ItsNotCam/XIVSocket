namespace XIVSocket.Lib.Network.Codec
{
    internal class EzDecoder
    {
        private int word = 0;
        private int bitsUsed = 0;
        private int bufferIndex = 0;
        private byte[] buffer;
        private int bitSize;

        public int BufferIndex => bufferIndex;

        public EzDecoder(int bitSize, byte[] buffer)
        {
            this.bitSize = bitSize;
            this.buffer = buffer;
        }

        public int Unpack()
        {
            if (bitsUsed < bitSize)
            {
                word = word << 8 | buffer[bufferIndex++] & 0xFF;
                bitsUsed += 8;
            }

            var result = word >> bitsUsed - bitSize & (1 << bitSize) - 1;
            bitsUsed -= bitSize;
            return result;
        }
    }

}
