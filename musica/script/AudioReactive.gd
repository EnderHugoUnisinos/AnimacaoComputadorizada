extends Node

const BAND_LOW  = Vector2(20, 150)
const BAND_MID  = Vector2(150, 2000)
const BAND_HIGH = Vector2(2000, 8000)

var low_energy: float = 0.0
var mid_energy: float = 0.0
var high_energy: float = 0.0

var spectrum: AudioEffectSpectrumAnalyzerInstance

func _ready():
	spectrum = AudioServer.get_bus_effect_instance(0, 0)

func _process(_delta):
	if spectrum == null:
		return
	low_energy  = spectrum.get_magnitude_for_frequency_range(BAND_LOW.x,  BAND_LOW.y).length()
	mid_energy  = spectrum.get_magnitude_for_frequency_range(BAND_MID.x,  BAND_MID.y).length()
	high_energy = spectrum.get_magnitude_for_frequency_range(BAND_HIGH.x, BAND_HIGH.y).length()
