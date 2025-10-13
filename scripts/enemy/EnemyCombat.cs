using System;
using Godot;

/// <summary>
/// Represents the combat capabilities of an enemy.
/// Tracks health, death state, and attack cooldowns.
/// Handles the combat behavior of an enemy, including projectile firing and accuracy mechanics.
/// </summary>
/// <author>Sören Lehmann</author>
/// <coauthor>Elias Kugel</coauthor>
public partial class EnemyCombat : BaseCombat
{
    private AudioStreamPlayer3D _audioPlayer;
    private bool _isDying = false;
    private Enemy _owner;

    /// <summary>
    /// Called when the node enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        // Get the Enemy owner
        _owner = GetParent<Enemy>();
        if (_owner == null)
        {
            GD.PrintErr("[EnemyCombat] Failed to find Enemy parent!");
        }
    }

    /// <summary>
    /// Called when the enemy takes damage.
    /// </summary>
    /// <param name="isHeadshot">Whether the damage was from a headshot</param>
    /// <summary>
    /// Handles damage taken by the enemy. Extends base functionality to handle death sequence.
    /// </summary>
    /// <param name="isHeadshot">If true, causes instant death regardless of damage amount</param>
    public override void TakeDamage(bool isHeadshot = false)
    {
        base.TakeDamage(isHeadshot);

        // Handle death
        if (CurrentHealth <= 0 && !_isDying)
        {
            StartDeathSequence();
        }
    }

    /// <summary>
    /// Starts the death sequence: plays random death sound and then removes the enemy.
    /// Changes enemy state to Passive while the death sound plays.
    /// </summary>
    private void StartDeathSequence()
    {
        if (_owner == null) return;

        _isDying = true;
        _owner.CurrentState = Enemy.EnemyState.Passive;

        // Create audio player if not exists
        if (_audioPlayer == null)
        {
            _audioPlayer = new AudioStreamPlayer3D();
            AddChild(_audioPlayer);
            _audioPlayer.Finished += OnDeathSoundFinished;
        }

        // Select random death sound
        int soundNumber = _rng.RandiRange(1, 4);
        var audioStream = GD.Load<AudioStream>($"res://assets/audio/enemy/enemy_{soundNumber}.mp3");
        _audioPlayer.Stream = audioStream;
        _audioPlayer.Play();
    }

    /// <summary>
    /// Called when the death sound finishes playing.
    /// Changes the enemy state to Dead and removes it from the scene.
    /// </summary>
    private void OnDeathSoundFinished()
    {
        if (_owner == null) return;

        _owner.CurrentState = Enemy.EnemyState.Dead;
        _owner.QueueFree();
    }

    private readonly Enemy _enemy;
    private readonly int _maxHealth;
    private int _currentHealth;
    private Node3D _muzzle;
    private float _attackCooldown = 1.0f;
    private float _currentCooldown = 0.0f;
    private PackedScene _projectileScene;
    private readonly RandomNumberGenerator _rng;
    
    // Accuracy settings
    private readonly float _maxSpreadAngle;     // Maximum deviation angle in degrees
    private readonly float _accurateShotChance; // Chance for accurate shots
    private readonly float _accurateSpreadAngle; // Small spread for "accurate" shots

    /// <summary>
    /// Initializes a new instance of EnemyCombat with the specified enemy, health values, and accuracy settings.
    /// Sets up projectile scene, muzzle reference, and initializes the random number generator.
    /// </summary>
    /// <param name="enemy">The enemy instance this combat system belongs to.</param>
    /// <param name="maxHealth">The maximum health points for this enemy.</param>
    /// <param name="maxSpreadAngle">Maximum spread angle for inaccurate shots in degrees.</param>
    /// <param name="accurateShotChance">Probability (0-1) of firing an accurate shot.</param>
    /// <param name="accurateSpreadAngle">Spread angle for accurate shots in degrees.</param>
    public EnemyCombat(Enemy enemy, int maxHealth, float maxSpreadAngle = 15.0f, 
                      float accurateShotChance = 0.7f, float accurateSpreadAngle = 5.0f)
    {
        _enemy = enemy;
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
        _maxSpreadAngle = maxSpreadAngle;
        _accurateShotChance = accurateShotChance;
        _accurateSpreadAngle = accurateSpreadAngle;
        
        _rng = new RandomNumberGenerator();
        _rng.Randomize();

        // Load projectile scene
        _projectileScene = GD.Load<PackedScene>("res://scenes/gun/Projectile.tscn");
        
        // Get muzzle reference
        _muzzle = _enemy.GetNode<Node3D>("L Arm Turn/Muzzle");
        if (_muzzle == null)
        {
            GD.PrintErr("Muzzle node not found at 'L Arm Turn/Muzzle'!");
        }
    }

    /// <summary>
    /// Updates the combat system every physics frame.
    /// Handles attack cooldown and triggers projectile firing when aggressive.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame.</param>
    public void Update(double delta)
    {
        if (_enemy.CurrentState == Enemy.EnemyState.Aggressive)
        {
            _currentCooldown -= (float)delta;
            if (_currentCooldown <= 0)
            {
                FireProjectile();
                _currentCooldown = _attackCooldown;
            }
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
        // Determine if this will be an "accurate" shot
        bool isAccurateShot = _rng.Randf() < _accurateShotChance;
        
        // Choose spread angle based on accuracy
        float maxSpread = isAccurateShot ? _accurateSpreadAngle : _maxSpreadAngle;
        
        // Calculate random angles for both horizontal and vertical spread
        float horizontalAngle = _rng.RandfRange(-maxSpread, maxSpread);
        float verticalAngle = _rng.RandfRange(-maxSpread, maxSpread);
        
        // Create rotation transforms
        var horizontalRot = Transform3D.Identity.Rotated(Vector3.Up, Mathf.DegToRad(horizontalAngle));
        var verticalRot = Transform3D.Identity.Rotated(Vector3.Right, Mathf.DegToRad(verticalAngle));
        
        // Apply rotations to the base direction
        return (horizontalRot * verticalRot).Basis * baseDirection;
    }

    /// <summary>
    /// Creates and fires a projectile with calculated spread.
    /// Instantiates a projectile at the muzzle position and applies a randomized direction.
    /// </summary>
    private void FireProjectile()
    {
        if (_muzzle == null || _projectileScene == null) return;

        var projectile = _projectileScene.Instantiate<Node3D>();
        _enemy.GetTree().CurrentScene.AddChild(projectile);
        
        // Set initial position and rotation
        projectile.GlobalTransform = _muzzle.GlobalTransform;
        
        // Get base direction and apply spread
        var baseDirection = -_enemy.GlobalTransform.Basis.Z;
        var spreadDirection = CalculateSpreadDirection(baseDirection);
        
        // Fire the projectile with the calculated spread direction
        projectile.Call("Fire", spreadDirection);
    }
}