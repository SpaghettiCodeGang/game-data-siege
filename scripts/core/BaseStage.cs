using Godot;

/// <summary>
/// Base class for a game stage/level.
/// Provides lifecycle hooks and a reference to the active player.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class BaseStage : Node3D
{
    /// <summary>
    /// Optional spawn point for the player in this stage.
    /// If set in the scene, the player will be moved to this marker on stage enter.
    /// </summary>
    [Export] protected Marker3D PlayerPositionMarker;
    
    /// <summary>
    /// Reference to the player instance currently active in this stage.
    /// Set by <see cref="SetPlayer"/> when the GameManager loads the stage.
    /// </summary>
    protected Player Player;

    /// <summary>
    /// Assigns the player instance to the stage and optionally repositions them.
    /// </summary>
    /// <param name="player">The player instance.</param>
    /// <remarks>
    /// If the stage has a <see cref="Marker3D"/> assigned to <c>PlayerPositionMarker</c>,
    /// the player will be teleported to that position and orientation when entering the stage.
    /// This allows each stage to define its own spawn point.
    /// </remarks>
    public virtual void SetPlayer(Player player)
    {
        Player = player;
        
        if (PlayerPositionMarker == null) return;
        Player.GlobalTransform = PlayerPositionMarker.GlobalTransform;
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
    
    /// <summary>
    /// Called when the player presses a button on their controller while this stage is active.
    /// Override this method in derived stages to implement custom behavior for different buttons.
    /// </summary>
    /// <param name="buttonName">The name or identifier of the button that was pressed.</param>
    public virtual void OnPlayerButtonPressed(string buttonName) { }
}