extends Node3D

@onready var decal: Decal = $Decal
@onready var timer: Timer = $Timer

@export var lifetime: float = 1.0
@export var fade_duration: float = 0.4

func _ready() -> void:
	timer.one_shot = true
	timer.wait_time = maxf(lifetime - fade_duration, 0.0)
	timer.timeout.connect(fade_out)
	timer.start()

func set_surface(
		hit_position: Vector3,
		hit_normal: Vector3
) -> void:
	global_position = hit_position + hit_normal * 0.05

func fade_out() -> void:
	var tween := create_tween()
	tween.tween_property(
		decal,
		"modulate",
		Color(1.0, 1.0, 1.0, 0.0),
		fade_duration
	)
	tween.tween_callback(queue_free)
