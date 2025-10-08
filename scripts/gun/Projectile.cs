using Godot;
using System;

/// <summary>
/// Represents a physical projectile in the game world.
/// The projectile moves in a specified direction at a given speed,
/// deals damage on impact, and despawns automatically after a set lifetime.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class Projectile : RigidBody3D
{
    [Export] public float Speed = 30f;   // Base speed of the projectile
    [Export] public int Damage = 1;      // Base damage dealt on impact

    private Vector3 _direction = Vector3.Zero;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Cconnects physics collision signals.
    /// </summary>
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    /// <summary>
    /// Fires the projectile in the given direction.
    /// Usually called after instantiating the projectile.
    /// </summary>
    /// <param name="direction">The direction in which to fire the projectile.</param>
    public void Fire(Vector3 direction)
    {
        _direction = direction.Normalized();
        LinearVelocity = _direction * Speed;
    }

    /// <summary>
    /// Handles collision events when the projectile hits another body.
    /// Removes the projectile upon impact.
    /// </summary>
    /// <param name="body">The body node that was hit by the projectile.</param>
    private void OnBodyEntered(Node body)
    {
        QueueFree();
    }
    
}