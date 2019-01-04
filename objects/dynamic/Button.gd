extends KinematicBody

var pressed = false
var just_pressed = false

func _ready():
	pass

func _process(delta):
	if test_move(get_transform(),Vector3(0,1,0)):
		if !pressed:
			just_pressed = true
			pressed = true
			translate(Vector3(0,-0.5,0))
		if just_pressed:
			just_pressed = false
	else:
		if pressed && !just_pressed:
			translate(Vector3(0,0.5,0))
			pressed = false
	pass
