using Godot;

/// <summary>
/// Handles all VR controller input for the player.
/// 
/// This class is responsible for polling the left and right <see cref="XRController3D"/> devices,
/// detecting button presses, and notifying the current stage or other subsystems
/// (such as the <see cref="PlayerInventory"/>) of player actions.
/// </summary>
/// <author>Sören Lehmann</author>
public class PlayerInputHandler
{
    private readonly Player _player;
    private bool _prevAButton;
    private bool _prevBButton;
    
    public PlayerInputHandler(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// Processes controller input each frame.
    /// 
    /// Handles both hands’ primary buttons ("A" and "X") as well as the menu button
    /// on the left controller. Each input event is forwarded to the current stage
    /// or triggers specific player actions such as returning to the main menu.
    /// </summary>
    public void ProcessInput()
    {
        if (_player.LeftController == null || _player.RightController == null) return;

        // Menü-Button Left
        if (_player.LeftController.IsButtonPressed("menu_button"))
        {
            _player.PlayerInventory?.CurrentGun?.Call("on_magazine_ejected");
            GameManager.Instance.ReturnToMenu();
        }

        // X-Button Left
        var bPressed = _player.LeftController.IsButtonPressed("ax_button");
        if (bPressed && !_prevBButton)
            _player.CurrentStage?.OnPlayerButtonPressed("X");
        _prevBButton = bPressed;

        // A-Button Right
        var aPressed = _player.RightController.IsButtonPressed("ax_button");
        if (aPressed && !_prevAButton)
            _player.CurrentStage?.OnPlayerButtonPressed("A");
        _prevAButton = aPressed;
    }
}