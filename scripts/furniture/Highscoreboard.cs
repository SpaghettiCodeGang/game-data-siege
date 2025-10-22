using Godot;
using System;

public partial class Highscoreboard : Node3D
{
    [Export] public RichTextLabel CurrentScore;
    [Export] public RichTextLabel Highscore;
    
    public void setCurrentscore(int currentScore)
    {
        CurrentScore.Text = "current Score: " + currentScore;
        GD.Print("current Score: " + currentScore);
    }
    
    public void setHighscore(int highscore)
    {
        Highscore.Text = "Highscore: " + highscore;
        GD.Print("Highscore: " + highscore);
    }
}