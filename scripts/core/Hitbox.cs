using Godot;
using System;

public partial class Hitbox : Area3D
{
    [Export] public bool IsHead = false;
    [Export] public float DamageMultiplier = 1.0f;

    [Export] public Node3D Owner;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not Projectile projectile || Owner == null) return;

        if (IsHead)
        {
            GD.Print("HeadShot auf " + Owner);
        }
        else
        {
            GD.Print("BodyShot auf " + Owner);
        }
        projectile.QueueFree();
    }
}