using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVSocket.Lib.Network.EzProto.Structs
{
    public struct EzLocationPacket {
        public static readonly EzFlag FIRST     = new(0x00, 0x00);
        public static readonly EzFlag POSITION  = new(0x01, 0x09);
        public static readonly EzFlag ROTATION  = new(0x02, 0x05);
        public static readonly EzFlag AREA      = new(0x03, 0x03);
        public static readonly EzFlag TERRITORY = new(0x04, 0x03);
        public static readonly EzFlag REGION    = new(0x05, 0x03);
        public static readonly EzFlag SUB_AREA  = new(0x06, 0x03);
        public static readonly EzFlag RESERVED1 = new(0x07, 0x00);
        public static readonly EzFlag RESERVED2 = new(0x08, 0x00);
        public static readonly EzFlag RESERVED3 = new(0x09, 0x00);
        public static readonly EzFlag RESERVED4 = new(0x0A, 0x00);
        public static readonly EzFlag RESERVED5 = new(0x0B, 0x00);
        public static readonly EzFlag RESERVED6 = new(0x0C, 0x00);
        public static readonly EzFlag RESERVED7 = new(0x0D, 0x00);
        public static readonly EzFlag RESERVED8 = new(0x0E, 0x00);
        public static readonly EzFlag LAST      = new(0x0F, 0x00);
    }

    public struct EzFlag {
        public byte flag  { get; }
        public sbyte lenB { get; }

        public EzFlag(byte flag, sbyte lenB) : this() {
            this.flag = flag;
            this.lenB = lenB;
        }
    }

    public enum EzResponseCode {
        OK                  = 0x00,
        MALFORMED           = 0x01,
        NOT_IMPLEMENTED     = 0x02,
        NOT_FOUND           = 0x03,
        INTERNAL_ERROR      = 0x04,
        NOT_AVAILABLE       = 0x05,
        TOO_MANY_REQUESTS   = 0x06
    }
}
