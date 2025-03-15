@tool
extends EditorPlugin


func _enter_tree() -> void:
	resource_saved.connect(on_resource_changed)


func on_resource_changed(res: Resource):
	if res is not Script:
		return

	if not res.is_tool():
		return

	if not inherits_from_programmatic_theme(res):
		return

	var constants = res.get_script_constant_map()
	if not constants.get("UPDATE_ON_SAVE", false):
		return

	var instance = res.new()
	instance._run()


func inherits_from_programmatic_theme(script: Script):
	if script.get_global_name() == "ProgrammaticTheme":
		return true

	script.is_tool()

	var base_class = script.get_base_script()
	if base_class == null:
		return false

	return inherits_from_programmatic_theme(base_class)
