using Godot;
using System;

public class NewScore
{
    public readonly int Highscore;
    public readonly int CurrentScore;
    
    public NewScore(int currentScore, int highscore)
    {
        if (currentScore >= highscore)
        {
            Highscore = currentScore;
        }
        else
        {
            Highscore = highscore;
        }
        CurrentScore = currentScore;
    }
}
