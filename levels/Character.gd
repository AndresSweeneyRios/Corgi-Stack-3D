extends Node

var chars = []
var which = 1

func _ready():
	chars = [get_node("../Corgi"),get_node("../Corgi_Long"),get_node("../Schatzi")]
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
	pass
