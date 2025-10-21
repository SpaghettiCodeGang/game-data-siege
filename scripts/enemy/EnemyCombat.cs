using System;
using Godot;

/// <summary>
/// Represents the combat capabilities of an enemy.
/// Tracks health, death state, and attack cooldowns.
/// Handles the combat behavior of an enemy, including projectile firing and accuracy mechanics.
/// </summary>
/// <author>Sören Lehmann</author>
/// <coauthor>Elias Kugel</coauthor>
public class EnemyCombat
{
    private readonly Enemy _enemy;
    private float _currentCooldown;
    private readonly RandomNumberGenerator _rng;
    private float _currentHealth;

    /// <summary>
    /// Initializes a new instance of EnemyCombat with the specified enemy.
    /// </summary>
    /// <param name="enemy">The enemy instance this combat system belongs to.</param>
    public EnemyCombat(Enemy enemy)
    {
        _enemy = enemy;
        _currentHealth = _enemy.MaxHealth;
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    /// <summary>
    /// Updates the combat system every physics frame.
    /// Handles attack cooldown and triggers projectile firing when aggressive.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame.</param>
    public void Update(double delta)
    {
        if (_enemy.CurrentState != Enemy.EnemyState.Aggressive) return;
        _currentCooldown -= (float)delta;

        if (!(_currentCooldown <= 0)) return;
        FireProjectile();
        _currentCooldown = _enemy.AttackCooldown;
    }

    /// <summary>
    /// Processes damage taken by the enemy and updates their health accordingly.
    /// If the enemy's health drops to zero or below, initiates the death sequence.
    /// Headshots result in instant death.
    /// </summary>
    /// <param name="damage">The amount of damage to be applied to the enemy.</param>
    public void TakeDamage(float damage, bool isHeadshot)
    {
        if (_currentHealth <= 0) return;

        if (isHeadshot) _currentHealth = 0;
        else _currentHealth -= damage;
           
        
        if (_currentHealth <= 0)
        {
            _enemy.CurrentState = Enemy.EnemyState.Passive;
            _enemy.DeathSequence();
        }
    }

    /// <summary>
    /// Calculates a randomized shot direction based on accuracy settings.
    /// Has a 70% chance to be "accurate" (small spread) and 30% chance to be "inaccurate" (larger spread).
    /// </summary>
    /// <param name="baseDirection">The base direction vector to modify.</param>
    /// <returns>A new direction vector with random spread applied.</returns>
    private Vector3 CalculateSpreadDirection(Vector3 baseDirection)
    {
        var isAccurateShot = _rng.Randf() < _enemy.AccurateShotChance;
        
        var maxSpread = isAccurateShot ? _enemy.AccurateSpreadAngle : _enemy.MaxSpreadAngle;
        
        var horizontalAngle = _rng.RandfRange(-maxSpread, maxSpread);
        var verticalAngle = _rng.RandfRange(-maxSpread, maxSpread);
        
        var horizontalRot = Transform3D.Identity.Rotated(Vector3.Up, Mathf.DegToRad(horizontalAngle));
        var verticalRot = Transform3D.Identity.Rotated(Vector3.Right, Mathf.DegToRad(verticalAngle));
        
        return (horizontalRot * verticalRot).Basis * baseDirection;
    }

    /// <summary>
    /// Creates and fires a projectile with calculated spread.
    /// Instantiates a projectile at the muzzle position and applies a randomized direction.
    /// </summary>
    private void FireProjectile()
    {
        if (_enemy.Muzzle == null || _enemy.ProjectileScene == null) return;

        var projectile = _enemy.ProjectileScene.Instantiate<Projectile>();
        _enemy.GetTree().CurrentScene.AddChild(projectile);
        
        projectile.GlobalTransform = _enemy.Muzzle.GlobalTransform;
        
        var baseDirection = -_enemy.GlobalTransform.Basis.Z;
        var spreadDirection = CalculateSpreadDirection(baseDirection);
        
        projectile.Fire(spreadDirection);
    }
}