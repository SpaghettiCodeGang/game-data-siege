using Godot;

public partial class Player : Node
{
    [Export] public Node3D RightHolster;
    [Export] public Node3D LeftMagBox;
    [Export] public PackedScene GunScene;
    [Export] public PackedScene MagazineScene;

    private Gun _currentGun;
    private Magazin _currentMagazine;

    public override void _Ready()
    {
        SpawnGun();
        SpawnMagazine();
    }

    public void SpawnGun()
    {
        if (GunScene == null || RightHolster == null) return;
        _currentGun = GunScene.Instantiate<Gun>();
        RightHolster.AddChild(_currentGun);
    }

    public void SpawnMagazine()
    {
        if (MagazineScene == null || LeftMagBox == null) return;
        _currentMagazine = MagazineScene.Instantiate<Magazin>();
        LeftMagBox.AddChild(_currentMagazine);
    }
}