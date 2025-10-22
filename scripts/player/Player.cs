using Godot;

/// <summary>
/// Represents the player entity in the game.
/// 
/// This class acts as the main interface for all player-related functionality,
/// including stats, combat, inventory, input, and interactions with the current stage.
/// It serves primarily as a composition root — delegating logic to helper classes
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
    [Export] public PlayerDamageOverlay PlayerDamageOverlay;
    [Export] public AudioStreamPlayer3D PlayerDeathSequenz;
    [Export] public AudioStreamPlayer3D PlayerHitSequenz;
    [Export] public float MaxHealth = 10;
    public float CurrentHealth;

    public PlayerCombat PlayerCombat { get; private set; }
    public PlayerInventory PlayerInventory { get; private set; }
    public PlayerInputHandler PlayerInputHandler { get; private set; }
    public PlayerLaserHandler PlayerLaserHandler { get; private set; }
    public BaseStage CurrentStage { get; private set; }

    /// <summary>
    /// Initializes all player subsystems once the node is added to the scene tree.
    /// Each subsystem receives a reference to this player for context and access
    /// to exported properties.
    /// </summary>
    public override void _Ready()
    {
        PlayerCombat = new PlayerCombat(this);
        PlayerInventory = new PlayerInventory(this);
        PlayerInputHandler = new PlayerInputHandler(this);
        PlayerLaserHandler = new PlayerLaserHandler(this);
        CurrentHealth = MaxHealth;
    }
    
    /// <summary>
    /// Called every frame to process player input via the associated input handler.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public override void _Process(double delta)
    {
        PlayerInputHandler.ProcessInput();
    }

    /// <summary>
    /// Assigns the current stage the player is in.
    /// </summary>
    /// <param name="stage">The stage to associate with the player.</param>
    public void SetCurrentStage(BaseStage stage) => CurrentStage = stage;

    /// <summary>
    /// Initiates the death sequence for the player.
    /// Plays a death sound and waits for it to finish before transitioning to the menu stage.
    /// </summary>
    public async void DeathSequenz()
    {
        PlayerDeathSequenz.Play();
        await ToSignal(PlayerDeathSequenz, "finished");

        ResetAndReturnToMenu();
    }
    
    /// <summary>
    /// Restores the player's health and resets combat-related state.
    /// 
    /// This method fully heals the player, triggers any necessary
    /// cleanup on equipped weapons (such as ejecting the current magazine),
    /// and updates the damage overlay to reflect full health.
    /// </summary>
    public void ResetAndReturnToMenu()
    {
        CurrentHealth = MaxHealth;
        PlayerInventory?.CurrentGun?.Call("on_magazine_ejected");
        PlayerDamageOverlay.SetHealthPercent(CurrentHealth / MaxHealth);
        
        GameManager.Instance.ReturnToMenu();
    }
}