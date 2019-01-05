extends Node

var chars = []

var portal
var which = 0

func _ready():
	chars = [get_node("Corgi"),get_node("Corgi_Long"),get_node("Schatzi")]
	portal = find_node('PortalBox').get_parent()
	pass

func _process(delta):
	if Input.is_action_just_pressed("next"):
		chars[which].get_node("Target/Camera").clear_current()
		which += 1

		if which > chars.size()-1:
			which = 0
			
		chars[which].get_node("Target/Camera").make_current()
		
	if Input.is_action_just_pressed("prev"):
		chars[which].get_node("Target/Camera").clear_current()
		which -= 1
		
		if which < 0:
			which = chars.size()-1
			
		chars[which].get_node("Target/Camera").make_current()
		
	if portal.hit:
		get_tree().change_scene("res://gui/title.tscn")
	pass
