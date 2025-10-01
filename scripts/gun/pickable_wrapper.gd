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

func _on_picked_up(pickable):
	weapon_picked_up.emit(pickable)

func _on_dropped(pickable):
	weapon_dropped.emit()