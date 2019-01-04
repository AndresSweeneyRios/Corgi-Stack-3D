extends Node

var portal

func _ready():
	portal = find_node('PortalBox').get_parent()
	pass

func _process(delta):
	if portal.hit:
		get_tree().change_scene("res://levels/3.tscn")
	pass