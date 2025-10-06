using Godot;
using System;

public partial class Projectile : RigidBody3D
{
    [Export] public float Speed = 30f;   // Geschwindigkeit der Kugel
    [Export] public int Damage = 1;      // Grundschaden
    [Export] public float LifeTime = 5f; // Sekunden bis Auto-Despawn

    private Vector3 _direction = Vector3.Zero;

    public override void _Ready()
    {
        // Nach Ablauf der Lebenszeit Projektil entfernen
        GetTree().CreateTimer(LifeTime).Timeout += () => QueueFree();

        // Signale verbinden (für Physik-Kollisionen)
        BodyEntered += OnBodyEntered;
    }

    /// <summary>
    /// Methode zum Abfeuern des Projektils. 
    /// Übergib die Richtung (z. B. Waffenausrichtung.Basis.Z * -1).
    /// </summary>
    public void Fire(Vector3 direction)
    {
        _direction = direction.Normalized();
        LinearVelocity = _direction * Speed;
    }

    /// <summary>
    /// Wenn Projektil einen Körper trifft (z. B. PlayerBody3D).
    /// </summary>
    private void OnBodyEntered(Node body)
    {
        QueueFree();
    }
    
}