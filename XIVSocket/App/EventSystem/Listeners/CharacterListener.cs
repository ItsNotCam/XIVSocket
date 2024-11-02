using Dalamud.Game.Network.Structures.InfoProxy;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XIVSocket.App.EventSystem.Listeners
{
    internal class CharacterListener : IDisposable
    {
        private Plugin Plugin;
        private System.Threading.Tasks.Task TickTask;
        private CancellationTokenSource CancellationTokenSource;
        private Dictionary<uint, ClassJob> JobsById;

        private int lastExp = 0;
        private int lastJob = 0;
        private int lastLevel = 0;
        private Vector3 lastPos = Vector3.Zero;

        public CharacterListener(Plugin plugin)
        {
            Plugin = plugin;
            CancellationTokenSource = new CancellationTokenSource();

            JobsById = new Dictionary<uint, ClassJob>();
            Plugin.DataManager.GetExcelSheet<ClassJob>()!.ToList().ForEach(job =>
            {
                JobsById.Add(job.RowId, job);
            });
        }

        public void Start()
        {
            Plugin.Framework.Update += tick;
        }

        public async void Stop()
        {
            Plugin.Framework.Update -= tick;
        }

        public void Dispose()
        {
            Stop();
        }

        private ClassJob GetJobNameById(uint jobId)
        {
            JobsById.TryGetValue(jobId, out var job);
            return job;
        }

        private ClassJob GetJobNameById(int jobId)
        {
            return GetJobNameById((uint)jobId);
        }

        private void tick(IFramework framework)
        {
            var currentExperience = GetCurrentExperience();
            var currentJob = GetCurrentJob();
            var currentLevel = GetCurrentLevel();
            var job = GetJobNameById(lastJob);

            if (lastJob != currentJob.RowId)
            {
                Plugin.NetworkManager.SendMessage($"Job change: Lvl: {lastLevel} {job!.Name} -> Lvl. {currentLevel} {currentJob.Name}");
                lastJob = (int)currentJob.RowId;
                lastExp = currentExperience;
                lastLevel = currentLevel;
                return;
            }

            /* Level */
            if (lastLevel != currentLevel)
            {
                var delta = currentLevel - lastLevel;
                var dir = delta > 0 ? "+" : "";
                if (delta > 0)
                {
                    Plugin.NetworkManager.SendMessage($"Lvl up! Lvl. {lastLevel} -> {currentLevel} {job.Name}");
                }

                Plugin.NetworkManager.SendMessage($"Lvl {currentLevel} {currentJob.Name} | Exp {dir}{delta} ({currentExperience})");
                lastLevel = currentLevel;
            }

            /* Experience */

            if (currentExperience != lastExp)
            {
                var delta = currentExperience - lastExp;
                var dir = delta > 0 ? "+" : "";
                Plugin.NetworkManager.SendMessage($"Lvl {currentLevel} {currentJob.Name} | Exp {dir}{delta} ({currentExperience})");
            }
            lastExp = currentExperience;

            var pos = GetCurrentPosition();
            if (!pos.Equals(lastPos))
            {
                Plugin.NetworkManager.SendMessage($"Moved! {lastPos.ToString()} -> {pos.ToString()}");
                lastPos = pos;
            }
        }

        public Vector3 GetCurrentPosition()
        {
            var pos = Plugin.ClientState.LocalPlayer.Position;
            return pos;
        }

        public Vector3 GetClosestAetherite()
        {
            return Vector3.Zero;
        }

        public unsafe int GetCurrentLevel()
        {
            var playerState = PlayerState.Instance();
            if (playerState->IsLoaded == 0)
                return -1;

            return playerState->CurrentLevel;
        }

        public unsafe ClassJob GetCurrentJob()
        {
            var playerState = PlayerState.Instance();
            if (playerState->IsLoaded == 0)
                return null;


            int classJobId = playerState->CurrentClassJobId;
            JobsById.TryGetValue((uint)classJobId, out var job);
            if (job != null)
            {
                return job;
            }

            return null;
        }


        public unsafe int GetCurrentExperience()
        {
            var playerState = PlayerState.Instance();
            if (playerState->IsLoaded == 0)
                return 0;

            var classJobId = playerState->CurrentClassJobId;
            var classJobRow = Plugin.DataManager.GetExcelSheet<ClassJob>()!.GetRow(classJobId);
            var currentExperience = playerState->ClassJobExperience[classJobRow.ExpArrayIndex];

            return currentExperience;
        }
    }
}
