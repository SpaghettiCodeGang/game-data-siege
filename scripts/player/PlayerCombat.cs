using Godot;
using System;

/// <summary>
/// Handles combat-related functionality for the player character.
/// Manages health, damage taking, and combat state.
/// </summary>
/// <author>Elias Kugel</author>
public partial class PlayerCombat : Node
{
    [Export] public Player Player { get; set; }
    private float _currentHealth;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes the player's health from the Player configuration.
    /// </summary>
    public override void _Ready()
    {
        _currentHealth = Player.MaxHealth;
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
