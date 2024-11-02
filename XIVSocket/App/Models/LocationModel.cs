using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace XIVSocket.App.Models
{
    public class LocationModel
    {
        public uint RegionId { get; set; }
        public string RegionName { get; set; }

        public uint TerritoryId { get; set; }
        public string TerritoryName { get; set; }

        public uint AreaId { get; set; }
        public string AreaName { get; set; }

        public uint SubAreaId { get; set; }
        public string SubAreaName { get; set; }

        public uint HousingWardId { get; set; }
        public string HousingWardName { get; set; }

        public SerializableVector3 Position { get; set; }

        public LocationModel() {
            Init(0, "", 0, 0, 0, "", 0, "", 0, "", new SerializableVector3());
        }

        public LocationModel(uint regionId, string regionName, uint territoryId, uint territoryName, uint areaId, string areaName, uint subAreaId, string subAreaName, uint housingWardId, string housingWardName, SerializableVector3 position) {
            Init(regionId, regionName, territoryId, territoryName, areaId, areaName, subAreaId, subAreaName, housingWardId, housingWardName, position);
        }

        private void Init(uint regionId, string regionName, uint territoryId, uint territoryName, uint areaId, string areaName, uint subAreaId, string subAreaName, uint housingWardId, string housingWardName, SerializableVector3 position) { 
            RegionId = regionId;
            RegionName = regionName;
            TerritoryId = territoryId;
            AreaId = areaId;
            AreaName = areaName;
            SubAreaId = subAreaId;
            SubAreaName = subAreaName;
            HousingWardId = housingWardId;
            HousingWardName = housingWardName;
            Position = position;
        }

        // replace tostring method
        public override string ToString()
        {
            return string.Join("\n", new string[] {
                $"Region: {RegionName}",
                $"Territory: {TerritoryName}",
                $"Area: {AreaName}",
                $"Sub Area: {SubAreaName}",
                $"Housing Ward: {HousingWardName}",
                $"Position: {Position.ToString()}"
            });
        }
        //public override string ToString()
        //{
        //    return string.Join("\n", new string[] {
        //        $"Region: {RegionName} ({RegionId})",
        //        $"Territory: {TerritoryName} ({TerritoryId})",
        //        $"Area: {AreaName} ({AreaId})",
        //        $"Sub Area: {SubAreaName} ({SubAreaId})",
        //        $"Housing Ward: {HousingWardName} ({HousingWardId})",
        //        $"Position: {Position.ToString()}"
        //    });
        //}
    }
}
