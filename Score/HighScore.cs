using System;

[Serializable]
public class HighScore {
    public int score;
    public string playerInitials;

    public HighScore(int sc, string inits) {
        score = sc;
        playerInitials = inits;
    }
}