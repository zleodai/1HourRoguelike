
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private PlayerController _playerController;

    private void Awake()
    {
        _playerController = FindObjectsByType<PlayerController>()[0];

        if (_playerController == null || fillImage == null)
        {
            Debug.LogError("EnemyHealthBar needs a parent EnemyScript and an assigned fill Image.", this);
            enabled = false;
        }
    }

    private void Update()
    
    {
        fillImage.fillAmount = _playerController.HealthFraction;
    }
}

































