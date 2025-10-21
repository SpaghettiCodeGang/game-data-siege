using Godot;

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

    [ExportGroup("Intensität")]
    [Export] public float BaseEmission = 0.2f;       // Minimalwert bei Puls
    [Export] public float PulseAmplitude = 1.0f;     // Wie stark der Puls schwankt
    [Export] public float MaxEmission = 5.0f;        // Oberes Limit bei 0 HP

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

    public override void _Process(double delta)
    {
        var dt = (float)delta;
        _pulseTime += dt * PulseSpeed;
        
        if (_health < TriggerThreshold)
            UpdateOverlay(dt);
        else
            FadeOutOverlay(dt);
    }

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

    private void FadeOutOverlay(float delta)
    {
        _mat.EmissionEnergyMultiplier = Mathf.MoveToward(_mat.EmissionEnergyMultiplier, 0f, delta * FadeSpeed);

        if (!(_mat.EmissionEnergyMultiplier <= 0.01f) || !_isVisible) return;
        _mesh.Visible = false;
        _isVisible = false;
    }

    public void SetHealthPercent(float health)
    {
        _health = Mathf.Clamp(health, 0f, 1f);
    }
}
