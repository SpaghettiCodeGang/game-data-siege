@tool
extends XRToolsPickable

signal magazine_picked_up()

func _ready():
	super._ready()
	
	# Verbinde die XR Tools Signale
	picked_up.connect(_on_magazine_picked_up)
	dropped.connect(_on_magazine_dropped)

	
func _on_magazine_picked_up(_pickable):
	magazine_picked_up.emit()
	
func _on_magazine_dropped(_pickable):
	freeze = false