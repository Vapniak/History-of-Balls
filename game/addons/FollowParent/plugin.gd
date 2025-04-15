@tool
extends EditorPlugin


func _enter_tree():
	add_custom_type(
			"FollowParent2D", "Node2D",
			preload("res://addons/FollowParent/FollowParent2D/FollowParent2D.gd"), null)
	add_custom_type(
			"FollowParent3D", "Node3D",
			preload("res://addons/FollowParent/FollowParent3D/FollowParent3D.gd"), null)


func _exit_tree():
	remove_custom_type("FollowParent2D")
	remove_custom_type("FollowParent3D")
