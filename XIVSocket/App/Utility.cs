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
    }
}
