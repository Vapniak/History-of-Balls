@tool
extends ggsSetting
class_name SettingVSync

enum VSyncModes {
	DISABLED,
	ENABLED,
	ADAPTIVE,
	MAILBOX  # Only available in some renderers
}

func _init() -> void:
	value_type = TYPE_INT
	value_hint = PROPERTY_HINT_ENUM
	value_hint_string = "Disabled,Enabled,Adaptive,Mailbox"
	default = VSyncModes.ENABLED
	section = "display"
	read_only_properties = ["value_type", "value_hint", "value_hint_string"]

func apply(value: int) -> void:
	match value:
		VSyncModes.DISABLED:
			DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_DISABLED)
		VSyncModes.ENABLED:
			DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_ENABLED)
		VSyncModes.ADAPTIVE:
			DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_ADAPTIVE)
		VSyncModes.MAILBOX:
			DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_MAILBOX)

	GGS.setting_applied.emit(key, value)
