using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Manages the actual level of the game, handling enemy spawning and management.
/// Controls enemy behavior transitions, spawn timing, and difficulty progression.
/// </summary>
/// <author>Elias Kugel</author>
public partial class Level1 : BaseStage
{
    [Export] public PackedScene EnemyScene;
    [Export] public Node3D SpawnArea;
    [Export] public float RespawnDelay = 5.0f;
    [Export] public float AggressiveDelay = 3.0f;
    [Export] public int MaxEnemies = 5;
    private int _currentEnemyCount;
    private int _killedEnemiesCounter;
    private double _respawnTimer;
    private int _damageIncrease;
    private readonly Dictionary<Enemy, double> _enemyAggressiveTimers = new();

    public override void OnEnter()
    {
        if (Player == null) return;
        Player.PlayerInventory.SpawnGun();
        Player.PlayerInventory.SpawnMagazine();
        _killedEnemiesCounter = 0;
        _respawnTimer = RespawnDelay;
    }

    /// <summary>
    /// Handles the continuous game loop processing for Level 1.
    /// Manages enemy spawning, timing, and state transitions from passive to aggressive.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame in seconds.</param>
    public override void _Process(double delta)
    {        
        if (_currentEnemyCount < MaxEnemies)
        {
            _respawnTimer -= delta;
            if (_respawnTimer <= 0)
            {
                Vector3 spawnPosition = GetRandomPositionInSpawnArea();
                Enemy enemy = SpawnEnemy(spawnPosition);
                
                if (enemy != null)
                {
                    enemy.TreeExiting += OnEnemyDied;
                    enemy.CurrentState = Enemy.EnemyState.Passive; 
                    _enemyAggressiveTimers[enemy] = AggressiveDelay;
                }
                
                _respawnTimer = RespawnDelay;
            }
        }

        var enemiesToUpdate = new List<Enemy>(_enemyAggressiveTimers.Keys);
        foreach (var enemy in enemiesToUpdate)
        {
            _enemyAggressiveTimers[enemy] -= delta;
            if (_enemyAggressiveTimers[enemy] <= 0)
            {
                enemy.CurrentState = Enemy.EnemyState.Aggressive;
                _enemyAggressiveTimers.Remove(enemy);
            }
        }
    }

    /// <summary>
    /// Handles the event when an enemy dies.
    /// Updates enemy count and resets respawn timer if below maximum enemy limit.
    /// </summary>
    private void OnEnemyDied()
    {
        _currentEnemyCount--;
        if (_currentEnemyCount < MaxEnemies)
        {
            _respawnTimer = RespawnDelay;
        }
    }

    /// <summary>
    /// Calculates a random spawn position within the designated spawn area.
    /// Takes into account the spawn area's bounds and applies reasonable limits to spawn dimensions.
    /// </summary>
    /// <returns>A Vector3 representing a valid spawn position within the spawn area.</returns>
    private Vector3 GetRandomPositionInSpawnArea()
    {
        if (SpawnArea == null) return Vector3.Zero;

        var spawnTransform = SpawnArea.GlobalTransform;
        var spawnScale = spawnTransform.Basis.Scale;
        var spawnPosition = spawnTransform.Origin;
        
        var random = new RandomNumberGenerator();
        random.Randomize();
        
        float maxWidth = 10.0f;
        float maxDepth = 10.0f;
        
        float actualWidth = Mathf.Min(maxWidth, spawnScale.X);
        float actualDepth = Mathf.Min(maxDepth, spawnScale.Z);
        
        float halfWidth = actualWidth / 2.0f;
        float halfDepth = actualDepth / 2.0f;
        
        float x = random.RandfRange(spawnPosition.X - halfWidth, spawnPosition.X + halfWidth);
        float z = random.RandfRange(spawnPosition.Z - halfDepth, spawnPosition.Z + halfDepth);
        
        float y = spawnPosition.Y + 1.0f;
        
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Spawns a new enemy at the specified position.
    /// Initializes enemy properties, updates counters, and applies damage scaling based on killed enemies.
    /// </summary>
    /// <param name="position">The position where the enemy should be spawned.</param>
    /// <returns>The spawned Enemy instance, or null if EnemyScene is not set.</returns>
    private Enemy SpawnEnemy(Vector3 position)
    {
        if (EnemyScene == null) return null;
        var enemy = EnemyScene.Instantiate<Enemy>();
        var transform = new Transform3D(Basis.Identity, position);
        enemy.GlobalTransform = transform;
        enemy.Player = Player;
        AddChild(enemy);
        if (_killedEnemiesCounter >= 5)
        {
            _damageIncrease++;
            _killedEnemiesCounter = 0;
        }        
        enemy.EnemyCombat.DamageAddition += _damageIncrease;
        _currentEnemyCount++;
        _killedEnemiesCounter++;
        return enemy;
    }

    /// <summary>
    /// Handles cleanup when exiting the level.
    /// Removes the player's gun and magazine from their inventory.
    /// </summary>
    public override void OnExit()
    {
        if (Player == null) return;
        Player.PlayerInventory.RemoveGun();
        Player.PlayerInventory.RemoveMagazine();
    }
}
