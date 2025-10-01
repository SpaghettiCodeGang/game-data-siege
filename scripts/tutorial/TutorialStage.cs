using Godot;

/// <summary>
/// Stage for the tutorial sequence.
/// Spawns basic equipment (gun and magazine) on enter
/// and cleans them up on exit.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class TutorialStage : BaseStage
{
    /// <summary>
    /// Spawn point for the enemy in this stage.
    /// </summary>
    [Export] protected Marker3D EnemyPositionMarker;
    
    public override void OnEnter()
    {
        if (Player == null) return;
        Player.SpawnGun();
        Player.SpawnMagazine();
        SpawnEnemy(EnemyPositionMarker);

        // TODO: Add whiteboard, instructions, etc.
    }

    public override void OnExit()
    {
        if (Player == null) return;
        Player.RemoveGun();
        Player.RemoveMagazine();
    }
}