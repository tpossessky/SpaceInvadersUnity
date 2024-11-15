using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image[] lives;
    [SerializeField] private TextMeshProUGUI[] highScores;
    [SerializeField] private TextMeshProUGUI highScoreTitle;

    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI startGameText;
    private int curLives;
    
    [FormerlySerializedAs("initials")] [SerializeField] private TextMeshProUGUI[] initialsUI;
    private char[] curInitials = {'A', 'A', 'A'};
    private int curInitialIndex;
    
    private float inputCooldown = 0.05f; // Cooldown time in seconds
    private float nextInputTime;

    public Action<string> onPlayerEnteredHighScore;
    private void Start() {
        HideHighScoreDialog();
        hideGameOver();
        curLives = lives.Length - 1;
        showStartGame();
        
    }

    public void setScore(int newScore) {
        var formattedScore = newScore.ToString("D8");
        scoreText.text = $"SCORE: {formattedScore}";
    }

    public void removeLife() {
        lives[curLives].enabled = false;
        curLives--;
    }

    public void newGame() {
        foreach(var life in lives) {
            life.enabled = true;
        }
        curLives = lives.Length - 1;
        curInitials[0] = 'A';
        curInitials[1] = 'A';
        curInitials[2] = 'A';
    }

    public void showStartGame() {
        startGameText.alpha = 255f;
    }

    public void hideStartGame() {
        startGameText.alpha = 0f;
    }

    public void showGameOver() {
        gameOverText.alpha = 255f;
    }

    public void hideGameOver() {
        gameOverText.alpha = 0f;
    }

    public void ShowHighScoreDialog() {
        initialsUI[0].alpha = 255f;
        initialsUI[1].alpha = 255f;
        initialsUI[2].alpha = 255f;
        highScoreTitle.alpha = 255f;
    }

    public void HideHighScoreDialog() {
        initialsUI[0].alpha = 0f;
        initialsUI[1].alpha = 0f;
        initialsUI[2].alpha = 0f;
        highScoreTitle.alpha = 0f;
    }

    public void SetHighScores(HighScore[] scores) {
        for(var i = 0; i < highScores.Length; i++) {
            var formattedNum = scores[i].score.ToString("D8");
            highScores[i].text = $"{scores[i].playerInitials} {formattedNum}";
        }
    }


    public void OnInitialsEnter(InputAction.CallbackContext callbackContext)
    {
        if(gameOverText.alpha < 1)
            return;
        if (Time.time < nextInputTime) 
            return; 

        var input = callbackContext.ReadValue<Vector2>();
        var leftRight = Mathf.RoundToInt(input.x);
       
        var upDown = Mathf.RoundToInt(input.y);

        if (upDown > 0) {
            curInitials[curInitialIndex]++;
            //wrap around
            if (curInitials[curInitialIndex] > 'Z') 
                curInitials[curInitialIndex] = 'A'; 
        }
        else if (upDown < 0) {
            curInitials[curInitialIndex]--;
            // wrap around
            if (curInitials[curInitialIndex] < 'A') 
                curInitials[curInitialIndex] = 'Z'; 
        }

        if (leftRight > 0) {
            if (curInitialIndex < 2) {
                curInitialIndex++;
            }
        }
        else if(leftRight < 0) {
            if (curInitialIndex > 0) {
                curInitialIndex--;
            }
        }

        nextInputTime = Time.time + inputCooldown; // Reset cooldown
        UpdateCurUIInitial();
    }

    private void UpdateCurUIInitial() {
        
        initialsUI[curInitialIndex].text = curInitials[curInitialIndex].ToString();
    }

    public void OnConfirmInitials(InputAction.CallbackContext callbackContext) {
        var builder = new StringBuilder();
        builder.Append(curInitials[0]);
        builder.Append(curInitials[1]);
        builder.Append(curInitials[2]);
        
        onPlayerEnteredHighScore?.Invoke(builder.ToString());
    }
    
}