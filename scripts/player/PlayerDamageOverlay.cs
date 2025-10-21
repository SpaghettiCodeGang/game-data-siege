using Godot;

/// <summary>
/// Handles the visual feedback overlay for player damage.
/// 
/// This component manages a pulsing emission effect on a 3D mesh that becomes
/// visible when the player's health falls below a certain threshold.
/// The emission intensity and pulse frequency scale dynamically with the
/// player's remaining health to indicate critical damage.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class PlayerDamageOverlay : Node3D
{
    [ExportGroup("Verhalten")]
    [Export] public float TriggerThreshold = 0.5f;
    [Export] public float FadeSpeed = 2.0f;
    [Export] public float PulseSpeed = 3.0f;

    [ExportGroup("Intensität")]
    [Export] public float BaseEmission = 0.2f;
    [Export] public float PulseAmplitude = 1.0f;
    [Export] public float MaxEmission = 5.0f;
    
    private MeshInstance3D _mesh;
    private StandardMaterial3D _mat;

    private float _health = 1.0f;
    private float _pulseTime;
    private bool _isVisible;

    /// <summary>
    /// Initializes the overlay by retrieving its mesh and material references.
    /// Ensures the material supports emission and starts hidden by default.
    /// </summary>
    public override void _Ready()
    {
        _mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        _mat = _mesh.GetActiveMaterial(0) as StandardMaterial3D;

        if (_mat == null)
        {
            SetProcess(false);
            return;
        }

        _mat.EmissionEnabled = true;
        _mesh.Visible = false;
        _isVisible = false;
    }

    /// <summary>
    /// Called every frame to update the overlay’s emission and visibility state
    /// based on the current health value.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public override void _Process(double delta)
    {
        var dt = (float)delta;
        _pulseTime += dt * PulseSpeed;
        
        if (_health < TriggerThreshold)
            UpdateOverlay(dt);
        else
            FadeOutOverlay(dt);
    }

    /// <summary>
    /// Updates the emission intensity when the overlay is active.
    /// Handles the pulsing and smooth transitions of emission values.
    /// </summary>
    /// <param name="delta">Frame delta time.</param>
    private void UpdateOverlay(float delta)
    {
        if (!_isVisible)
        {
            _mesh.Visible = true;
            _isVisible = true;
        }

        var damageFactor = Mathf.InverseLerp(TriggerThreshold, 0f, _health);
        var pulse = (Mathf.Sin(_pulseTime) * 0.5f) + 0.5f;

        var targetEmission = (BaseEmission + PulseAmplitude * pulse) * Mathf.Lerp(0f, MaxEmission, damageFactor);
        _mat.EmissionEnergyMultiplier = Mathf.MoveToward(_mat.EmissionEnergyMultiplier, targetEmission, delta * FadeSpeed);
    }

    /// <summary>
    /// Gradually reduces the emission intensity and hides the mesh once the effect
    /// fades out completely.
    /// </summary>
    /// <param name="delta">Frame delta time.</param>
    private void FadeOutOverlay(float delta)
    {
        _mat.EmissionEnergyMultiplier = Mathf.MoveToward(_mat.EmissionEnergyMultiplier, 0f, delta * FadeSpeed);

        if (!(_mat.EmissionEnergyMultiplier <= 0.01f) || !_isVisible) return;
        _mesh.Visible = false;
        _isVisible = false;
    }

    /// <summary>
    /// Sets the player's current health as a normalized value (0–1).
    /// Updates the overlay intensity accordingly.
    /// </summary>
    /// <param name="health">Normalized health value (0 = dead, 1 = full health).</param>
    public void SetHealthPercent(float health)
    {
        _health = Mathf.Clamp(health, 0f, 1f);
    }
}
