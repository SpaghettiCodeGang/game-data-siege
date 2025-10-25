using Godot;
using System;

/// <summary>
/// Displays the player's highscore and current score on a scoreboard.
/// Updates the text display with data from the DataManager.
/// </summary>
/// <author>Elias Kugel</author>
public partial class Scoreboard : Node3D
{
    [Export] public RichTextLabel MainText;
    
    /// <summary>
    /// Initializes the scoreboard by fetching and displaying the highscore and current score
    /// from the DataManager instance.
    /// </summary>
    public override void _Ready()
    {
        MainText.Text = "Highscore: " + DataManager.Instance.Highscore + "\n\nAktueller Score:" + DataManager.Instance.CurrentScore;
    }
}
