using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ScoreHandler {

    private string filePath = Application.persistentDataPath + "/highscores.json";
    FileStream file;

    private HighScore baseHighScore = new(0, "BBB");
    private bool isReading;
    private bool isWriting;
    public ScoreHandler() {
        if (GetScores() == null) {
            for (var i = 0; i < 5; i++) {
                SaveScore(baseHighScore);
            }
        }
    }
    public void SaveScore(HighScore score) {
        Debug.Log("Saving new score!");
        if(score == null)
            return;
        var scores = GetScores();
        if (scores == null) {
            return;
        }
        if(isReading)
            return;
            
        var scoreList = new List<HighScore>(scores) { score };

        scoreList.Sort((a, b) => b.score.CompareTo(a.score)); 
        if (scoreList.Count > 5) {
            scoreList = scoreList.GetRange(0, 5); 
        }

        var json = JsonUtility.ToJson(new HighScoreList { scores = scoreList.ToArray() });
        
        if (File.Exists(filePath))
            file = File.OpenWrite(filePath);
        else 
            return;

        var bf = new BinaryFormatter();
        
        bf.Serialize(file, json);
        
        file.Close();
    }

    public HighScore[] GetScores()
    {
        if (!File.Exists(filePath)) {
            File.Create(filePath);
            return null;
        }

        if (isWriting)
            return null;
        
        var bf = new BinaryFormatter();
        try {
            file = File.OpenRead(filePath);
            isReading = true;
            var json = (string) bf.Deserialize(file);
            file.Close();
            
            var highScoreList = JsonUtility.FromJson<HighScoreList>(json);
            return highScoreList.scores;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return null; 
        }

    }
    
    [System.Serializable]
    private class HighScoreList {
        public HighScore[] scores;
    }
}
