@tool
extends Node
class_name ReactiveMusic

@export var brightness_effect_enabled: bool = false
@export var shake_effect_enabled: bool = false
@export var shake_scale_factor: float = 0.1

@export var reactive_node: CanvasItem

var original_modulate: Color = Color(1, 1, 1, 1)
var original_value: Vector2 = Vector2.ONE
var _apply_scale
var _restore_scale
var amplitude_smooth: float = 0.01

func _ready() -> void:
	original_modulate = reactive_node.modulate

	if reactive_node.has_method("get_scale") and reactive_node.has_method("set_scale"):
		original_value = reactive_node.get("scale")
		_apply_scale   = Callable(reactive_node, "set_scale")
		_restore_scale = Callable(reactive_node, "set_scale").bind(original_value)
	elif reactive_node.has_method("get_rect_scale") and reactive_node.has_method("set_rect_scale"):
		original_value = get("rect_scale")
		_apply_scale   = Callable(self, "set_rect_scale")
		_restore_scale = Callable(self, "set_rect_scale").bind(original_value)

func _process(delta: float) -> void:
	if Engine.is_editor_hint():
		return

	var dbL: float = AudioServer.get_bus_peak_volume_left_db(0, 0)
	var dbR: float = AudioServer.get_bus_peak_volume_right_db(0, 0)
	var ampL: float = pow(70.0, dbL / 12.0)
	var ampR: float = pow(70.0, dbR / 12.0)
	var amplitude: float = (ampL + ampR) * 1.1

	amplitude_smooth = lerp(amplitude_smooth, amplitude, 0.06)

	if brightness_effect_enabled:
		var bf: float = 0.2
		reactive_node.modulate = original_modulate * (1.0 + amplitude_smooth * bf)
	else:
		reactive_node.modulate = original_modulate

	if shake_effect_enabled and _apply_scale:
		var s: float = 1.0 + amplitude_smooth * shake_scale_factor
		_apply_scale.call(original_value * s)
	elif _restore_scale:
		_restore_scale.call()
