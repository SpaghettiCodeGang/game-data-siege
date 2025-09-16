using Godot;
using System;

public partial class Player : Node
{
    [Export] public Node3D RightHolster;
    [Export] public Node3D LeftMagBox;
    [Export] public PackedScene GunScene;
    [Export] public PackedScene MagazineScene;

    private Gun currentGun;
    private Magazin currentMagazine;

    public override void _Ready()
    {
        SpawnGun();
        SpawnMagazine();
    }

    public void SpawnGun()
    {
        currentGun = GunScene.Instantiate<Gun>();
        RightHolster.AddChild(currentGun);
    }

    public void SpawnMagazine()
    {
        currentMagazine = MagazineScene.Instantiate<Magazin>();
        RightHolster.AddChild(currentMagazine);
    }
}

