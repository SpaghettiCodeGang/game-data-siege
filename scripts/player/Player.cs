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
    [Export] public XRController3D RightController;
    [Export] public XRController3D LeftController;
    [Export] public int MaxHealth = 10;

    public PlayerCombat _combat;
    
    private RigidBody3D _currentGun;
    private RigidBody3D _currentMagazine;
    private RigidBody3D _loadedMagazine;
    private BaseStage _currentStage;
    
    private bool _prevAButton;
    private bool _prevBButton;
    
    /// <summary>
    /// Called every frame. Handles input from the VR controllers.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public override void _Process(double delta)
    {
        if (LeftController == null) return;
        if (LeftController.IsButtonPressed("menu_button"))
        {
            _currentGun?.Call("on_magazine_ejected");
            GameManager.Instance.ReturnToMenu();
        }
        
        var bPressed = LeftController.IsButtonPressed("ax_button");
        if (bPressed && !_prevBButton) _currentStage.OnPlayerButtonPressed("X");
        _prevBButton = bPressed;
        
        
        if (RightController == null) return;
        var aPressed = RightController.IsButtonPressed("ax_button");
        if (aPressed && !_prevAButton) _currentStage.OnPlayerButtonPressed("A");
        _prevAButton = aPressed;
    }

    /// <summary>
    /// Assigns the current stage the player is in.
    /// </summary>
    /// <param name="stage">The stage to associate with the player.</param>
    public void SetCurrentStage(BaseStage stage) => _currentStage = stage;

    /// <summary>
    /// Spawns a gun at the right holster position.
    /// Connects relevant signals for pickup and loading events.
    /// </summary>
    public void SpawnGun()
    {
        // Return early if required resources are not assigned
        if (GunScene == null || RightHolster == null) return;

        _currentGun = GunScene.Instantiate<RigidBody3D>();
        RightHolster.AddChild(_currentGun);
    
        _currentGun.Position = Vector3.Zero;
        _currentGun.Rotation = Vector3.Zero;
        _currentGun.Freeze = true;
        
        // Connect signals from Gun.gd
        _currentGun.Connect("gun_picked_up", 
            Callable.From(OnGunPickedUp));
        _currentGun.Connect("gun_loaded",
            Callable.From(OnGunLoaded));
        _currentGun.Connect("magazine_ejected",
            Callable.From(() => _loadedMagazine = null));
    }
    
    /// <summary>
    /// Triggered when the gun is picked up by the player.
    /// Moves the gun from the holster to the active stage.
    /// </summary>
    private void OnGunPickedUp()
    {
        if (_currentGun  == null) return;
        if (_currentGun.GetParent() != RightHolster) return;
        RightHolster.RemoveChild(_currentGun);
        _currentStage.AddChild(_currentGun);

        if (_currentStage is TutorialStage)
        {
            _currentStage.OnPlayerButtonPressed("Picked");
        }
    }

    /// <summary>
    /// Triggered when the gun is loaded with a magazine.
    /// Sends a signal to the current stage (used in TutorialStage).
    /// </summary>
    private void OnGunLoaded()
    {
        _loadedMagazine = _currentMagazine;
        SpawnMagazine();
        if (_currentStage is TutorialStage)
        {
            _currentStage.OnPlayerButtonPressed("Loaded");
        }
    }
    
    /// <summary>
    /// Removes the currently spawned gun from the game.
    /// </summary>
    public void RemoveGun()
    {
        if (_currentGun == null || _currentGun.IsQueuedForDeletion()) return;
        _currentGun.QueueFree();
        _currentGun = null;
    }

    /// <summary>
    /// Spawns a magazine at the left magazine box position.
    /// Connects the pickup signal for interaction.
    /// </summary>
    public void SpawnMagazine()
    {
        // Return early if required resources are not assigned
        if (MagazineScene == null || LeftMagBox == null) return;
        
        _currentMagazine = MagazineScene.Instantiate<RigidBody3D>();
        LeftMagBox.AddChild(_currentMagazine);
    
        _currentMagazine.Position = Vector3.Zero;
        _currentMagazine.Rotation = Vector3.Zero;
        _currentMagazine.Freeze = true;
        
        // Connect signals from Magazine.gd
        _currentMagazine.Connect("magazine_picked_up", 
            Callable.From(OnMagazinePickedUp));
        _currentMagazine.Connect("magazine_despawned", 
            Callable.From(OnMagazineDespawned));
    }
    
    /// <summary>
    /// Triggered when the magazine is picked up by the player.
    /// Moves the magazine from the mag box to the active stage.
    /// </summary>
    private void OnMagazinePickedUp()
    {
        if (_currentMagazine == null) return;
        if (_currentMagazine.GetParent() != LeftMagBox) return;
        LeftMagBox.RemoveChild(_currentMagazine);
        _currentStage.AddChild(_currentMagazine);
    }
    
    /// <summary>
    /// Triggered when the currently active magazine despawns.
    /// Clears the reference and spawns a new magazine unless the player is in the menu stage.
    /// </summary>
    private void OnMagazineDespawned()
    {
        if (_currentStage is MenuStage) return;
        _currentMagazine = null;
        SpawnMagazine();
    }
    
    /// <summary>
    /// Removes the currently spawned magazine from the game.
    /// </summary>
    public void RemoveMagazine()
    {
        if (_currentMagazine == null || _currentMagazine.IsQueuedForDeletion()) return;
        _currentMagazine.QueueFree();
        _currentMagazine = null;
    }
    

    /// <summary>
    /// Shows the laser pointer on both VR controllers.
    /// This manipulates the `FunctionPointer` node from Godot XR Tools.
    /// A value of 2 sets the laser to collide.
    /// </summary>
    public void ShowAllLasers()
    {
        foreach (var controller in new[] { RightController, LeftController })
        {
            var fp = controller.GetNodeOrNull<Node>("FunctionPointer");
            if (fp != null)
            {
                fp.Call("set_show_laser", 2);
                fp.Call("_update_pointer");
            }
        }
    }
    
    /// <summary>
    /// Hides the laser pointer on both VR controllers.
    /// This manipulates the `FunctionPointer` node from Godot XR Tools.
    /// A value of 0 disables the laser.
    /// </summary>
    public void HideAllLasers()
    {
        foreach (var controller in new[] { RightController, LeftController })
        {
            var fp = controller.GetNodeOrNull<Node>("FunctionPointer");
            if (fp != null)
            {
                fp.Call("set_show_laser", 0);
                fp.Call("_update_pointer");
            }
        }
    }
    
}