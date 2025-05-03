# music_reactive_effect.gd
@tool
extends CanvasItem

@export var brightness_effect_enabled: bool = false
@export var shake_effect_enabled: bool = false
@export var shake_scale_factor: float = 0.1

var original_modulate: Color = Color(1, 1, 1, 1)
var original_value: Vector2 = Vector2.ONE
var _apply_scale: Callable
var _restore_scale: Callable
var _apply_offset
var _restore_offset
var amplitude_smooth: float = 0.01

# Variables for parent control size and dynamic offset
var parent_control_size: Vector2 = Vector2.ZERO
var dynamic_offset: Vector2 = Vector2.ZERO

func _ready() -> void:
	original_modulate = modulate

	# Detect scale methods
	if has_method("get_scale") and has_method("set_scale"):
		original_value = get("scale")
		_apply_scale   = Callable(self, "set_scale")
		_restore_scale = Callable(self, "set_scale").bind(original_value)
	elif has_method("get_rect_scale") and has_method("set_rect_scale"):
		original_value = get("rect_scale")
		_apply_scale   = Callable(self, "set_rect_scale")
		_restore_scale = Callable(self, "set_rect_scale").bind(original_value)

	# Detect offset methods
	if has_method("set_position"):
		_apply_offset   = Callable(self, "set_position")
		_restore_offset = Callable(self, "set_position").bind(Vector2.ZERO)
	elif has_method("set_rect_position"):
		_apply_offset   = Callable(self, "set_rect_position")
		_restore_offset = Callable(self, "set_rect_position").bind(Vector2.ZERO)
	else:
		_apply_offset = null
		_restore_offset = null

	# Fetch parent Control node size
	var parent_node = get_parent()
	if parent_node and parent_node is Control:
		parent_control_size = parent_node.size
	else:
		parent_control_size = Vector2.ZERO

func _process(delta: float) -> void:
	if Engine.is_editor_hint():
		return

	# Audio amplitude calculation
	var dbL: float = AudioServer.get_bus_peak_volume_left_db(0, 0)
	var dbR: float = AudioServer.get_bus_peak_volume_right_db(0, 0)
	var ampL: float = pow(70.0, dbL / 12.0)
	var ampR: float = pow(70.0, dbR / 12.0)
	var amplitude: float = (ampL + ampR) * 1.1
	amplitude_smooth = lerp(amplitude_smooth, amplitude, 0.06)

	# Brightness effect
	if brightness_effect_enabled:
		modulate = original_modulate * (1.0 + amplitude_smooth * 0.2)
	else:
		modulate = original_modulate

	# Shake/scale effect with dynamic offset
	if shake_effect_enabled and _apply_scale:
		var s: float = 1.0 + amplitude_smooth * shake_scale_factor
		_apply_scale.call(original_value * s)

		# Compute offset to keep child centered in parent
		dynamic_offset = (parent_control_size - parent_control_size * s) * 0.5

		# Apply offset if available
		if _apply_offset:
			_apply_offset.call(dynamic_offset)
	elif _restore_scale:
		# Restore original scale
		_restore_scale.call()

		# Reset offset if available
		if _restore_offset:
			_restore_offset.call()
