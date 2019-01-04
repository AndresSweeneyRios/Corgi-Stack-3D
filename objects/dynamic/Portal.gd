extends KinematicBody

var hit = false

func _process(delta):
	if move_and_collide(Vector3(0,0,0)):
		hit = true
	pass
