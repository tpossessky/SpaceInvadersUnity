using System;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SpaceInvaders : MonoBehaviour {
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private ShieldFactory shieldFactory;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PlayerController playerController;
    
    [SerializeField] private float horizontalSpeed = 0.2f;
    private float curSpeed;
    [SerializeField] private float speedIncrement = 0.05f;
    [SerializeField] private float shiftDownAmount = 0.1f;
    [SerializeField] private float leftBoundary = -2.5f;
    [SerializeField] private float rightBoundary = 2.5f;

    public UnityEvent _onPause;
    public UnityEvent _onResume;
    
    private GameObject[,] enemies;
    
    private float minX;
    private float maxX;
    private float minEnemyY = -1.12f;
    private int direction = 1;
    private bool hasShiftedDown;

    private int score;
    private int lives = 3;
    private bool isRunning = true;
    private bool isPaused;
    private bool isPlayerDeathTimerRunning;

    private float playerDeathTimer = 1f;
    private float curPlayerDeathTimer;
    private HighScore lastHighScore = new(0, "AAA");
    
    private ScoreHandler scoreHandler;
    private HighScore[] curHighScores;
    private void Start() {
        scoreHandler = new ScoreHandler();
        InitHighScores();
        isRunning = false;
        uiManager.showStartGame();
        shieldFactory.EnemyShieldCollision += EnemyShieldCollision;
        curSpeed = horizontalSpeed;
        uiManager.onPlayerEnteredHighScore += SavePlayerHighScore;
        uiManager.HideHighScoreDialog();
    }

    private void InitHighScores() {
        curHighScores = scoreHandler.GetScores();
        if (curHighScores != null) {
            uiManager.SetHighScores(curHighScores);
        }
    }

    private void InitGame() {
        SpawnEnemies();
        SpawnShields();
        UpdateBoundaries();
        ResumeGame();
        playerController.PlayerRevive();
        uiManager.newGame();
        uiManager.HideHighScoreDialog();
        score = 0;
        isRunning = true;
    }

    private void LoadNextLevel() {
        curSpeed = horizontalSpeed + speedIncrement; 
        SpawnEnemies();
        UpdateBoundaries();
        ResumeGame();
    }

    private void SpawnEnemies() {
        enemyFactory.createEnemyGrid();
        enemies = enemyFactory.enemyGrid;
    }

    private void SpawnShields() {
        shieldFactory.SpawnShields();
    }

    private void Update() {
        if (!isRunning) {
            if (!isPaused) {
                _onPause?.Invoke();            
            }
            return;
        }

        if (getMinY() < -1.12) {
            gameOver();
        }
        
        if (CheckWin()) {
            PauseGame();
            LoadNextLevel();
        }

        if (isPlayerDeathTimerRunning && curPlayerDeathTimer < playerDeathTimer) {
            curPlayerDeathTimer += Time.deltaTime;
            return;
        }
        
        if (isPlayerDeathTimerRunning && curPlayerDeathTimer > playerDeathTimer) {
            isPlayerDeathTimerRunning = false;
            curPlayerDeathTimer = 0f; 
            playerController.PlayerRevive();
        }
        
        HandleEnemyMovement();

    }

    private void HandleEnemyMovement() {
        UpdateBoundaries();
        if (minX <= leftBoundary || maxX >= rightBoundary) {
            if (!hasShiftedDown) {
                MoveEnemiesDown(shiftDownAmount);
                hasShiftedDown = true;
                direction *= -1;
                curSpeed += speedIncrement;
            }
        } else {
            hasShiftedDown = false;
        }
        MoveEnemiesHorizontally(curSpeed * direction * Time.deltaTime);
    }


    private bool CheckWin() {
        for (var j = 0; j < enemies.GetLength(1); j++) {
            for (var i = 0; i < enemies.GetLength(0); i++) {
                if (enemies[i, j] != null)
                    return false;
            }
        }
        return true;
    }


    private void UpdateBoundaries() {
        minX = getMinX();
        maxX = getMaxX();
    }

    
    private float getMinX() {
        var min = float.MaxValue; 
        for (var j = 0; j < enemies.GetLength(1); j++) {
            for (var i = 0; i < enemies.GetLength(0); i++) {
                var enemy = enemies[i, j];
                if (enemy != null) {
                    min = Mathf.Min(min, enemy.transform.position.x); 
                }
            }
        }
        return Math.Abs(min - float.MaxValue) < 0.05f ? 0f : min; 
    }

    private float getMinY() {
        var minY = float.MaxValue;
        for (var j = 0; j < enemies.GetLength(1); j++) {
            for (var i = 0; i < enemies.GetLength(0); i++) {
                var enemy = enemies[i, j];
                if (enemy != null) {
                    minY = Mathf.Min(minY, enemy.transform.position.y);
                }
            }
        }
        
        return Mathf.Approximately(minY, float.MaxValue) ? 0f : minY;
    }

    
    private float getMaxX() {
        var max = float.MinValue; 
        for (var j = enemies.GetLength(1) - 1; j >= 0; j--) {
            for (var i = 0; i < enemies.GetLength(0); i++) {
                var enemy = enemies[i, j];
                if (enemy != null) {
                    max = Mathf.Max(max, enemy.transform.position.x);
                }
            }
        }
        return Math.Abs(max - float.MinValue) < 0.05f ? 0f : max; 
    }

    private void MoveEnemiesHorizontally(float xAmount) {
        foreach (var enemy in enemies) {
            if (enemy != null) {
                enemy.transform.position += new Vector3(xAmount, 0, 0);
            }
        }
    }

    private void MoveEnemiesDown(float yAmount) {
        foreach (var enemy in enemies) {
            if (enemy != null) {
                enemy.transform.position -= new Vector3(0, yAmount, 0);
            }
        }
    }

    /**
     * Player presses enter key to start
     */
    public void StartGameInput(InputAction.CallbackContext callbackContext) {
        if(isRunning)
            return;
        InitGame();
        uiManager.hideStartGame();
        uiManager.hideGameOver();
    }
    
    private void gameOver() {
        PauseGame();
        uiManager.showGameOver();
        uiManager.ShowHighScoreDialog();
        enemyFactory.clearEnemyGrid();
        lives = 3;
    }


/*
 * UNITY EVENT METHODS FROM PROJECTILE PREFAB
 */
    public void onEnemyHit(GameObject gameObj) {
        gameObj.GetComponent<Enemy>().onDeath();
        score += 100; 
        uiManager.setScore(score);
    }

    public void onPlayerHit(GameObject gameObj) {
        lives--;
        playerController.PlayerDeath();
        if (lives > 0) {
            isPlayerDeathTimerRunning = true;
            uiManager.removeLife();
        }
        else {
            gameOver();
        }
    }
    

    private void PauseGame() {
        isRunning = false;
        isPaused = true;
        _onPause?.Invoke();   
    }

    private void ResumeGame() {
        isRunning = true;
        isPaused = false;
        _onResume?.Invoke();
    }
    
    public void onShieldHit(GameObject gameObj) {
        gameObj.GetComponent<Shield>().TakeDamage();
    }

    private void EnemyShieldCollision() {
        gameOver();
    }

    public void onMothershipHit(GameObject gameObj) {
        gameObj.GetComponent<Mothership>();
        score += 300; 
        uiManager.setScore(score);
    }

    private void SavePlayerHighScore(String initials) {
        var hs = new HighScore(score, initials);

        if (!isRunning) {
            if (hs.score == lastHighScore.score && hs.playerInitials == lastHighScore.playerInitials)
                return;            
            Debug.Log($"Saving new high score: Initials: {initials}, Score {score}");
            scoreHandler.SaveScore(hs);
            lastHighScore = hs;
            InitHighScores();
        }
    }
    
}