using Godot;
using System;

/// <summary>
/// Represents a hitbox area that can detect and process projectile hits.
/// Handles damage application to both enemies and players.
/// </summary>
/// <author>Elias Kugel</author>
public partial class Hitbox : Area3D
{
    [Export] public bool IsHead = false;
    [Export] public Node3D HitboxOwner;

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Sets up the body entered event handler.
    /// </summary>
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    /// <summary>
    /// Handles collision events when a body enters the hitbox.
    /// Processes projectile hits and applies damage to the appropriate entity.
    /// </summary>
    /// <param name="body">The body that entered the hitbox.</param>
    private void OnBodyEntered(Node3D body)
    {
        if (body is not Projectile projectile || Owner == null) return;

        if (Owner is Enemy enemy) enemy.EnemyCombat.TakeDamage(projectile.Damage, IsHead);
        if (Owner is Player player) player.PlayerCombat.TakeDamage(projectile.Damage);

        projectile.QueueFree();
    }
    
}