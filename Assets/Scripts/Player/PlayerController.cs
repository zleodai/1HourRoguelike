using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private InputService InputService;
    
    private SpriteRenderer renderer;
    private Collider2D collider;
    private Rigidbody2D rigidbody;

    private float maxHealth;
    private Vector2 mousePos;

    [HideInInspector] public PlayerWeaponHandler weaponHandler;
    
    [Header("Player Stats")] 
    public float health = 100f;
    public float moveSpeed = 1f;
    public float weaponDamage = 10f;
    
    [HideInInspector] public Vector2 moveDirection;
    [HideInInspector] public Vector2 lookDirection;
    [HideInInspector] public bool alive;
    
    public float HealthFraction => maxHealth > 0f ? Mathf.Clamp01(health / maxHealth) : 0f;

    private void Awake() {
        InputService = ServiceManager.Instance.GetService<InputService>();
        
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
        
        collider = GetComponent<Collider2D>();
        collider.enabled = false;
        
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;

        maxHealth = health;
        alive = false;
        _gameOverActive = false;
    }

    public void Spawn() {
        weaponHandler.OnSpawn();
        
        renderer.enabled = true;
        collider.enabled = true;
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        
        alive = true;
        
        InputService.Inputs.Player.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        InputService.Inputs.Player.Move.canceled += _ => moveDirection = Vector2.zero;

        InputService.Inputs.Player.Interact.performed += OnClick;
        InputService.Inputs.Player.MousePos.performed += OnLook;

        InputService.Inputs.Player.Dash.performed += _ => OnDash();
        
        defaultBarDashColor = dashFillImage.color;
    }

    private bool takingDamage;
    public void TakeDamage(float damage) {
        if (takingDamage) return;
        
        takingDamage = true;
        
        //TODO PLAY TAKE DAMAGE SOUND
        
        health -= damage;
        if (health <= 0f) {
            alive = false;
            OnDeath();
        }

        StartCoroutine(TakeDamageEffect());
    }

    IEnumerator TakeDamageEffect() {
        renderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        renderer.color = Color.white;

        takingDamage = false;
    }

    private bool _gameOverActive;
    void OnDeath() {
        //TODO PLAY DEATH SOUND OR GAME OVER SOUND
        
        UIManager.Instance.GameOverUI.SetActive(true);
        _gameOverActive = true;
    }

    private Color defaultBarDashColor;
    void Update() {
        if (!alive) return;

        var dashOngoing = dashActive;
        var dashCooldownProgress = 1 - (nextDashAvailableTime - Time.time)/dashCooldown;

        if (dashOngoing) {
            dashFillImage.color = Color.rebeccaPurple;
        }
        else {
            dashFillImage.fillAmount = Mathf.Clamp(dashCooldownProgress, 0, 1);
        }
        

        rigidbody.angularVelocity = 0f;
        
        if (dashActive) return;
        
        rigidbody.linearVelocity = moveDirection * moveSpeed;
    }

    void OnClick(InputAction.CallbackContext _) {
        if (_gameOverActive) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (weaponHandler != null) weaponHandler.UseWeaponAttack();
    }

    void OnLook(InputAction.CallbackContext ctx) {
        mousePos = ctx.ReadValue<Vector2>();
        lookDirection = (mousePos - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)).normalized;
    }

    public float dashCooldown = 3f;
    [HideInInspector] public float nextDashAvailableTime;
    [HideInInspector] public bool dashActive;
    void OnDash() {
        if (dashActive) return;
        
        if (Time.time > nextDashAvailableTime) {
            dashActive = true;
            Debug.Log("Dashing");
            StartCoroutine(DashSequence());
        }
    }

    public float dashDistance = 3f;
    public float dashDuration = 0.2f;

    [SerializeField] private Image dashFillImage;
    
    IEnumerator DashSequence() {
        var start = transform.position;
        var destination = start + (new Vector3(lookDirection.x, lookDirection.y, 0) * dashDistance);
        
        var startTime = Time.time;
        var progress = (Time.time - startTime) / dashDuration;
        
        while (progress < 1f) {
            progress = (Time.time - startTime) / dashDuration;
            var pos = Vector3.Lerp(start, destination, progress);
            transform.position = pos;
            rigidbody.position = pos;
            
            yield return new WaitForEndOfFrame();
        }
        transform.position = destination;
        rigidbody.position = destination;
        
        Debug.Log("Stop Dashing");
        dashActive = false;
        nextDashAvailableTime = Time.time + dashCooldown;
        dashFillImage.color = defaultBarDashColor;
        dashFillImage.fillAmount = 0f;
    }
}
