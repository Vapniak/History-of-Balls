extends Control

@export var mode_option: OptionButton



func apply_window_mode(mode: int):
	match mode:
		SettingWindowMode.WindowMode.FULLSCREEN:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)

		SettingWindowMode.WindowMode.BORDERLESS_WINDOW:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
			DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_BORDERLESS, true)

		SettingWindowMode.WindowMode.WINDOWED:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
			DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_BORDERLESS, false)


	# Update the viewport scaling
	#_update_viewport_scaling()

#func _update_viewport_scaling():
	#var scaling_mode = GGS.get_setting_value("resolution_scaling")
	#var window_size = DisplayServer.window_get_size()
	#var base_size = Vector2i(1920, 1080)  # Your target render resolution
#
	#var root = get_tree().root
#
	#match scaling_mode:
		#SettingResolutionScale.ScalingModes.NATIVE:
			#root.size = window_size
			#root.scaling_3d_mode = Viewport.SCALING_3D_MODE_OFF
#
		#SettingResolutionScale.ScalingModes.FIT:
			#root.size = base_size
			#root.scaling_3d_mode = Viewport.SCALING_3D_MODE_BILINEAR
#
		#SettingResolutionScale.ScalingModes.FILL:
			#root.size = base_size
			#root.scaling_3d_mode = Viewport.SCALING_3D_MODE_FSR
#
		#SettingResolutionScale.ScalingModes.STRETCH:
			#root.size = window_size
			#root.scaling_3d_mode = Viewport.SCALING_3D_MODE_OFF
