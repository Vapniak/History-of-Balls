@tool
extends ggsSetting
class_name SettingWindowMode

@export var display_size: settingDisplaySize

enum WindowMode {
	FULLSCREEN,
	BORDERLESS_WINDOW,
	WINDOWED
}

func _init() -> void:
	value_type = TYPE_INT
	value_hint = PROPERTY_HINT_ENUM
	value_hint_string = "Fullscreen,Borderless Window,Windowed"
	default = WindowMode.FULLSCREEN
	section = "display"

func apply(value: int) -> void:
	var size_index = GGS.get_value(display_size)
	match value:
		WindowMode.FULLSCREEN:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)

		WindowMode.BORDERLESS_WINDOW:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
			DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_BORDERLESS, true)
		WindowMode.WINDOWED:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
			DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_BORDERLESS, false)

	display_size.apply(size_index)

	GGS.setting_applied.emit(key, value)
