using Godot;
using System;

/// <summary>
/// Manages game data persistence, including saving and loading player scores.
/// </summary>
/// <author>Elias Kugel</author>
/// <coauthor>Sören Lehmann</coauthor>
public partial class DataManager : Node
{
    public static DataManager Instance { get; private set; }
    public int Highscore { get; set; }
    public int CurrentScore { get; set; }
    private const string DataPath = "user://saveGame.save";
    /// <summary>
    /// Initializes the singleton instance and loads saved data from disk.
    /// </summary>
    public override void _Ready()
    {
        if (Instance == null) Instance = this;
        LoadData();
    }

    /// <summary>
    /// Updates the current score and saves data if a new high score is achieved.
    /// Automatically persists data to disk after updating.
    /// </summary>
    /// <param name="score">The new score to set as the current score.</param>
    public void UpdateScore(int score)
    {
        CurrentScore = score;
        if (CurrentScore > Highscore) Highscore = CurrentScore;
        SaveData();
    }

    /// <summary>
    /// Saves the current game data (highscore and current score) to disk.
    /// </summary>
    private void SaveData()
    {
        using var file = FileAccess.Open(DataPath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr("Failed to write the file.");
            return;
        }

        var data = new Godot.Collections.Dictionary<string, Variant>
        {
            {"highscore", Highscore},
            {"currentscore", CurrentScore}
        };
        
        file.StoreVar(data);
        file.Close();
    }
    
    /// <summary>
    /// Loads game data from disk if a save file exists.
    /// Reads highscore and current score from the saved file.
    /// </summary>
    private void LoadData()
    {
        if (!FileAccess.FileExists(DataPath)) return;
        
        using var file = FileAccess.Open(DataPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr("Failed to read the file.");
            return;
        }

        var data = (Godot.Collections.Dictionary<string, Variant>)file.GetVar();
        if (data.TryGetValue("highscore", out var highscore)) Highscore = (int)highscore;
        if (data.TryGetValue("currentscore", out var currentscore)) CurrentScore = (int)currentscore;
        
        file.Close();
    }
}
