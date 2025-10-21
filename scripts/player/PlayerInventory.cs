using Godot;

/// <summary>
/// Manages all equipment-related entities for the player,
/// including spawning, removal, and state tracking of guns and magazines.
/// 
/// This class encapsulates the logic for weapon lifecycle management
/// (holstering, pickup, loading, despawning) and communicates
/// with the current stage for context-sensitive events such as tutorials.
/// </summary>
/// <author>Sören Lehmann</author>
public class PlayerInventory
{
    private readonly Player _player;
    public RigidBody3D CurrentGun;
    public RigidBody3D CurrentMagazine;
    
    public PlayerInventory(Player player)
    {
        _player = player;
    }
    
        /// <summary>
    /// Spawns a gun at the right holster position.
    /// Connects relevant signals for pickup and loading events.
    /// </summary>
    public void SpawnGun()
    {
        // Return early if required resources are not assigned
        if (_player.GunScene == null || _player.RightHolster == null) return;

        CurrentGun = _player.GunScene.Instantiate<RigidBody3D>();
        _player.RightHolster.AddChild(CurrentGun);
    
        CurrentGun.Position = Vector3.Zero;
        CurrentGun.Rotation = Vector3.Zero;
        CurrentGun.Freeze = true;
        
        // Connect signals from Gun.gd
        CurrentGun.Connect("gun_picked_up", 
            Callable.From(OnGunPickedUp));
        CurrentGun.Connect("gun_despawned",
            Callable.From(OnGunDespawned));        
        CurrentGun.Connect("gun_loaded",
            Callable.From(OnGunLoaded));
    }
    
    /// <summary>
    /// Triggered when the gun is picked up by the player.
    /// Moves the gun from the holster to the active stage.
    /// </summary>
    private void OnGunPickedUp()
    {
        if (CurrentGun  == null) return;
        if (CurrentGun.GetParent() != _player.RightHolster) return;
        _player.RightHolster.RemoveChild(CurrentGun);
        _player.CurrentStage.AddChild(CurrentGun);

        if (_player.CurrentStage is TutorialStage)
        {
            _player.CurrentStage.OnPlayerButtonPressed("Picked");
        }
    }
    
    /// <summary>
    /// Triggered when the currently active Gun despawns.
    /// Clears the reference and spawns a new Gun unless the player is in the menu stage.
    /// </summary>
    private void OnGunDespawned()
    {
        if (_player.CurrentStage is MenuStage) return;
        CurrentGun = null;
        SpawnGun();
    }

    /// <summary>
    /// Triggered when the gun is loaded with a magazine.
    /// Sends a signal to the current stage (used in TutorialStage).
    /// </summary>
    private void OnGunLoaded()
    {
        SpawnMagazine();
        if (_player.CurrentStage is TutorialStage)
        {
            _player.CurrentStage.OnPlayerButtonPressed("Loaded");
        }
    }
    
    /// <summary>
    /// Removes the currently spawned gun from the game.
    /// </summary>
    public void RemoveGun()
    {
        if (CurrentGun == null || CurrentGun.IsQueuedForDeletion()) return;
        CurrentGun.QueueFree();
        CurrentGun = null;
    }

    /// <summary>
    /// Spawns a magazine at the left magazine box position.
    /// Connects the pickup signal for interaction.
    /// </summary>
    public void SpawnMagazine()
    {
        // Return early if required resources are not assigned
        if (_player.MagazineScene == null || _player.LeftMagBox == null) return;
        
        CurrentMagazine = _player.MagazineScene.Instantiate<RigidBody3D>();
        _player.LeftMagBox.AddChild(CurrentMagazine);
    
        CurrentMagazine.Position = Vector3.Zero;
        CurrentMagazine.Rotation = Vector3.Zero;
        CurrentMagazine.Freeze = true;
        
        // Connect signals from Magazine.gd
        CurrentMagazine.Connect("magazine_picked_up", 
            Callable.From(OnMagazinePickedUp));
        CurrentMagazine.Connect("magazine_despawned", 
            Callable.From(OnMagazineDespawned));
    }
    
    /// <summary>
    /// Triggered when the magazine is picked up by the player.
    /// Moves the magazine from the mag box to the active stage.
    /// </summary>
    private void OnMagazinePickedUp()
    {
        if (CurrentMagazine == null) return;
        if (CurrentMagazine.GetParent() != _player.LeftMagBox) return;
        _player.LeftMagBox.RemoveChild(CurrentMagazine);
        _player.CurrentStage.AddChild(CurrentMagazine);
    }
    
    /// <summary>
    /// Triggered when the currently active magazine despawns.
    /// Clears the reference and spawns a new magazine unless the player is in the menu stage.
    /// </summary>
    private void OnMagazineDespawned()
    {
        if (_player.CurrentStage is MenuStage) return;
        CurrentMagazine = null;
        SpawnMagazine();
    }
    
    /// <summary>
    /// Removes the currently spawned magazine from the game.
    /// </summary>
    public void RemoveMagazine()
    {
        if (CurrentMagazine == null || CurrentMagazine.IsQueuedForDeletion()) return;
        CurrentMagazine.QueueFree();
        CurrentMagazine = null;
    }
}