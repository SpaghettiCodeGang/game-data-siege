using Godot;

/// <summary>
/// Stage representing the game's menu stage.
/// Resets the player's position and rotation when entered.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class MenuStage : BaseStage
{
    public override void OnEnter()
    {
        if (Player == null) return;
        
        Player.PlayerLaserHandler?.ShowAllLasers();
    }

    public override void OnExit()
    {
        Player.PlayerLaserHandler?.HideAllLasers();
    }
}