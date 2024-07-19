using System;
using _Shoot_Kill.Architecture.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Menu<T> :  Singleton<T> where T : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    public Action onClose { get; protected set; }

    protected static readonly int IsClose = Animator.StringToHash("isClose");
    protected virtual void OnClose() => onClose?.Invoke();
}
