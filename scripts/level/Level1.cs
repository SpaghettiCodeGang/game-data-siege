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
    [Export] public Node3D SpawnArea; // Der Bereich, in dem Gegner spawnen sollen
    [Export] public float RespawnDelay = 5.0f; // Sekunden
    [Export] public float AggressiveDelay = 3.0f; // Sekunden bis der Gegner aggressiv wird
    [Export] public int MaxEnemies = 5;
    private int currentEnemyCount = 0;
    private int killedEnemiesCounter = 0;
    private const float ClearRadius = 1.0f; // Meter
    private double respawnTimer = 0.0;
    public int _damageIncrease = 0;
    private Dictionary<Enemy, double> enemyAggressiveTimers = new();

    public override void OnEnter()
    {
        if (Player == null) return;
        Player.PlayerInventory.SpawnGun();
        Player.PlayerInventory.SpawnMagazine();
        killedEnemiesCounter = 0; // Counter zurücksetzen
        respawnTimer = RespawnDelay; // Timer starten
    }

    /// <summary>
    /// Handles the continuous game loop processing for Level 1.
    /// Manages enemy spawning, timing, and state transitions from passive to aggressive.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame in seconds.</param>
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Spawn-Timer verarbeiten
        if (currentEnemyCount < MaxEnemies)
        {
            respawnTimer -= delta;
            if (respawnTimer <= 0)
            {
                Vector3 spawnPosition = GetRandomPositionInSpawnArea();
                Enemy enemy = SpawnEnemy(spawnPosition);
                
                if (enemy != null)
                {
                    enemy.TreeExiting += OnEnemyDied;
                    enemy.CurrentState = Enemy.EnemyState.Passive; 
                    enemyAggressiveTimers[enemy] = AggressiveDelay;
                }
                
                respawnTimer = RespawnDelay;
            }
        }

        var enemiesToUpdate = new List<Enemy>(enemyAggressiveTimers.Keys);
        foreach (var enemy in enemiesToUpdate)
        {
            if (enemy == null || !IsInstanceValid(enemy)) 
            {
                enemyAggressiveTimers.Remove(enemy);
                continue;
            }

            enemyAggressiveTimers[enemy] -= delta;
            if (enemyAggressiveTimers[enemy] <= 0)
            {
                enemy.CurrentState = Enemy.EnemyState.Aggressive;
                enemyAggressiveTimers.Remove(enemy);
            }
        }
    }

    /// <summary>
    /// Handles the event when an enemy dies.
    /// Updates enemy count and resets respawn timer if below maximum enemy limit.
    /// </summary>
    private void OnEnemyDied()
    {
        currentEnemyCount--;
        if (currentEnemyCount < MaxEnemies)
        {
            respawnTimer = RespawnDelay; // Starte Timer neu, wenn ein Gegner stirbt
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

        // Get the spawn area's bounds using its transform
        var spawnTransform = SpawnArea.GlobalTransform;
        var spawnScale = spawnTransform.Basis.Scale;
        var spawnPosition = spawnTransform.Origin;
        
        // Create a random number generator
        var random = new RandomNumberGenerator();
        random.Randomize();
        
        // Begrenzt die Spawn-Fläche auf einen vernünftigen Bereich
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
        if (killedEnemiesCounter >= 5)
        {
            _damageIncrease++;
            killedEnemiesCounter = 0;
        }        
        enemy.EnemyCombat._damageAddition += _damageIncrease;
        currentEnemyCount++;
        killedEnemiesCounter++;
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
