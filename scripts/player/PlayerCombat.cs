using Godot;
using System;

/// <summary>
/// Handles combat-related functionality for the player character.
/// Manages health, damage taking, and combat state.
/// </summary>
/// <author>Elias Kugel</author>
public class PlayerCombat
{
    private readonly Player _player;
    private float _currentHealth;
    
    /// <summary>
    /// Initializes a new instance of PlayerCombat with the specified player.
    /// Initializes the player's health from the Player configuration.
    /// </summary>
    /// <param name="player">The player instance this combat system belongs to.</param>
    public PlayerCombat(Player player)
    {
        _player = player;
        _currentHealth = _player.MaxHealth;
    }

    /// <summary>
    /// Processes damage taken by the player and updates their health accordingly.
    /// </summary>
    /// <param name="damage">The amount of damage to be applied to the player.</param>
    /// <returns>True if the player survives the damage, false if the damage is fatal.</returns>
    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        if (_currentHealth <= 0)
        {
            GD.Print("Player is dead.");
        }
        else
        {
            GD.Print("Player hit for " + damage + " damage. Current health: " + _currentHealth);
        }
    }
}
