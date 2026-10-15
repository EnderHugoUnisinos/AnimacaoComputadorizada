extends Node3D

@onready var audio = get_node("/root/AudioReactive")
@onready var impactdet	: Node3D = $ImpactDetect
@onready var particles	: GPUParticles3D = $Rain
@onready var stream		: AudioStreamPlayer3D = $AudioStreamPlayer3D
@onready var we        	: WorldEnvironment = $WorldEnvironment
@onready var l1        	: Light3D = $Light1
@onready var l2        	: Light3D = $Light2
@onready var rl        	: Light3D = $RedLight
@onready var li        	: Light3D = $Lightning
@onready var ch        	: MeshInstance3D = $"CH-Man"

const HIGH_SCALE     := 1000.0

const SHAKE_MAX      := 0.06     
const SHAKE_DECAY    := 8.0      

@onready var cam : Camera3D = $Camera3D
var shake_amp     := 0.0
var cam_home      := Vector3.ZERO

const BLEND_SPEED    := 5.0    
const SHAPE_COUNT    := 3
const SHAPE_MIN_TIME := 5.0     
const SHAPE_MAX_TIME := 20.0      

const HEAD_GAIN := 2.0
const BODY_GAIN := 2.0
const SHOULDER_GAIN := 3.0

const HEAD_SMOOTH := 2.0
const BODY_SMOOTH := 3.0
const SHOULDER_SMOOTH := 5.0

const HEAD_MAX := 0.6
const BODY_MAX := 0.5
const SHOULDER_MAX := 0.8

var head_value := 0.0
var body_value := 0.0
var shoulder_value := 0.0

# looks better when paired up with:
# a good song never dies : 3.0; 25.0; 2.5; 0.15; 1.0; 30.0;
# the future is a foreign land : 3.0; 25.0; 1.5; 0.15; 1.0; 30.0;

const HIGH_AVG_RATE  := 3.0
const BASE_SPIKE     := 25.0
const SPIKE_RATIO    := 1.5
const FLASH_TIME     := 0.08
const FLASH_COOLDOWN := 1.0     
const HIGH_FLOOR := 30.0

var started         := false
var prev_high       := 0.0
var high_avg        := 0.0
var flash_timer     := 0.0
var cooldown_timer  := 0.0

var shape_vals      := [0.0, 0.0, 0.0] 
var active_shape    := 0
var shape_timer     := 0.0

var rain_angle := -30.0

var song_ended 		:= false

func _ready() -> void:
	cam_home = cam.position
	active_shape = randi() % SHAPE_COUNT
	shape_timer = randf_range(SHAPE_MIN_TIME, SHAPE_MAX_TIME)

func _set_sky_energy(v: float) -> void:
	var sky := we.environment.sky
	if sky:
		var mat := sky.sky_material
		if mat:
			mat.energy_multiplier = v

func song_finished() -> void:
	song_ended = true

func smooth_motion(
	current: float,
	target: float,
	delta: float,
	attack: float,
	release: float
) -> float:
	var speed = attack if target > current else release
	return lerpf(
		current,
		target,
		clamp(delta * speed, 0.0, 1.0)
	)

func _process(delta: float) -> void:
	var low  = audio.low_energy
	var mid  = audio.mid_energy
	var high = audio.high_energy
	var cur_high = high * HIGH_SCALE
	
	
	if (started):
		l1.light_energy = clamp(mid * 2.0, 0.5, 0.7)
		l2.light_energy = clamp(low * 2.0, 0.5, 0.7)
	
	var target_angle = lerpf(
	-15.0,
	-40.0,
	clamp(low * 5.0, 0.0, 1.0)
	)
	rain_angle = lerpf(rain_angle, target_angle, clamp(delta * 2.0, 0.0, 1.0))
	particles.rotation_degrees.z = rain_angle
	

	var head_target = clamp(
		low * HEAD_GAIN,
		0.0,
		HEAD_MAX
	)

	var body_target = clamp(
		mid * BODY_GAIN,
		0.0,
		BODY_MAX
	)

	var shoulder_target = clamp(
		high * SHOULDER_GAIN,
		0.0,
		SHOULDER_MAX
	)

	head_value = smooth_motion(
		head_value, head_target, delta, 3.0, 1.2
	)

	body_value = smooth_motion(
		body_value, body_target, delta, 4.0, 1.5
	)

	shoulder_value = smooth_motion(
		shoulder_value, shoulder_target, delta, 7.0, 2.0
	)

	# Apply all three blend shapes
	ch.set_blend_shape_value(0, head_value)
	ch.set_blend_shape_value(1, body_value)
	ch.set_blend_shape_value(2, shoulder_value)


	if low < 0.1:
		rl.light_energy = clamp(high, 3.3, 3.5) if (started and !song_ended) else clamp(high, 0.0, 1.0)
	else:
		started = true
		rl.light_energy = clamp(high * 6.0, 3.1, 3.5)

	high_avg = lerpf(high_avg, cur_high, clamp(delta * HIGH_AVG_RATE, 0.0, 1.0))
	
	var threshold = high_avg * SPIKE_RATIO
	var rise = cur_high - prev_high
	prev_high = cur_high

	if cooldown_timer > 0.0:
		cooldown_timer -= delta
	elif rise > threshold and cur_high > HIGH_FLOOR:
		flash_timer = FLASH_TIME
		cooldown_timer = FLASH_TIME + FLASH_COOLDOWN
		shake_amp = SHAKE_MAX

	if flash_timer > 0.0:
		flash_timer -= delta
		_set_sky_energy(50.0)
		li.light_energy = 2.0
	else:
		_set_sky_energy(1.0)
		li.light_energy = 0.0

	particles.emitting = low > 0.01
	#if low > 0.1:
	#	particles.amount_ratio = clamp(mid * 3.0, 0.1, 0.3)
	#else:
	particles.amount_ratio = clamp(mid * 3.0, 0.1, 1.0)
	if !(low > 0.01):
		impactdet.splashes_per_second = 0
	else:	
		impactdet.splashes_per_second = mid*1000
	
	if shake_amp > 0.0001:
		shake_amp = move_toward(shake_amp, 0.0, SHAKE_DECAY * delta * SHAKE_MAX)
		cam.position = cam_home + Vector3(
			randf_range(-shake_amp, shake_amp),
			randf_range(-shake_amp, shake_amp),
			randf_range(-shake_amp, shake_amp)
		)
	else:
		shake_amp = 0.0
		cam.position = cam_home
		
