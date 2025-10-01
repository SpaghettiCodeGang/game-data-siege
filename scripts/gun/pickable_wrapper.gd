@tool
extends XRToolsPickable
class_name PickableWrapper

# Signal für C# Kommunikation
signal weapon_picked_up(by_controller: XRController3D)
signal weapon_dropped()

func _ready():
	super._ready()
	
	# Verbinde die XR Tools Signale
	picked_up.connect(_on_picked_up)
	dropped.connect(_on_dropped)

func _on_picked_up(_pickable):
	freeze = false
	weapon_picked_up.emit(_pickable)

func _on_dropped(_pickable):
	freeze = false
	weapon_dropped.emit()
