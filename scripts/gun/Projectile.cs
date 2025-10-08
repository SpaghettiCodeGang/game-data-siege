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
	[Export] public float Speed = 80f;   // Base speed of the projectile
	[Export] public int Damage = 1;      // Base damage dealt on impact
	
	[Export] public Node3D ImpactEffect;
	
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
	/// Spawns the impact effect and removes the projectile.
	/// </summary>
	/// <param name="body">The body node that was hit by the projectile.</param>
	private void OnBodyEntered(Node body)
	{
		SpawnImpactEffect();
		QueueFree();
	}
	
	/// <summary>
	/// Detaches and activates the embedded impact effect.
	/// Moves the effect to the world scene so it can persist
	/// after the projectile is destroyed, activates all particle systems,
	/// and attaches a timer for automatic cleanup.
	/// </summary>
	private void SpawnImpactEffect()
	{
		if (ImpactEffect == null)
			return;

		// Detach the effect from the projectile so it won’t be deleted together with it
		ImpactEffect.GetParent()?.RemoveChild(ImpactEffect);
		GetTree().CurrentScene.AddChild(ImpactEffect);

		// Place the effect at the projectile’s current global position and rotation
		ImpactEffect.GlobalTransform = GlobalTransform;

		// Enable all nested GPU particle systems
		EnableAllParticles(ImpactEffect);

		// Create a timer to automatically free the effect after a short delay
		var timer = new Timer
		{
			WaitTime = 2.5f,
			OneShot = true
		};
		ImpactEffect.AddChild(timer);
		timer.Timeout += ImpactEffect.QueueFree;
		timer.Start();
	}
	

	/// <summary>
	/// Recursively searches for and enables all GPUParticles3D nodes in the hierarchy.
	/// </summary>
	private void EnableAllParticles(Node node)
	{
		foreach (var child in node.GetChildren())
		{
			if (child is GpuParticles3D particles)
				particles.Emitting = true;

			EnableAllParticles(child);
		}
	}

}
