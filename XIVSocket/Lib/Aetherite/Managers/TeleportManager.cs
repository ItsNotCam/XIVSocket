using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace XIVSocket.Lib.Aetherite.Managers
{
    public static unsafe class TeleportManager
    {
        public static bool Teleport(TeleportInfo info)
        {
            if (Plugin.ClientState.LocalPlayer == null)
                return false;
            var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, 5);
            if (status != 0)
            {
                var msg = GetLogMessage(status);
                if (!string.IsNullOrEmpty(msg))
                    Plugin.PluginLogger.Debug(msg, true);
                return false;
            }

            if (Plugin.ClientState.LocalPlayer.CurrentWorld.RowId != Plugin.ClientState.LocalPlayer.HomeWorld.RowId)
            {
                if (AetheryteManager.IsHousingAetheryte(info.AetheryteId, info.Plot, info.Ward, info.SubIndex))
                {
                    Plugin.PluginLogger.Debug($"Unable to Teleport to {AetheryteManager.GetAetheryteName(info)} while visiting other Worlds.", true);
                    return false;
                }
            }

            return Telepo.Instance()->Teleport(info.AetheryteId, info.SubIndex);
        }

        public static bool Teleport(TeleportAlias alias)
        {
            if (Plugin.ClientState.LocalPlayer == null)
                return false;
            var status = ActionManager.Instance()->GetActionStatus(ActionType.Action, 5);
            if (status != 0)
            {
                var msg = GetLogMessage(status);
                if (!string.IsNullOrEmpty(msg))
                    Plugin.PluginLogger.Debug(msg);
                return false;
            }

            if (Plugin.ClientState.LocalPlayer.CurrentWorld.RowId != Plugin.ClientState.LocalPlayer.HomeWorld.RowId)
            {
                if (AetheryteManager.IsHousingAetheryte(alias.AetheryteId, alias.Plot, alias.Ward, alias.SubIndex))
                {
                    Plugin.PluginLogger.Debug($"Unable to Teleport to {alias.Aetheryte} while visiting other Worlds.", true);
                    return false;
                }
            }
            return Telepo.Instance()->Teleport(alias.AetheryteId, alias.SubIndex);
        }

        private static string GetLogMessage(uint id)
        {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.LogMessage>();
            if (sheet == null) return string.Empty;
            return sheet.GetRow(id).Text.ExtractText();
        }
    }
}
