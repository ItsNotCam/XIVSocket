using Lumina.Excel;
using System;
using XIVEvents;

namespace XIVSocket.App
{
    public static class Utility
    {
        public static T? GetRowOfType<T>(uint id) where T : struct, IExcelRow<T>
        {
            // Attempt to get the Excel sheet for the given type
            var sheet = GetExcelSheetSafe<T>();

            // Ensure the sheet is not null, then try to get the row by ID
            return sheet?.GetRow(id);
        }

        private static ExcelSheet<T>? GetExcelSheetSafe<T>() where T : struct, Lumina.Excel.IExcelRow<T>
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(7))
            {
                return Services.DataManager.GetExcelSheet<T>();
            }
            return null;
        }

        public static ushort ToUint16BE(byte[] buffer, uint offset) {
            return (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
        }

    }
}
