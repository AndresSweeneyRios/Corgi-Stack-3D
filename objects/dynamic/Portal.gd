extends KinematicBody

var hit = false

func _physics_process(delta):
	if test_move(global_transform,Vector3(0,0,-1)) || test_move(global_transform,Vector3(0,0,1)):
		hit = true
	pass