using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamage = 1f;
    
    [SerializeField] private GameObject LootPrefab;

    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private Rigidbody2D _rigidbody;
    private PlayerController _player;
    private Transform _playerTransform;
    private bool _alive;
    private bool attacking;
    private float _maxHealth;

    private void Awake() {
        _maxHealth = Mathf.Max(health, 0f);
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();

        if (_renderer == null || _collider == null || _rigidbody == null)
        {
            Debug.LogError("EnemyScript requires a SpriteRenderer, Collider2D, and Rigidbody2D.", this);
            enabled = false;
        }
        
        var circleCollider = (CircleCollider2D) _collider;
        if (circleCollider != null) {
            var localScale = transform.localScale;
            var avgLocalScale = (localScale.x + localScale.y) / 2f;
            attackDistance = circleCollider.radius * avgLocalScale + 0.6f; //Add player radius
        }
    }

    public float HealthFraction => _maxHealth > 0f ? Mathf.Clamp01(health / _maxHealth) : 0f;

    public void OnSpawn(PlayerController player) {
        _alive = true;
        _renderer.enabled = true;
        _collider.enabled = true;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;

        _player = player;
        _playerTransform = player.transform;

        if (_playerTransform == null)
        {
            Debug.LogWarning("EnemyScript spawned without a player target.", this);
            return;
        }

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void Update() {
        if (!_alive) return;
        
        if (health <= 0) {
            TriggerDeath();
            return;
        }
        
        if (_rigidbody == null || _playerTransform == null)
        {
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector2.zero;
            }

            return;
        }

        Vector2 toPlayer = (Vector2)_playerTransform.position - _rigidbody.position;
        if (toPlayer.sqrMagnitude <= attackDistance * attackDistance)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            AttackPlayer();
            return;
        }

        _rigidbody.linearVelocity = toPlayer.normalized * moveSpeed;
        _rigidbody.angularVelocity = 0f;
    }

    private void TriggerDeath() {
        _alive = false;
        _renderer.enabled = false;
        _collider.enabled = false;
        _rigidbody.bodyType = RigidbodyType2D.Static;
        
        //TODO PLAY DEATH SOUND
        
        Instantiate(LootPrefab, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }

    bool takingDamage;
    public void TakeDamage(float damage) {
        if (takingDamage) return;
        takingDamage = true;
        
        health -= damage;
        
        //TODO PLAY TAKE DAMAGE SOUND
        
        StartCoroutine(TakeDamageEffect());
    }

    IEnumerator TakeDamageEffect() {
        _renderer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        _renderer.color = Color.white;

        takingDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var damageAttribute = other.GetComponent<EnemyDamageAttribute>();
        if (damageAttribute == null) return;
        TakeDamage(damageAttribute.damage);
    }
    
    private void AttackPlayer() {
        if (attacking || _player == null) return;

        attacking = true;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence() {
        _player.TakeDamage(attackDamage);
        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
    }
}


