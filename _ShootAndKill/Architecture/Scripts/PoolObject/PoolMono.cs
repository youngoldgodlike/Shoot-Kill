using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolMono<T>  where T : MonoBehaviour
{
    public T prefab { get; }
    public bool autoExpand { get; set; }
    public Transform container { get; }

    private List<T> _pool;

    public PoolMono(T prefab, int capacity)
    {
        this.prefab = prefab;
        this.container = null;
        
        CreatePool(capacity);
    }

    public PoolMono(T prefab, int capacity, Transform container)
    {
        this.prefab = prefab;
        this.container = container;
        
        CreatePool(capacity);
    }

    private void CreatePool(int capacity)
    {
        _pool = new List<T>();

        for (int i = 0; i < capacity; i++)
            CreateObject();
    }

    private T CreateObject(bool isActiveByDefault = false)
    {
        var createdObject = Object.Instantiate(prefab, container);
        createdObject.gameObject.SetActive(isActiveByDefault);
        _pool.Add(createdObject);

        return createdObject;
    }

    public T TryGetFreeElement(bool isActive)
    {
        if (HasFreeElement(out var element, isActive))
            return element;

        if (autoExpand)
          return CreateObject(true);
        
        throw  new Exception($"There is no free element in poo; of type {typeof(T)}");
    }

    public bool HasFreeElement(out T element, bool isActive)
    {
        foreach (var mono in _pool)
        {
            if (!mono.gameObject.activeInHierarchy)
            {
                element = mono;
                mono.gameObject.SetActive(isActive);
                return true;
            }
        }
        
        element = null;
        return false;
    }
}
