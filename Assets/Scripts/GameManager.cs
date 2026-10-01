using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    
    private SubtitleService SubtitleService;
    private EnemySpawnService EnemySpawnService;
    
    private UIManager UIManager => UIManager.Instance;
    public PlayerController PlayerController;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(this);
        }
        
        SubtitleService = ServiceManager.Instance.GetService<SubtitleService>();
        EnemySpawnService = ServiceManager.Instance.GetService<EnemySpawnService>();
    }

    public void StartGame() {
        UIManager.MainMenuUI.SetActive(false);
        StartCoroutine(StartNewGameSequence());
    }

    IEnumerator StartNewGameSequence() {
        var line1 = SubtitleService.SubtitleArgs.Voice1("Welcome to the dungeons!!!");
        yield return SubtitleService.CreateAndPlaySubtitle(line1);
        
        PlayerController.Spawn();
        UIManager.GameplayUI.SetActive(true);
        gameRunning = true;
        
        EnemySpawnService.SpawnRandomEnemyAroundPlayer(4, 10, PlayerController);
    }

    [SerializeField] private float spawnEnemyInterval = 3f;
    private float lastSpawnedEnemyTime;
    private bool gameRunning;

    [SerializeField] private float spawnIncreaseInterval = 60f;
    private float lastSpawnIncreaseTime;
    
    void Update() {
        if (!gameRunning) return;
        Debug.Log("Game running");
        
        if (Time.time > lastSpawnedEnemyTime) {
            lastSpawnedEnemyTime = Time.time + spawnEnemyInterval;
            EnemySpawnService.SpawnRandomEnemyAroundPlayer(4, 10, PlayerController);
        }

        if (Time.time > lastSpawnIncreaseTime) {
            lastSpawnIncreaseTime = Time.time + spawnIncreaseInterval;
            spawnEnemyInterval -= 0.5f;
        }
    }
}