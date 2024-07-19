using System;
using _Shoot_Kill.Architecture.Scripts.Data;
using UnityEngine;

namespace Assets.Architecture.Scripts.Utils
{
    [Serializable]
    public class Cooldown
    {
        public Cooldown(float value,float maxValue)
        {
            delay = new FloatDataProperty(value, maxValue);
        }
        
        public FloatDataProperty delay;
        
       [SerializeField] private float _timesUp = 0;
        
        public bool isReady => _timesUp <= Time.time;

        public void Reset() => _timesUp = Time.time + delay.Value;

        public void ResetTimesUp() => _timesUp = 0;


    }
}
