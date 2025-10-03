using Godot;

/// <summary>
/// Central game manager implemented as a singleton.
/// Handles loading and unloading of stages (levels),
/// assigns the player to stages, and provides a method to return to the menu.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class GameManager : Node
{
    [Export] public PackedScene MenuStage;
    
    [Export] private Node3D _currentStage;
    [Export] private Player _player;

    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Initializes the singleton instance and loads the menu stage at startup.
    /// </summary>
    public override void _Ready()
    {
        if (Instance == null) Instance = this;
        LoadStage(MenuStage);
    }
    
    /// <summary>
    /// Loads a new stage by instantiating the given scene,
    /// cleaning up the previous one, and assigning the player if available.
    /// </summary>
    /// <param name="packedScene">The scene resource representing the stage to load.</param>
    public void LoadStage(PackedScene packedScene)
    {
        if (packedScene == null) return;
        
        // Clean up existing stage
        foreach (var child in _currentStage.GetChildren())
        {
            if (child is BaseStage baseStageOld) baseStageOld.OnExit();
            child.QueueFree();
        }

        // Instantiate and add the new stage
        var instance = packedScene.Instantiate();
        _currentStage.AddChild(instance);

        // If the stage derives from BaseStage, pass the player and call OnEnter
        if (instance is not BaseStage baseStage || _player == null) return;
        baseStage.SetPlayer(_player);
        _player.SetCurrentStage(baseStage);
        baseStage.OnEnter();
    }
    
    /// <summary>
    /// Returns to the menu stage if the current stage is not already the menu.
    /// Triggered e.g. via VR controller input.
    /// </summary>
    public void ReturnToMenu()
    {
        // Ensure there is at least one stage loaded
        if (_currentStage.GetChildCount() <= 0) return;
        var currentSceneNode = _currentStage.GetChild(0);

        // Prevent reloading if we are already in the menu
        if (currentSceneNode is Menu) return;
        LoadStage(MenuStage);
    }
}