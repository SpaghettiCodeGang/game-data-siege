using Godot;

namespace gamedatasiege.scripts.player;

/// <summary>
/// Represents the Player entity in the game.
/// Handles spawning and removing weapons and magazines,
/// as well as basic input for returning to the main menu.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class Player : Node
{
    [Export] public Node3D RightHolster;
    [Export] public Node3D LeftMagBox;
    [Export] public PackedScene GunScene;
    [Export] public PackedScene MagazineScene;
    [Export] private XRController3D _rightController;
    [Export] private XRController3D _leftController;
    
    private Gun _currentGun;
    private Magazin _currentMagazine;
    
    /// <summary>
    /// Called every frame. Checks for input to return to the menu.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public override void _Process(double delta)
    {
        if (_leftController != null && _leftController.IsButtonPressed("menu_button"))
        {
            core.GameManager.Instance.ReturnToMenu();
        }
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