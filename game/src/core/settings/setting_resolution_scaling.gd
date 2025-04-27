@tool
extends ggsSetting
class_name SettingResolutionScale

func _init() -> void:
	value_type = TYPE_FLOAT
	value_hint = PROPERTY_HINT_RANGE
	value_hint_string = "0.5,2.0,0.1"
	default = 1.0
	section = "display"

func apply(value) -> void:
	GGS.get_viewport().scaling_3d_scale = value
	GGS.setting_applied.emit(key, value)
