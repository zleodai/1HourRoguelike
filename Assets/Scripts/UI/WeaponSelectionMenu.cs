using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionMenu : MonoBehaviour {
    public WeaponSelectButton bowSelectionButton;
    public WeaponSelectButton scytheSelectionButton;
    public GameObject continueButton;
    public PlayerWeaponAssigner playerWeaponAssigner;

    private List<WeaponSelectButton> weaponSelectButtons;

    private WeaponSelectButton selectedButton;

    void Awake() {
        continueButton.SetActive(false);
        
        weaponSelectButtons = new List<WeaponSelectButton> {bowSelectionButton, scytheSelectionButton};
        
        bowSelectionButton.GetOnClick().AddListener(() => { WeaponButtonPressed(bowSelectionButton); });
        scytheSelectionButton.GetOnClick().AddListener(() => { WeaponButtonPressed(scytheSelectionButton); });
    }

    void WeaponButtonPressed(WeaponSelectButton button) {
        if (selectedButton == button) {
            button.selected = false;
            selectedButton = null;
            continueButton.SetActive(false);
            return;
        }

        if (selectedButton != null) {
            selectedButton.selected = false;
            selectedButton = null;
        }

        selectedButton = button;
        selectedButton.selected = true;
        continueButton.SetActive(true);
    }

    public void ApplyWeaponSelected() {
        playerWeaponAssigner.AssignPlayerWeapon(selectedButton.weaponName);
    }
}
