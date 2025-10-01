using Godot;

public partial class Gun : Node3D
{
    private Node _pickableNode;
    private bool _isHeld = false;
    
    public override void _Ready()
    {
        // Finde den PickableWrapper (muss als Child existieren)
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
        //TODO: Waffenlogik
    }
    
    private void OnDropped()
    {
        _isHeld = false;
        //TODO: Waffenlogik
    }
}