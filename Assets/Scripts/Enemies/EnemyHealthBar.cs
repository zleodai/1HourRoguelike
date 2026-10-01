using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private EnemyScript _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<EnemyScript>();

        if (_enemy == null || fillImage == null)
        {
            Debug.LogError("EnemyHealthBar needs a parent EnemyScript and an assigned fill Image.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        fillImage.fillAmount = _enemy.HealthFraction;
    }
}
