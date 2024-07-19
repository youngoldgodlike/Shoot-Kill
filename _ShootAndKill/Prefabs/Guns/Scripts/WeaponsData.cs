using System;
using System.Collections.Generic;
using Assets.Prefabs.Guns.Scripts;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponsData", menuName = "Weapons/Data")]
public class WeaponsData : ScriptableObject
{
    [field: SerializeField] public List<WeaponData> weapons { get; private set; }

    public WeaponData GetWeapon(EWeaponID id)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.id == id)
                return weapon;
        }
        return null;
    }
}

[Serializable]
public class WeaponData
{
    [field: SerializeField] public EWeaponID id { get; private set; }
    [field: SerializeField] public ProjectileAttack gun { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController animator { get; private set; }
    [field: SerializeField] public GameObject modelForSelectionMenu { get; private set; }
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public bool isLock { get; private set; }
}

public enum EWeaponID
{
    MiniGun,
    Pistol
}
