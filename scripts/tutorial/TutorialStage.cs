using Godot;

namespace gamedatasiege.scripts.tutorial;

/// <summary>
/// Stage for the tutorial sequence.
/// Spawns basic equipment (gun and magazine) on enter
/// and cleans them up on exit.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class TutorialStage : core.BaseStage
{
    public override void OnEnter()
    {
        Player.SpawnGun();
        Player.SpawnMagazine();
        
        // TODO: Add whiteboard, instructions, etc.
    }

    public override void OnExit()
    {
        Player.RemoveGun();
        Player.RemoveMagazine();
    }
}