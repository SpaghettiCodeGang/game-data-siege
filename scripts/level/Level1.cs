using Godot;
using System.Collections.Generic;

public partial class Level1 : BaseStage
{
    [Export] public PackedScene EnemyScene;
    [Export] public Marker3D EnemyPositionMarker1;
    [Export] public Marker3D EnemyPositionMarker2;
    [Export] public Marker3D EnemyPositionMarker3;

    public override void OnEnter()
    {
        if (Player == null) return;
    }

}
