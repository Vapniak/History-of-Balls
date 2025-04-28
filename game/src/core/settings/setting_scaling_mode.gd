@tool
extends ggsSetting
class_name SettingScalingMode

func _init() -> void:
	value_type = TYPE_INT
	value_hint = PROPERTY_HINT_ENUM
	value_hint_string = "Bilinear,FSR 1.0,FSR 2.0"
	default = Viewport.Scaling3DMode.SCALING_3D_MODE_FSR2
	section = "display"

func apply(value) -> void:
	GGS.get_viewport().scaling_3d_mode = value
