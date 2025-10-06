@tool
extends XRToolsPickable

signal magazine_picked_up()
signal magazine_despawned()

@export var capacity: int = 8

var remaining_rounds: int
var is_loaded_in_gun: bool = false

func _ready():
	super._ready()
	remaining_rounds = capacity;
	
	# Verbinde die XR Tools Signale
	picked_up.connect(_on_magazine_picked_up)
	dropped.connect(_on_magazine_dropped)
	
func _on_magazine_picked_up(_pickable):
	if $LifetimeTimer.is_stopped() == false:
		$LifetimeTimer.stop()
	magazine_picked_up.emit()
	
func _on_magazine_dropped(_pickable):
	freeze = false
	
	await get_tree().process_frame
	if not is_loaded_in_gun:
		$LifetimeTimer.start()
	
func _on_lifetime_timer_timeout():
	magazine_despawned.emit()
	queue_free()
	
func consume_round() -> bool:
	if remaining_rounds > 0:
		remaining_rounds -= 1
		return true
	return false