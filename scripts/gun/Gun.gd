@tool
extends XRToolsPickable

@onready
var magazin_snapzone = $GunModel/MagazinSnapZone

var magazin
var controller

func _ready():
	super._ready()
	
	# Verbinde die XR Tools Signale
	picked_up.connect(_on_picked_up)
	dropped.connect(_on_dropped)

func _process(_delta: float):
	if is_picked_up() and get_picked_up_by_controller() and get_picked_up_by_controller().is_button_pressed("by_button"):
		if magazin:
			$AnimationPlayer.play("EjectMagazin")
	
func _on_picked_up(_pickable):
	pass
	
func _on_dropped(_pickable):
	freeze = false
	
func on_magazin_loaded():
	pass
	
func on_magazin_ejected():
	magazin_snapzone.drop_object()
	
	magazin = null

func _on_magazin_snap_zone_has_picked_up(what: Variant) -> void:
	$AnimationPlayer.play("LoadMagazin")
	magazin = what