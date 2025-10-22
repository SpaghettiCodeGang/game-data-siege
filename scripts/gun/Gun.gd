@tool
extends XRToolsPickable

## Represents a gun that can be picked up, loaded with a magazine, and fired.
## Handles XR controller input, magazine loading and ejection, and projectile instantiation.
## Designed for use with XR Tools and compatible magazine snap zones.
## @author Sören Lehmann

## Signals for player interaction and weapon state changes.
## They enable communication with the C# player script.
signal gun_picked_up()
signal gun_loaded()
signal gun_despawned()

@export var projectile_scene: PackedScene

@onready var magazine_snapzone = $GunModel/MagazineSnapZone
@onready var muzzle = $GunModel/Muzzle
@onready var muzzle_flash_effect: Node3D = $GunModel/VfxMuzzleFlash

var magazine
var trigger_pressed_last_frame: bool = false

## Called when the gun enters the scene tree.
## Connects XR Tools pickup and drop events.
func _ready():
	super._ready()
	
	picked_up.connect(_on_gun_picked_up)
	dropped.connect(_on_gun_dropped)

## Called every frame.
## Handles input for magazine ejection using the controller’s "by_button".
func _process(_delta: float):
	if is_picked_up() and get_picked_up_by_controller() and get_picked_up_by_controller().is_button_pressed("by_button"):
		if magazine and is_instance_valid(magazine):
			magazine.is_loaded_in_gun = false
			magazine.get_node("LifetimeTimer").start()
			$AnimationPlayer.play("EjectMagazin")
		
## Called every physics frame.
## Handles trigger input and fires a projectile when the trigger is pressed.	
func _physics_process(_delta: float):
	if is_picked_up() and get_picked_up_by_controller():
		var controller: XRController3D = get_picked_up_by_controller()
		var pressed = controller.is_button_pressed("trigger_click")

		if pressed and not trigger_pressed_last_frame:
			fire()

		trigger_pressed_last_frame = pressed

## Called when the gun is picked up.
## Emits the `gun_picked_up` signal.
func _on_gun_picked_up(_pickable):
	if $LifetimeTimer.is_stopped() == false:
		$LifetimeTimer.stop()
	gun_picked_up.emit()
	
## Called when the gun is dropped.
## Resets physics freeze state and restarts lifetime timer.
func _on_gun_dropped(_pickable):
	freeze = false

	$LifetimeTimer.start()
	
## Called when the lifetime timer times out.
## Emits `gun_despawned` and removes the gun from the scene.
func _on_lifetime_timer_timeout():
	on_magazine_ejected()
	gun_despawned.emit()
	queue_free()
	
## Called when a magazine has been loaded.
## Currently unused, but reserved for future logic.
func on_magazine_loaded():
	pass
	
## Handles magazine ejection.
## Marks the magazine as unloaded, starts its lifetime timer, drops it from the snap zone.
func on_magazine_ejected():
	if magazine and is_instance_valid(magazine):
		magazine.is_loaded_in_gun = false
		magazine.get_node("LifetimeTimer").start()
		magazine_snapzone.drop_object()
	magazine = null

## Triggered when the magazine snap zone picks up a magazine object.
## Loads the magazine, plays a load animation, and emits the `gun_loaded` signal.
## @param what: The picked-up magazine instance.
func _on_magazine_snap_zone_has_picked_up(what: Variant) -> void:
	if magazine != null:
		return
		
	magazine = what
	magazine.is_loaded_in_gun = true
	$AnimationPlayer.play("LoadMagazin")
	gun_loaded.emit()
	
## Fires a projectile from the muzzle if a valid magazine is inserted and has ammunition.
## Consumes one round from the magazine and spawns a projectile instance in the scene.
func fire() -> void:
	if projectile_scene == null:
		return
		
	if magazine == null or not is_instance_valid(magazine):
		$SoundEmpty.play()
		return
		
	if not magazine.consume_round():
		$SoundEmpty.play()
		return
		
	if muzzle_flash_effect:
		enable_all_particles(muzzle_flash_effect)
	
	var direction = muzzle.global_transform.basis.z
	var projectile = projectile_scene.instantiate()
	
	projectile.global_transform = muzzle.global_transform
	projectile.Fire(direction)
	$SoundShot.play()
	get_tree().current_scene.add_child(projectile)

## Recursively searches for and enables all GPUParticles3D nodes in the hierarchy.
## @param node: The root node from GPUParticles3Ds 
func enable_all_particles(node: Node) -> void:
	for child in node.get_children():
		if child is GPUParticles3D:
			child.emitting = true
		
		enable_all_particles(child)
