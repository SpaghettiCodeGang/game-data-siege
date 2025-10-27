using Godot;
using System;

/// <summary>
/// Handles combat-related functionality for the player character.
/// Manages health, damage taking, and combat state.
/// </summary>
/// <author>Sören Lehmann</author>
/// <coauthor>Elias Kugel</coauthor>
public class PlayerCombat
{
    private readonly Player _player;
    
    /// <summary>
    /// Initializes a new instance of PlayerCombat with the specified player.
    /// </summary>
    /// <param name="player">The player instance this combat system belongs to.</param>
    public PlayerCombat(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// Processes damage taken by the player and updates their health accordingly.
    /// </summary>
    /// <param name="damage">The amount of damage to be applied to the player.</param>
    /// <returns>True if the player survives the damage, false if the damage is fatal.</returns>
    public void TakeDamage(float damage)
    {
        _player.CurrentHealth = Mathf.Max(0f, _player.CurrentHealth - damage);
        
        if (_player.CurrentHealth <= 0)
        {
            _player.DeathSequence();
        }
        else
        {
            _player.PlayerHitSequence.Play();
            _player.PlayerDamageOverlay.SetHealthPercent(_player.CurrentHealth / _player.MaxHealth);
        }
    }
    
    /// <summary>
    /// Heals player by 1 point.
    ///
    /// This method is called when the player kills an enemy and gives him one HP, if he is below max health.
    /// </summary>
    public void Heal()
    {
        if (_player.CurrentHealth < _player.MaxHealth)
        {
            _player.CurrentHealth += 1;
            _player.PlayerDamageOverlay.SetHealthPercent(_player.CurrentHealth / _player.MaxHealth);
        }
    }
}
