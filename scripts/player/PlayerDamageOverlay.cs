using Godot;

public partial class PlayerDamageOverlay : Node3D
{
    private MeshInstance3D _mesh;
    private StandardMaterial3D _mat;

    private float _health = 1.0f;
    private float _pulseTime = 0.0f;

    [ExportGroup("Verhalten")]
    [Export] public float TriggerThreshold = 0.5f;   // Unter diesem Wert sichtbar
    [Export] public float FadeSpeed = 2.0f;          // Wie schnell Emission reagiert
    [Export] public float PulseSpeed = 3.0f;         // Geschwindigkeit der Pulsierung

    [ExportGroup("Intensität")]
    [Export] public float BaseEmission = 0.2f;       // Minimalwert bei Puls
    [Export] public float PulseAmplitude = 1.0f;     // Wie stark der Puls schwankt
    [Export] public float MaxEmission = 5.0f;        // Oberes Limit bei 0 HP

    public override void _Ready()
    {
        _mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        _mat = _mesh.GetActiveMaterial(0) as StandardMaterial3D;
        _mesh.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (_mat == null)
            return;

        _pulseTime += (float)delta * PulseSpeed;

        if (_health >= TriggerThreshold)
        {
            HideOverlay((float)delta);
            return;
        }

        ShowOverlay((float)delta);
    }

    private void ShowOverlay(float delta)
    {
        _mesh.Visible = true;

        // Normierte Health: 0 → 1 in Abhängigkeit vom Triggerbereich
        float damageFactor = Mathf.InverseLerp(TriggerThreshold, 0f, _health);
        float pulse = Mathf.Sin(_pulseTime) * 0.5f + 0.5f; // 0–1 Bereich

        // Basis + Puls + Health-Skalierung
        float targetEmission = (BaseEmission + PulseAmplitude * pulse) * Mathf.Lerp(0f, MaxEmission, damageFactor);

        // Smooth Übergang
        _mat.EmissionEnergyMultiplier = Mathf.MoveToward(_mat.EmissionEnergyMultiplier, targetEmission, delta * FadeSpeed);
    }

    private void HideOverlay(float delta)
    {
        _mesh.Visible = false;
        _mat.EmissionEnergyMultiplier = Mathf.MoveToward(_mat.EmissionEnergyMultiplier, 0f, delta * FadeSpeed);
    }

    public void SetHealthPercent(float health)
    {
        _health = Mathf.Clamp(health, 0f, 1f);
    }
}
