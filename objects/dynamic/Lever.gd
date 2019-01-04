extends KinematicBody

var pressed = false
var just_pressed = false

func _ready():
	pass

func _process(delta):
	if just_pressed:
		just_pressed = false
		
	if test_move(get_transform(),Vector3(0,0.5,0)):
		if !pressed:
			just_pressed = true
			pressed = true
			translate(Vector3(0,-0.5,0))
			
	if pressed:
		get_node("on").show()
		get_node("off").hide()
	else:
		get_node("on").hide()
		get_node("off").show()
	pass
