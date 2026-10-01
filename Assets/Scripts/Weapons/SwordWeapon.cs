using System.Collections;
using UnityEngine;

public class SwordWeapon : PlayerWeaponHandler {
    private const float DefaultWeaponDistance = 0.5f;
    
    private PlayerController _player;
    private float _weaponDist;
    
    [SerializeField] private SpriteRenderer renderer;
    private Collider2D collider;
    
    [Header("Stats")]
    [SerializeField] private float swingDamage = 25f;
    
    private EnemyDamageAttribute damageAttribute;
    
    void Start() {
        damageAttribute = gameObject.AddComponent<EnemyDamageAttribute>();
        damageAttribute.damage = swingDamage;
        
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

    private bool _swingingSword;

    public override void UseWeaponAttack() {
        if (_swingingSword) return;
        
        _swingingSword = true;
        
        StartCoroutine(SwingSwordSequence());
    }

    IEnumerator SwingSwordSequence() {
        float baseSwingDist = _weaponDist;
        float maxSwingDist = 1f;
        float swingSpeed = 10f;
        
        //TODO ADD SOUND EFFECT :)
        
        while (_weaponDist < maxSwingDist) {
            _weaponDist += Time.deltaTime * swingSpeed;
            yield return new WaitForEndOfFrame();
        }
        
        _weaponDist = baseSwingDist;

        _swingingSword = false;
    }

    public override void ApplyUpgrade() {
        var currentScale = transform.localScale;
        transform.localScale = currentScale * 1.01f;
    }
}
