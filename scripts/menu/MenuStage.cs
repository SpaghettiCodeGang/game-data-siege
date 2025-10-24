using Godot;

/// <summary>
/// Stage representing the game's menu stage.
/// Resets the player's position and rotation when entered.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class MenuStage : BaseStage
{
    public override async void OnEnter()
    {
        if (Player == null) return;
        
        Player.PlayerLaserHandler?.ShowAllLasers();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        GD.Print("Entered Menu Stage, Highscore:" + DataManager.Instance.Highscore);
        GD.Print("Current Score:" + DataManager.Instance.CurrentScore);
    }

    public override void OnExit()
    {
        Player.PlayerLaserHandler?.HideAllLasers();
    }
}