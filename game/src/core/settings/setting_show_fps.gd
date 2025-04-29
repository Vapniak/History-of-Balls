@tool
extends ggsSetting
class_name SettingShowFPS


func _init() -> void:
	value_type = TYPE_BOOL
	value_hint = PROPERTY_HINT_ENUM
	default = true
	section = "display"
	read_only_properties = ["value_type", "value_hint", "value_hint_string"]

func apply(value: int) -> void:
	ProjectSettings.set_setting("display/show_fps", value)

	GGS.setting_applied.emit(key, value)
