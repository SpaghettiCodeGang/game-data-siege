using Godot;

/// <summary>
/// Represents an enemy character in the game.
/// Handles movement, hovering, facing the player, and state transitions.
/// </summary>
/// /// <author>Sören Lehmann</author>
/// /// <coauthor>Elias Kugel</coauthor>
public partial class Enemy : CharacterBody3D
{
    [Export] public Node3D Muzzle;
    [Export] public PackedScene ProjectileScene;
    
    [ExportGroup("Movement")] 
    [Export] public float MaxRadius = 1.0f;
    [Export] public float HoverAmplitude = 0.2f;
    [Export] public float WanderSpeed = 0.5f;
    [Export] public float TurnSpeed = 1.0f;

    [ExportGroup("Combat")] 
    [Export] public int MaxHealth = 10;
    [Export] public float MaxSpreadAngle = 15.0f;
    [Export] public float AccurateShotChance = 0.7f;
    [Export] public float AccurateSpreadAngle = 5.0f;
    [Export] public float AttackCooldown = 1.0f;

    /// <summary>
    /// Current state of the enemy.
    /// </summary>
    public enum EnemyState
    {
        Passive,
        Aggressive,
        Dead
    }

    public EnemyState CurrentState = EnemyState.Passive;

    public Player Player;
    private EnemyMovement _movement;
    private EnemyCombat _combat;

    /// <summary>
    /// Initializes the enemy's movement component.
    /// </summary>
    public override void _Ready()
    {
        _movement = new EnemyMovement(this);
        _combat = new EnemyCombat(this);
    }

    /// <summary>
    /// Called every physics frame.
    /// Updates movement and behavior based on the current state.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public override void _PhysicsProcess(double delta)
    {
        switch (CurrentState)
        {
            case EnemyState.Passive:
                FacePlayer();
                _movement.Hover((float)delta);
                break;

            case EnemyState.Aggressive:
                FacePlayer();
                _movement?.Hover((float)delta);
                _combat?.Update(delta);
                break;

            case EnemyState.Dead:
                QueueFree();
                break;
        }
    }

    /// <summary>
    /// Rotates the enemy to face the player horizontally.
    /// </summary>
    private void FacePlayer()
    {
        if (Player == null) return;

        var myPos = GlobalPosition;
        var target = new Vector3(Player.GlobalPosition.X, myPos.Y, Player.GlobalPosition.Z);
        LookAt(target, Vector3.Up);
    }
}