extends KinematicBody

var enter

func _ready():
	enter = get_node("Enter")
	pass

func _process(delta):
	if test_move(global_transform,Vector3(0,0,4)):
		enter.show()
		if Input.is_action_just_pressed("ui_accept"):
			get_tree().change_scene("res://levels/1.tscn")
	else:
		enter.hide()
	pass
