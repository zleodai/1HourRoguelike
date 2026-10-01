using System.Collections.Generic;
using UnityEngine;
public class PlayerWeaponAssigner : MonoBehaviour {
    [SerializeField] private Dictionary<string, GameObject> weaponPrefabs;
    
    private PlayerController _player;

    void Awake() {
        _player = FindAnyObjectByType<PlayerController>();
    }
    
    public void AssignPlayerWeapon(string weaponName) {
        var weaponObject = Instantiate(weaponPrefabs[weaponName], _player.transform);
        var weaponScript = weaponObject.GetComponent<PlayerWeaponHandler>();
        _player.weaponHandler = weaponScript;
    }
}