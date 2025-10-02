@tool
extends XRToolsPickable

@onready
var magazine_snapzone = $GunModel/MagazineSnapZone

var magazine

signal gun_picked_up()
signal gun_loaded()

func _ready():
	super._ready()
	
	# Verbinde die XR Tools Signale
	picked_up.connect(_on_gun_picked_up)
	dropped.connect(_on_gun_dropped)

func _process(_delta: float):
	if is_picked_up() and get_picked_up_by_controller() and get_picked_up_by_controller().is_button_pressed("by_button"):
		if magazine:
			$AnimationPlayer.play("EjectMagazin")
	
func _on_gun_picked_up(_pickable):
	gun_picked_up.emit()
	
func _on_gun_dropped(_pickable):
	freeze = false
	
func on_magazine_loaded():
	pass
	
func on_magazine_ejected():
	magazine_snapzone.drop_object()
	magazine = null

func _on_magazine_snap_zone_has_picked_up(what: Variant) -> void:
	$AnimationPlayer.play("LoadMagazin")
	magazine = what
	gun_loaded.emit()
	