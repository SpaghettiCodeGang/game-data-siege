using Godot;

public class PlayerLaserHandler
{
    private readonly Player _player;
    public PlayerLaserHandler(Player player)
    {
        _player = player;
    }

    public void ShowAllLasers() => SetLaserState(2);
    public void HideAllLasers() => SetLaserState(0);

    private void SetLaserState(int state)
    {
        foreach (var controller in new[] { _player.LeftController, _player.RightController })
        {
            var fp = controller.GetNodeOrNull<Node>("FunctionPointer");
            fp?.Call("set_show_laser", state);
            fp?.Call("_update_pointer");
        }
    }
}