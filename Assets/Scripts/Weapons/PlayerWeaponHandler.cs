using System.Collections;
using UnityEngine;

public abstract class PlayerWeaponHandler : MonoBehaviour {
    public abstract void OnSpawn();
    public abstract void UseWeaponAttack();
    public abstract void ApplyUpgrade();
}
