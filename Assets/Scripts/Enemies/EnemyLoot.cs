
using UnityEngine;

public class EnemyLoot : MonoBehaviour {
    public string LootName;
    
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            
            var playerController = other.gameObject.GetComponent<PlayerController>();
            var weaponHandler = playerController.weaponHandler;

            switch (LootName) {
                case "Sword":
                    weaponHandler.ApplyUpgrade();
                    break;
            }
            
            Destroy(gameObject);
        }
    }
}
