using Godot;

/// <summary>
/// Represents the combat capabilities of an enemy.
/// Tracks health, death state, and attack cooldowns.
/// </summary>
/// <author>Sören Lehmann</author>
public class EnemyCombat
{
    private readonly Enemy _enemy;

    private readonly int _maxHealth;
    private readonly int _currentHealth;

    public EnemyCombat(Enemy enemy, int maxHealth)
    {
        _enemy = enemy;
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
    }
}