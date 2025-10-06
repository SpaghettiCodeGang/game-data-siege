@tool
extends XRToolsPickable

@export var projectile_scene: PackedScene

@onready var magazine_snapzone = $GunModel/MagazineSnapZone
@onready var muzzle = $GunModel/Muzzle

var magazine
var trigger_pressed_last_frame: bool = false

signal gun_picked_up()
signal gun_loaded()
signal magazine_ejected()

func _ready():
	super._ready()
	
	# Verbinde die XR Tools Signale
	picked_up.connect(_on_gun_picked_up)
	dropped.connect(_on_gun_dropped)

func _process(_delta: float):
	if is_picked_up() and get_picked_up_by_controller() and get_picked_up_by_controller().is_button_pressed("by_button"):
		if magazine and is_instance_valid(magazine):
			magazine.is_loaded_in_gun = false
			magazine.get_node("LifetimeTimer").start()
			$AnimationPlayer.play("EjectMagazin")
			
func _physics_process(_delta: float):
	if is_picked_up() and get_picked_up_by_controller():
		var controller: XRController3D = get_picked_up_by_controller()
		var pressed = controller.is_button_pressed("trigger_click")

		if pressed and not trigger_pressed_last_frame:
			fire()

		trigger_pressed_last_frame = pressed

func _on_gun_picked_up(_pickable):
	gun_picked_up.emit()
	
func _on_gun_dropped(_pickable):
	freeze = false
	
func on_magazine_loaded():
	pass
	
func on_magazine_ejected():
	if magazine and is_instance_valid(magazine):
		magazine.is_loaded_in_gun = false
		magazine.get_node("LifetimeTimer").start()
		magazine_snapzone.drop_object()
	magazine = null
	magazine_ejected.emit()

func _on_magazine_snap_zone_has_picked_up(what: Variant) -> void:
	if magazine != null:
		return
		
	magazine = what
	magazine.is_loaded_in_gun = true
	$AnimationPlayer.play("LoadMagazin")
	gun_loaded.emit()
	
func fire() -> void:
	if projectile_scene == null:
		return
	
	var direction = muzzle.global_transform.basis.z
	var projectile = projectile_scene.instantiate()
	
	projectile.global_transform = muzzle.global_transform
	projectile.Fire(direction)
	get_tree().current_scene.add_child(projectile)