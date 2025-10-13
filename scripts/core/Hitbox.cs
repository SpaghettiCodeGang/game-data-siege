using Godot;
using System;

/// <summary>
/// Represents a hitbox for combat interactions.
/// Detects projectile collisions and forwards damage to the associated combat component.
/// Supports different hit zones (head/body) and damage multipliers.
/// </summary>
/// <author>Sören Lehmann</author>
/// <coauthor>Elias Kugel</coauthor>
public partial class Hitbox : Area3D
{
    /// <summary>
    /// Whether this hitbox represents a head hit zone.
    /// </summary>
    [Export] public bool IsHead = false;

    /// <summary>
    /// Damage multiplier applied to hits in this zone.
    /// </summary>
    [Export] public float DamageMultiplier = 1.0f;

    /// <summary>
    /// The entity that owns this hitbox.
    /// </summary>
    [Export] public Node3D HitboxOwner;

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Automatically assigns the hitbox owner by searching up the node hierarchy
    /// for a Player or Enemy node if not manually set.
    /// </summary>
    public override void _Ready()
    {
        // Auto-assign hitbox owner if not set
        if (HitboxOwner == null)
        {
            // Get the parent that's either a Player or Enemy
            var current = GetParent();
            while (current != null)
            {
                if (current is Player or Enemy)
                {
                    HitboxOwner = current as Node3D;
                    break;
                }
                current = current.GetParent();
            }
        }

        BodyEntered += OnBodyEntered;
    }

    /// <summary>
    /// Called when a body enters this hitbox's area.
    /// Handles projectile hits and damage calculation.
    /// </summary>
    /// <param name="body">The body that entered the hitbox area.</param>
    private void OnBodyEntered(Node3D body)
    {
        GD.Print($"[Hitbox] Hit detected - Body type: {body?.GetType().Name ?? "null"}");
        
        if (body is not Projectile projectile)
        {
            GD.Print("[Hitbox] Not a projectile");
            return;
        }
        
        if (HitboxOwner == null)
        {
            GD.Print("[Hitbox] No hitbox owner assigned");
            return;
        }

        GD.Print($"[Hitbox] Looking for Combat component on {HitboxOwner.Name}");
        var combat = HitboxOwner.GetNodeOrNull<BaseCombat>("Combat");
        
        if (combat == null)
        {
            GD.Print($"[Hitbox] No Combat component found on {HitboxOwner.Name}");
            // Print parent hierarchy to debug node path
            Node current = HitboxOwner;
            while (current != null)
            {
                GD.Print($"[Hitbox] Node hierarchy: {current.Name} ({current.GetType().Name})");
                current = current.GetParent();
            }
            return;
        }

        GD.Print($"[Hitbox] Valid hit on {HitboxOwner.Name} - IsHead: {IsHead}");
        combat.TakeDamage(IsHead);
        projectile.QueueFree();
    }
}