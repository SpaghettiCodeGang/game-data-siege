using Godot;

/// <summary>
/// Represents an enemy character in the game.
/// Handles movement, hovering, facing the player, and state transitions.
/// </summary>
/// /// <author>Sören Lehmann</author>
public partial class Enemy : CharacterBody3D
{
    [Export] public float MaxRadius = 1.0f;
    [Export] public float HoverAmplitude = 0.2f;
    [Export] public float WanderSpeed = 0.5f;
    [Export] public float TurnSpeed = 1.0f;
    
    /// <summary>
    /// Current state of the enemy.
    /// </summary>
    public enum EnemyState { Passive, Aggressive, Dead }
    public EnemyState CurrentState = EnemyState.Passive;

    private Player _player;
    private EnemyMovement _movement;
    private EnemyCombat _combat;

    /// <summary>
    /// Initializes the enemy's movement component.
    /// </summary>
    public override void _Ready()
    {
        _movement = new EnemyMovement(this, MaxRadius, HoverAmplitude, WanderSpeed, TurnSpeed);
    }
    
    /// <summary>
    /// Initializes the enemy with a reference to the player and maximum health.
    /// Must be called after instantiation, typically by the stage or spawner.
    /// </summary>
    /// <param name="player">The active player instance.</param>
    /// <param name="maxHealth">The maximum health to assign to this enemy.</param>
    public void Initialize(Player player, int maxHealth)
    {
        _player = player;
        _combat = new EnemyCombat(this, maxHealth);
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
                _movement.Hover((float)delta);
                FacePlayer();
                break;

            case EnemyState.Aggressive:
                _movement.Hover((float)delta);
                FacePlayer();
                // _combat.Attack(_player);
                break;

            case EnemyState.Dead:
                QueueFree();
                break;
        }
    }
    
    /// <summary>
    /// Sets the current state of the enemy.
    /// </summary>
    /// <param name="newState">The new state to assign.</param>
    public void SetState(EnemyState newState) => CurrentState = newState;

    /// <summary>
    /// Rotates the enemy to face the player horizontally.
    /// </summary>
    private void FacePlayer()
    {
        if (_player == null) return;
        
        var myPos = GlobalPosition;
        var target = new Vector3(_player.GlobalPosition.X, myPos.Y, _player.GlobalPosition.Z);
        LookAt(target, Vector3.Up);
    }
}