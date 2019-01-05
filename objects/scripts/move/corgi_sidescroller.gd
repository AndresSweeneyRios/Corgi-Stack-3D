extends KinematicBody

var speed = 2600
var direction = Vector3()
var velocity = Vector3()
var rot = Vector3()
var gravity = -5
var jspd = 110
var weight = 1
var ext_weight = 0
		
func kp(k):
	return Input.is_action_pressed(k)
		
func _physics_process(delta):
	direction = Vector3(0,0,0)
		
	if get_node("Target/Camera").is_current():
		
		if kp("ui_left"):
			direction += -get_transform().basis.x
			rot.y = 180
		if kp("ui_right"):
			direction += get_transform().basis.x
			rot.y = 0
		
		get_node('Animation').rotation_degrees = rot;
		get_node('CollisionShape').rotation_degrees = rot;
		
		direction = direction.normalized() * speed * delta
		velocity.x = direction.x
		velocity.z = direction.z
		
		if velocity.y == 0 && test_move(global_transform,Vector3(0,-1,0)) && Input.is_action_just_pressed("jump"):
			velocity.y = jspd
	else:
		velocity.x = 0
		velocity.z = 0
		
	velocity.y += gravity
	velocity = move_and_slide(velocity, Vector3(0,1,0), 5.0)