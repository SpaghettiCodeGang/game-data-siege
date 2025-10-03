using Godot;

/// <summary>
/// Represents the player entity in the game.
/// Acts as a central interface for player-related actions, abilities, and input handling.
/// Provides methods for spawning/removing items, interacting with the environment,
/// and reacting to controller input.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class Player : Node3D
{
    [Export] public Node3D RightHolster;
    [Export] public Node3D LeftMagBox;
    [Export] public PackedScene GunScene;
    [Export] public PackedScene MagazineScene;
    [Export] private XRController3D _rightController;
    [Export] private XRController3D _leftController;
    
    private Gun _currentGun;
    private Magazin _currentMagazine;
    private BaseStage _currentStage;
    
    private bool _prevAButton;
    private bool _prevBButton;
    
    /// <summary>
    /// Called every frame. Handles input from the VR controllers.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public override void _Process(double delta)
    {
        if (_leftController == null) return;
        if (_leftController.IsButtonPressed("menu_button")) GameManager.Instance.ReturnToMenu();
        
        if (_rightController == null) return;
        var aPressed = _rightController.IsButtonPressed("ax_button");
        var bPressed = _rightController.IsButtonPressed("by_button");

        if (aPressed && !_prevAButton) _currentStage.OnPlayerButtonPressed("A");
        if (bPressed && !_prevBButton) _currentStage.OnPlayerButtonPressed("B");

        _prevAButton = aPressed;
        _prevBButton = bPressed;
    }

    public void SetCurrentStage(BaseStage stage)
    {
        _currentStage = stage;
    }

    public void SpawnGun()
    {
        // Return early if required resources are not assigned
        if (GunScene == null || RightHolster == null) return;
        
        _currentGun = GunScene.Instantiate<Gun>();
        RightHolster.AddChild(_currentGun);
    }
    
    public void RemoveGun()
    {
        _currentGun?.QueueFree();
        _currentGun = null;
    }

    public void SpawnMagazine()
    {
        // Return early if required resources are not assigned
        if (MagazineScene == null || LeftMagBox == null) return;
        
        _currentMagazine = MagazineScene.Instantiate<Magazin>();
        LeftMagBox.AddChild(_currentMagazine);
    }
    
    public void RemoveMagazine()
    {
        _currentMagazine?.QueueFree();
        _currentMagazine = null;
    }
}