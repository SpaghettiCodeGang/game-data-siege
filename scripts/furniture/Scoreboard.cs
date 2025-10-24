using Godot;
using System;

public partial class Scoreboard : Node3D
{
    [Export] public RichTextLabel MainText;
    public override void _Ready()
    {
        MainText.Text = "Highscore: " + DataManager.Instance.Highscore + "\n\ncurrent Score:" + DataManager.Instance.CurrentScore;
    }
}
