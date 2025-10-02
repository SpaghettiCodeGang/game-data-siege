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
    [Export] public Area3D RightHolster;
    [Export] public Area3D LeftMagBox;
    [Export] public PackedScene GunScene;
    [Export] public PackedScene MagazineScene;
    [Export] private XRController3D _rightController;
    [Export] private XRController3D _leftController;
    
    private RigidBody3D _currentGun;
    private RigidBody3D _currentMagazine;
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
        if (_leftController.IsButtonPressed("menu_button"))
        {
            _currentGun?.Call("on_magazine_ejected");
            GameManager.Instance.ReturnToMenu();
        }
        
        var bPressed = _leftController.IsButtonPressed("ax_button");
        if (bPressed && !_prevBButton) _currentStage.OnPlayerButtonPressed("X");
        _prevBButton = bPressed;
        
        
        if (_rightController == null) return;
        var aPressed = _rightController.IsButtonPressed("ax_button");
        if (aPressed && !_prevAButton) _currentStage.OnPlayerButtonPressed("A");
        _prevAButton = aPressed;
    }

    public void SetCurrentStage(BaseStage stage)
    {
        _currentStage = stage;
    }

    public void SpawnGun()
    {
        // Return early if required resources are not assigned
        if (GunScene == null || RightHolster == null) return;

        _currentGun = GunScene.Instantiate<RigidBody3D>();
        RightHolster.AddChild(_currentGun);
    
        _currentGun.Position = Vector3.Zero;
        _currentGun.Rotation = Vector3.Zero;
        _currentGun.Freeze = true;
        
        // Verbinde Signal
        _currentGun.Connect("gun_picked_up", 
            Callable.From(OnGunPickedUp));
        _currentGun.Connect("gun_loaded",
            Callable.From(OnGunLoaded));
    }
    
    private void OnGunPickedUp()
    {
        if (_currentGun.GetParent() != RightHolster) return;
        RightHolster.RemoveChild(_currentGun);
        _currentStage.AddChild(_currentGun);

        if (_currentStage is TutorialStage)
        {
            _currentStage.OnPlayerButtonPressed("Picked");
        }
    }

    private void OnGunLoaded()
    {
        if (_currentStage is TutorialStage)
        {
            _currentStage.OnPlayerButtonPressed("Loaded");
        }
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
        
        _currentMagazine = MagazineScene.Instantiate<RigidBody3D>();
        LeftMagBox.AddChild(_currentMagazine);
    
        _currentMagazine.Position = Vector3.Zero;
        _currentMagazine.Rotation = Vector3.Zero;
        _currentMagazine.Freeze = true;
        
        // Verbinde Signal
        _currentMagazine.Connect("magazine_picked_up", 
            Callable.From(OnMagazinePickedUp));
    }
    
    private void OnMagazinePickedUp()
    {
        if (_currentMagazine.GetParent() != LeftMagBox) return;
        LeftMagBox.RemoveChild(_currentMagazine);
        _currentStage.AddChild(_currentMagazine);
    }
    
    public void RemoveMagazine()
    {
        _currentMagazine?.QueueFree();
        _currentMagazine = null;
    }
    
    
}