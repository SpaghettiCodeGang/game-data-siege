using Godot;

public partial class Gun : Node3D
{
    private Node _pickableNode;
    private bool _isHeld = false;
    
    public override void _Ready()
    {
        // Finde den Pickable-Node (muss als Child existieren)
        _pickableNode = GetNode("PickableWrapper");
        
        // Verbinde die Signale
        _pickableNode.Connect("weapon_picked_up", 
            Callable.From((Variant controller) => OnPickedUp(controller)));
        _pickableNode.Connect("weapon_dropped", 
            Callable.From(() => OnDropped()));
    }
    
    private void OnPickedUp(Variant controller)
    {
        _isHeld = true;
        GD.Print("Waffe wurde aufgehoben!");
        // Hier deine Logik
    }
    
    private void OnDropped()
    {
        _isHeld = false;
        GD.Print("Waffe wurde fallen gelassen!");
        // Hier deine Logik
    }
}