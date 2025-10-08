using Godot;

/// <summary>
/// Represents a synchronization system for player-attached objects (e.g., hitbox, holsters, or magazine boxes)
/// that dynamically adjusts their height and position based on the XRToolsPlayerBody collider.
/// 
/// The class continuously monitors the player's capsule collider height and propagates changes to
/// attached components such as hitboxes and holsters, ensuring that they stay aligned with the player's
/// current posture (e.g., standing, crouching).
/// </summary>
/// <author>Sören Lehmann</author>
public partial class PlayerHeightAttachmentSync : Node
{
    [ExportGroup("Player References")]
    [Export] public CharacterBody3D PlayerBody;
    [Export] public CollisionShape3D HitboxCollisionShape;

    [ExportGroup("Attachments")]
    [Export] public Area3D RightHolster;
    [Export] public float RightHolsterHeightRatio = 0.6f;
    [Export] public Area3D LeftMagBox;
    [Export] public float LeftMagBoxHeightRatio = 0.6f;

    
    private CollisionShape3D _playerBodyCollisionShape;
    private float _lastHeight;

    /// <summary>
    /// Called every physics frame.
    /// Ensures all required references are set, then synchronizes the hitbox and attached objects.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public override void _PhysicsProcess(double delta)
    {
        if (PlayerBody == null) return;
        if (HitboxCollisionShape == null) return;
        if (!EnsurePlayerBodyShape()) return;
        
        UpdateHitboxHeight();
        UpdateAttachments();
    }
    
    /// <summary>
    /// Searches for and caches the <see cref="CollisionShape3D"/> component
    /// within the player body if it has not been assigned yet.
    /// </summary>
    /// <returns>
    /// True if a valid <see cref="CollisionShape3D"/> was found; otherwise, false.
    /// </returns>
    private bool EnsurePlayerBodyShape()
    {
        if (_playerBodyCollisionShape != null)
            return true;

        foreach (var child in PlayerBody.GetChildren())
        {
            if (child is not CollisionShape3D shape) continue;
            _playerBodyCollisionShape = shape;
            return true;
        }
        return false;
    }
    
    
    /// <summary>
    /// Updates the hitbox collider to match the current dimensions and transform
    /// of the player's capsule collider.
    /// </summary>
    private void UpdateHitboxHeight()
    {
        if (_playerBodyCollisionShape.Shape is not CapsuleShape3D src ||
            HitboxCollisionShape.Shape is not CapsuleShape3D dst) return;
        dst.Height = src.Height;
        dst.Radius = src.Radius;
        HitboxCollisionShape.Transform = _playerBodyCollisionShape.Transform;
    }

    /// <summary>
    /// Adjusts the position of attached objects such as holsters or magazine boxes
    /// based on the player’s current body height.
    /// 
    /// Each object’s vertical offset is calculated using its configured height ratio,
    /// ensuring it stays in the correct relative position as the player moves or crouches.
    /// </summary>
    private void UpdateAttachments()
    {
        if (_playerBodyCollisionShape.Shape is not CapsuleShape3D src) return;
        
        var bodyHeight = src.Height;

        if (!(Mathf.Abs(bodyHeight - _lastHeight) > 0.02f)) return;
        _lastHeight = bodyHeight;
        
        var baseY = PlayerBody.GlobalPosition.Y;
        
        var holsterY = baseY + bodyHeight * RightHolsterHeightRatio;
        var magBoxY = baseY + bodyHeight * LeftMagBoxHeightRatio;

        if (RightHolster != null)
        {
            var pos = RightHolster.GlobalPosition;
            pos.Y = holsterY;
            RightHolster.GlobalPosition = pos;
        }

        if (LeftMagBox != null)
        {
            var pos = LeftMagBox.GlobalPosition;
            pos.Y = magBoxY;
            LeftMagBox.GlobalPosition = pos;
        }
    }
}
