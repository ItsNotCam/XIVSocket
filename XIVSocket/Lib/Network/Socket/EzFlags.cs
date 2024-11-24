namespace XIVSocket.Lib.Network.Socket
{
    public enum EzFlag
    {
        FIRST     = 0x01,
        HEARTBEAT = 0x02,
        EZ        = 0x1D,
        ECHO      = 0x3FF,

        INVALID_ROUTE     = 0x03,
        MALFORMED         = 0x04,
        NOT_IMPLEMENTED   = 0x05,
        NOT_FOUND         = 0x06,
        INTERNAL_ERROR    = 0x07,
        NOT_AVAILABLE     = 0x08,
        TOO_MANY_REQUESTS = 0x09,

        LOCATION_ALL       = 0x10,
        LOCATION_POSITION  = 0x11,
        LOCATION_ROTATION  = 0x12,
        LOCATION_AREA      = 0x13,
        LOCATION_TERRITORY = 0x14,
        LOCATION_REGION    = 0x15,
        LOCATION_SUB_AREA  = 0x16,

        JOB_ALL  = 0x20,
        JOB_MAIN = 0x21,
        JOB_CURRENT = 0x22,

        TIME = 0x30,
        NAME = 0x31,
    }
}
