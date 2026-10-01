using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    
    public GameObject MainMenuUI;
    public GameObject WeaponSelectionUI;
    public GameObject GameplayUI;
    public GameObject GameOverUI;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(this);
        }
    }
}
