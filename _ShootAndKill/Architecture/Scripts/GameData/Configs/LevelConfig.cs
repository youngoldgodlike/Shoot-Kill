using UnityEngine;

namespace Architecture.GameData.Configs
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
    public class LevelConfig : ScriptableObject
    {
        public WaveData[] waves = new WaveData[5];
    }
}

namespace Architecture.GameData
{
    public enum WaveType
    {
        Wave,Boss
    }
}