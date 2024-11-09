using Lumina.Excel;

namespace XIVSocket.App
{
    public static class Utility
    {
        public static T? GetRowOfType<T>(uint id) where T : ExcelRow
        {
            // Attempt to get the Excel sheet for the given type
            var sheet = Plugin.DataManager.GetExcelSheet<T>();

            // Ensure the sheet is not null, then try to get the row by ID
            return sheet?.GetRow(id);
        }

        public static ushort ToUint16BE(byte[] buffer, uint offset) {
            return (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
        }

    }
}
