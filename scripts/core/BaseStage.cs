using Godot;

/// <summary>
/// Base class for a game stage/level.
/// Provides lifecycle hooks and a reference to the active player.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class BaseStage : Node3D
{
    /// <summary>
    /// Reference to the player assigned to this stage.
    /// </summary>
    protected Player Player;

    /// <summary>
    /// Assigns the player instance to the stage.
    /// </summary>
    /// <param name="player">The player instance.</param>
    public virtual void SetPlayer(Player player)
    {
        Player = player;
    }

    /// <summary>
    /// Called when entering the stage. 
    /// Override in derived classes to set up the stage.
    /// </summary>
    public virtual void OnEnter() { }
    
    /// <summary>
    /// Called when exiting the stage.
    /// Override in derived classes to clean up resources or state.
    /// </summary>
    public virtual void OnExit() { }
}