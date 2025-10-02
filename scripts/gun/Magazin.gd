@tool
extends XRToolsPickable

func _ready():
	super._ready()
	
	# Verbinde die XR Tools Signale
	picked_up.connect(_on_picked_up)
	dropped.connect(_on_dropped)

	
func _on_picked_up(_pickable):
	pass
	
func _on_dropped(_pickable):
	freeze = false