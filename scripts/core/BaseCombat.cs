using Godot;

/// <summary>
/// Base class for combat functionality, providing health management for both player and enemy entities.
/// Manages health points, damage calculation, and provides the foundation for entity-specific combat behavior.
/// </summary>
/// <author>Elias Kugel</author>
public abstract partial class BaseCombat : Node
{
    /// <summary>
    /// Maximum health points the entity can have.
    /// </summary>
    [Export]
    public int MaxHealth = 10;

    /// <summary>
    /// Amount of damage dealt by body shots.
    /// </summary>
    [Export]
    public int BodyDamage = 2;

    /// <summary>
    /// Current health points of the entity.
    /// </summary>
    protected int CurrentHealth;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Called when the entity takes damage.
    /// </summary>
    /// <param name="isHeadshot">Whether the damage was from a headshot</param>
    /// <summary>
    /// Handles damage taken by the entity. Can be overridden by derived classes
    /// to implement specific damage behavior.
    /// </summary>
    /// <param name="isHeadshot">If true, causes instant death regardless of damage amount</param>
    public virtual void TakeDamage(bool isHeadshot = false)
    {
        if (isHeadshot)
        {
            CurrentHealth = 0;
        }
        else
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - BodyDamage);
        }
        
        EmitSignal(SignalName.HealthChanged, CurrentHealth);
    }

    [Signal]
    public delegate void HealthChangedEventHandler(int newHealth);
}
