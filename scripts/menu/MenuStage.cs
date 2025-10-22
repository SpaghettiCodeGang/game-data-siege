using Godot;

/// <summary>
/// Stage representing the game's menu stage.
/// Resets the player's position and rotation when entered.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class MenuStage : BaseStage
{
    [Export] public Highscoreboard Highscoreboard;

    public override void OnEnter()
    {
        if (Player == null) return;
        SetScore(GameManager.Instance.CurrentScore);
        // Move the player to the world origin and reset rotation
        Player.GlobalPosition = Vector3.Zero;
        Player.GlobalRotation = Vector3.Zero;
        
        
        Player.PlayerLaserHandler?.ShowAllLasers();
    }

    public override void OnExit()
    {
        Player.PlayerLaserHandler?.HideAllLasers();
    }

    public void SetScore(int currentScore)
    {
        GD.Print("MenuStage SetScore called with currentScore: " + currentScore);
        if (Highscoreboard != null)
        {
            Highscoreboard.setHighscore(GameManager.Instance.Highscore);
            Highscoreboard.setCurrentscore(GameManager.Instance.CurrentScore);
        }
    }
}