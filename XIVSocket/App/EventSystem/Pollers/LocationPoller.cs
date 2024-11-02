using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using XIVSocket.App.EventSystem.Events;
using XIVSocket.App.Logging;
using XIVSocket.App.Models;

using TerritoryType = Lumina.Excel.GeneratedSheets.TerritoryType;

using static XIVSocket.App.Utility;

namespace XIVSocket.App.EventSystem.Pollers
{
    public class LocationPoller : IPollerBase
    {
        public LocationModel Location { get; }

        private EventManager EventManager;
        private Logger logger;

        private bool hasInitialized;

        public LocationPoller(EventManager manager) {
            Location = new LocationModel();

            EventManager = manager;
            logger = manager.Plugin.Logger;
        }

        private unsafe static TerritoryInfo* AreaInfo => TerritoryInfo.Instance();
        private unsafe static HousingManager* HousingInfo => HousingManager.Instance();

        private static PlaceName? GetPlaceName(uint row) => GetRowOfType<PlaceName>(row);

        public void poll(IFramework framework)
        {
            // get client state
            var clientState = Plugin.ClientState;

            if(clientState.LocalPlayer is null) {
                return;
            }

            // get current player position
            var curPos = clientState.LocalPlayer.Position;

            // get current area
            uint curAreaId = Location.AreaId;
            unsafe { curAreaId = AreaInfo->AreaPlaceNameId; }
            string curAreaName = GetRowOfType<PlaceName>(curAreaId)?.Name ?? "unknown";

            // get current territory
            uint curTerritoryId = clientState.TerritoryType;
            var curTerritory = GetRowOfType<TerritoryType>(curTerritoryId);
            string curTerritoryName = curTerritory?.PlaceName.Value?.Name.ToString() ?? "unknown";

            // get current region
            uint curRegionId = curTerritory.PlaceNameRegion.Value.RowId;
            string curRegionName = GetRowOfType<PlaceName>(curRegionId)?.Name ?? "unknown";

            // sub area
            uint curSubAreaId = Location.SubAreaId;
            unsafe { curSubAreaId = AreaInfo->SubAreaPlaceNameId; }
            var curSubAreaName = GetRowOfType<PlaceName>(curSubAreaId)?.Name ?? "unknown";

            if (!Location.Position.Equals(curPos)) {
                EventManager.Dispatch(new PlayerMoveEvent(Location.Position.ToVector3(), curPos));
                Location.Position = new SerializableVector3(curPos);
            }

            if(Location.RegionId != curRegionId)
            {
                EventManager.Dispatch(new PlayerChangeRegionEvent(Location.RegionName, curRegionName));
                Location.RegionId = curRegionId;
                Location.RegionName = curRegionName;
            }

            if (Location.AreaId != curAreaId)
            {
                EventManager.Dispatch(new PlayerChangeAreaEvent(Location.AreaName, curAreaName));
                Location.AreaId = curAreaId;
                Location.AreaName = curAreaName;
            }

            if (Location.TerritoryId != curTerritoryId) {
                EventManager.Dispatch(new PlayerChangeTerritoryEvent(Location.TerritoryName, curTerritoryName));
                Location.TerritoryId = curTerritoryId;
                Location.TerritoryName = curTerritoryName;
            }

            if(Location.SubAreaId != curSubAreaId) {
                EventManager.Dispatch(new PlayerChangeSubAreaEvent(Location.SubAreaName, curSubAreaName));
                Location.SubAreaId = curSubAreaId;
                Location.SubAreaName = curSubAreaName;
            }
        }
    }
}
