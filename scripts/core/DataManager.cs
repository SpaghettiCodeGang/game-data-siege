using Godot;
using System;

public partial class DataManager : Node
{
    public static DataManager Instance { get; private set; }
    public int Highscore { get; set; }
    public int CurrentScore { get; set; }
    private const string DataPath = "user://saveGame.save";
    
    public override void _Ready()
    {
        if (Instance == null) Instance = this;
        LoadData();
    }

    public void UpdateScore(int score)
    {
        CurrentScore = score;
        if (CurrentScore > Highscore) Highscore = CurrentScore;
        SaveData();
    }

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
