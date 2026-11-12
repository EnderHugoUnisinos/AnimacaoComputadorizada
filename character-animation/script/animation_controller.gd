extends Node3D

@onready var anim_tree = %AnimationTree
var waiting = false

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if Input.is_action_just_pressed("WaitToggle"):
		waiting = !waiting
		anim_tree.set("parameters/StateMachine/conditions/waiting", waiting)
		anim_tree.set("parameters/StateMachine/conditions/not_waiting", !waiting)
	if Input.is_action_just_pressed("LookAround"):
		if !anim_tree.get("parameters/LookAroundShot/active"):
			anim_tree.set("parameters/LookAroundShot/request", AnimationNodeOneShot.ONE_SHOT_REQUEST_FIRE);
	if Input.is_action_just_pressed("Stretch"):
		if !anim_tree.get("parameters/StretchShot/active"):
			anim_tree.set("parameters/StretchShot/request", AnimationNodeOneShot.ONE_SHOT_REQUEST_FIRE);
		pass
	pass
