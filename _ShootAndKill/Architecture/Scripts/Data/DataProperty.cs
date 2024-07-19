using System;
using R3;
using UnityEngine;

[Serializable]
public abstract class DataProperty<T> : ReactiveProperty<T>
{
    protected DataProperty(T value, T maxValue)
    {
        Value = value;
        this.maxValue = maxValue;
    }
    
    [field: SerializeField] public T maxValue { get; protected set;}
    
}
