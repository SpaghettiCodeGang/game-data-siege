@tool
extends XRToolsPickable

## Represents a magazine that can be picked up in VR.
## Tracks the number of remaining rounds and handles automatic despawning.
## @author Sören Lehmann

## Signals for player interaction and magazin state changes.
## They enable communication with the C# player script.
signal magazine_picked_up()
signal magazine_despawned()

@export var capacity: int = 8

var remaining_rounds: int
var is_loaded_in_gun: bool = false

## Called when the gun enters the scene tree.
## Connects XR Tools pickup and drop events.

## Called when the magazine enters the scene tree.
## Connects XR Tools pickup and drop events and sets the remaining rounds to capacity.
func _ready():
	super._ready()
	remaining_rounds = capacity;
	
	# Connect XR Tools signals
	picked_up.connect(_on_magazine_picked_up)
	dropped.connect(_on_magazine_dropped)
	
## Called when the magazine is picked up.
## Stops the lifetime timer (if running) and emits `magazine_picked_up`.
func _on_magazine_picked_up(_pickable):
	if $LifetimeTimer.is_stopped() == false:
		$LifetimeTimer.stop()
	magazine_picked_up.emit()
	
## Called when the magazine is dropped.
## Resets physics freeze state and restarts lifetime timer if not loaded in a gun.
func _on_magazine_dropped(_pickable):
	freeze = false
	
	# Wait one frame to ensure physics updates before starting timer
	await get_tree().process_frame
	if not is_loaded_in_gun:
		$LifetimeTimer.start()
	
## Called when the lifetime timer times out.
## Emits `magazine_despawned` and removes the magazine from the scene.
func _on_lifetime_timer_timeout():
	magazine_despawned.emit()
	queue_free()
	
## Consumes one round from the magazine.
## @return True if a round was consumed, false if the magazine is empty.
func consume_round() -> bool:
	if remaining_rounds > 0:
		remaining_rounds -= 1
		return true
	return false