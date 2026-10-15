extends Node3D

@export var splash_scene: PackedScene

@export var rain_radius: float = 8.0
@export var ray_height: float = 10.0
@export var ray_depth: float = 20.0

@export var splashes_per_second: float = 30.0

var elapsed := 0.0

func _process(delta: float) -> void:
	elapsed += delta

	var interval := 1.0 / maxf(splashes_per_second, 1.0)

	while elapsed >= interval:
		elapsed -= interval
		try_spawn_splash()


func try_spawn_splash() -> void:
	if splash_scene == null:
		return

	var offset := Vector3(
		randf_range(-rain_radius, rain_radius),
		0.0,
		randf_range(-rain_radius, rain_radius)
	)

	var ray_start := global_position + offset
	ray_start.y += ray_height

	var ray_end := ray_start - Vector3.UP * ray_depth

	var query := PhysicsRayQueryParameters3D.create(
		ray_start,
		ray_end
	)

	var result := get_world_3d().direct_space_state.intersect_ray(query)

	if result.is_empty():
		return

	var normal: Vector3 = result["normal"]

	if normal.dot(Vector3.UP) < 0.7:
		return

	var splash = splash_scene.instantiate()
	get_tree().current_scene.add_child(splash)

	splash.set_surface(
		result["position"],
		normal
	)
