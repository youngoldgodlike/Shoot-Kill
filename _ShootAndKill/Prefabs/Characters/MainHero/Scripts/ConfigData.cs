using System;
using UnityEngine;

namespace Assets.Prefabs.Characters.MainHero.Scripts
{
    [Serializable]
    public class ConfigData <T>
    {
        [field: SerializeField] public T Value { get; private set; }
        [field: SerializeField] public T maxValue { get; private set; }
    }
}