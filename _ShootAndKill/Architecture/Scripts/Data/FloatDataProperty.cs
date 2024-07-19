using System;

namespace _Shoot_Kill.Architecture.Scripts.Data
{
    [Serializable]
    public class FloatDataProperty : DataProperty<float>
    {
        public FloatDataProperty(float value, float maxValue) : base(value, maxValue)
        {
        }

        public bool isMax => Value >= maxValue;
        public bool isMin => Value <= maxValue;
    }
}