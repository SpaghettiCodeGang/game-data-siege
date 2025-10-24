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
        
        // Move the player to the world origin and reset rotation
        Player.GlobalPosition = Vector3.Zero;
        Player.GlobalRotation = Vector3.Zero;
        
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