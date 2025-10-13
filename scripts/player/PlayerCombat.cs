using Godot;

/// <summary>
/// Handles combat-related functionality specific to the player.
/// Inherits from BaseCombat to provide health management and damage handling.
/// Implements player-specific behavior such as returning to menu on death.
/// </summary>
/// <author>Elias Kugel</author>
public partial class PlayerCombat : BaseCombat
{
    public override void _Ready()
    {
        base._Ready();
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Called when the player takes damage. Override to handle player-specific death logic.
    /// </summary>
    /// <param name="isHeadshot">Whether the damage was from a headshot</param>
    public override void TakeDamage(bool isHeadshot = false)
    {
        base.TakeDamage(isHeadshot);

        // Player death
        if (CurrentHealth <= 0)
        {
            GameManager.Instance.ReturnToMenu();
        }
    }
}
