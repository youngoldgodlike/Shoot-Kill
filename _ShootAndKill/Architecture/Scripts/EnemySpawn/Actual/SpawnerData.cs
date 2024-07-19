using System.Collections.Generic;
using System.Linq;
using Architecture.GameData;

namespace SpawnSystem
{
    public class SpawnerData
    {
        private readonly List<WaveData> _waves = new();

        public void AddWave(WaveData request) {
            //if(_waves.Count>0) _waves.Last().regularEnemies.cts.Cancel();
            _waves.Add(request);
        }

        public bool TryGetLastWave(out WaveData data) {
            if (_waves.Count < 1) {
                data = default;
                return false;
            }

            data = _waves.Last();
            return true;
        }
    }
}