using System;

namespace _Shoot_Kill.Architecture.Scripts.Data
{
    [Serializable]
    public class IntDataProperty : DataProperty<int>
    {
        public IntDataProperty(int value, int maxValue) : base(value, maxValue)
        {
        }
        public bool isMax => Value >= maxValue;
        public bool isMin => Value <= maxValue;
    }
}