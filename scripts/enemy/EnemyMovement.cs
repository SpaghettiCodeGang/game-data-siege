using Godot;

/// <summary>
/// Handles the movement logic for an enemy, including wandering and hovering.
/// This class is used internally by the <see cref="Enemy"/> instance.
/// </summary>
/// <author>Sören Lehmann</author>
public class EnemyMovement
{
    private readonly Enemy _enemy;
    private readonly Vector3 _basePos;
    private Vector3 _currentDir = Vector3.Zero;
    private Vector3 _targetDir = Vector3.Zero;
    private float _changeDirTimer;
    private readonly RandomNumberGenerator _rng;

    /// <summary>
    /// Initializes a new instance of EnemyMovement with the specified enemy.
    /// </summary>
    /// <param name="enemy">The enemy instance this combat system belongs to.</param>
    public EnemyMovement(Enemy enemy)
    {
        _enemy = enemy;
        _basePos = _enemy.GlobalPosition;
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    /// <summary>
    /// Updates the enemy's position based on wandering and hovering logic.
    /// Should be called every physics frame.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public void Hover(float delta)
    {
        var horizontalVel = HorizontalVelocity(delta);
        var verticalVel = VerticalVelocity(delta);

        _enemy.Velocity = new Vector3(horizontalVel.X, verticalVel, horizontalVel.Z);
        _enemy.MoveAndSlide();
    }

    /// <summary>
    /// Calculates the horizontal velocity of the enemy based on wandering logic and distance from the base position.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    /// <returns>The horizontal velocity vector.</returns>
    private Vector3 HorizontalVelocity(float delta)
    {
        _changeDirTimer -= delta;

        if (_changeDirTimer <= 0)
        {
            var angle = _rng.RandfRange(0, Mathf.Tau);
            _targetDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).Normalized();
            _changeDirTimer = _rng.RandfRange(2.0f, 5.0f);
        }

        _currentDir = _currentDir.Slerp(_targetDir, _enemy.TurnSpeed * delta);
        var horizontalVel = _currentDir * _enemy.WanderSpeed;

        Vector3 flatPos = new(_enemy.GlobalPosition.X, 0, _enemy.GlobalPosition.Z);
        Vector3 flatBase = new(_basePos.X, 0, _basePos.Z);
        var flatOffset = flatPos - flatBase;

        if (flatOffset.Length() > _enemy.MaxRadius)
            horizontalVel += (_basePos - _enemy.GlobalPosition).Normalized() * _enemy.WanderSpeed;

        return horizontalVel;
    }

    /// <summary>
    /// Calculates the vertical velocity of the enemy to create a hovering effect.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    /// <returns>The vertical velocity in meters per second.</returns>
    private float VerticalVelocity(float delta)
    {
        var targetY = _basePos.Y + Mathf.Sin(Time.GetTicksMsec() / 1000f) * _enemy.HoverAmplitude;
        return Mathf.Lerp(_enemy.Velocity.Y, (targetY - _enemy.GlobalPosition.Y) / delta, 0.5f);
    }
}