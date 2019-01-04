extends KinematicBody

var hit = false

func _physics_process(delta):
	if test_move(global_transform,Vector3(-1,0,0)) || test_move(global_transform,Vector3(1,0,0)):
		hit = true
	pass