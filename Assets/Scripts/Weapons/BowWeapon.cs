using System.Collections;
using UnityEngine;

public class BowWeapon : PlayerWeaponHandler {
    private const float DefaultWeaponDistance = 0.5f;
    
    private PlayerController _player;
    private float _weaponDist;
    
    [SerializeField] private SpriteRenderer renderer;
    private Collider2D collider;
    
    [SerializeField] private GameObject arrowPrefab;
    
    [Header("Stats")]
    [SerializeField] private float arrowDamage = 25f;
    [SerializeField] private float arrowSpeed = 25f;
    [SerializeField] private float arrowDuration = 3f;
    [SerializeField] private float usageCooldown = 2.5f;
    
    void Start() {
        collider = GetComponent<Collider2D>();
        
        _player = FindAnyObjectByType<PlayerController>();
        _weaponDist = DefaultWeaponDistance;
        
        renderer.enabled = false;
        collider.enabled = false;
    }

    public override void OnSpawn() {
        renderer.enabled = true;
        collider.enabled = true;
    }
    
    void Update() {
        UpdateWeaponPos(_weaponDist);
    }

    void UpdateWeaponPos(float lookDist) {
        var lookDir = _player.lookDirection;
        
        var weaponOffset = lookDir * lookDist;
        transform.localPosition = new Vector3(weaponOffset.x, weaponOffset.y, 0);
        
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private bool canAttack = true;
    public override void UseWeaponAttack() {
        if (!canAttack) return;
        canAttack = false;
        
        var arrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        var arrowDamageAttribute = arrow.AddComponent<EnemyDamageAttribute>();
        arrowDamageAttribute.damage = arrowDamage;
        var arrowRB = arrow.GetComponent<Rigidbody2D>();
        arrowRB.linearVelocity = _player.lookDirection * arrowSpeed;
        Destroy(arrow, arrowDuration);

        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(usageCooldown);
        canAttack = true;
    }

    public override void ApplyUpgrade() {
        arrowDamage *= 1.1f;
    }
}
