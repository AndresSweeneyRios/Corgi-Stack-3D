extends KinematicBody

var speed = 2600
var direction = Vector3()
var gravity = -5
var velocity = Vector3()
var jspd = 110
var rot = Vector3()
		
func _ready():
	pass
		
func _physics_process(delta):
	if get_node("Target/Camera").is_current():
		direction = Vector3(0,0,0)
		
		if Input.is_action_pressed("ui_left"):
			direction.x += -get_transform().basis.x.x
			direction.z += -get_transform().basis.x.z
			rot.y = 180
			
		if Input.is_action_pressed("ui_right"):
			direction.x += get_transform().basis.x.x
			direction.z += get_transform().basis.x.z
			rot.y = 0
			
		if Input.is_action_pressed("ui_up"):
			direction.x += -get_transform().basis.z.x
			direction.z += -get_transform().basis.z.z
			rot.y = 90
			
		if Input.is_action_pressed("ui_down"):
			direction.x += get_transform().basis.z.x
			direction.z += get_transform().basis.z.z
			rot.y = -90
			
		if !Input.is_action_pressed("ui_right") && Input.is_action_pressed("ui_left") && Input.is_action_pressed("ui_down"):
			rot.y = -125
			
		if !Input.is_action_pressed("ui_left") && Input.is_action_pressed("ui_right") && Input.is_action_pressed("ui_down"):
			rot.y = -45
			
		if !Input.is_action_pressed("ui_right") && Input.is_action_pressed("ui_left") && Input.is_action_pressed("ui_up"):
			rot.y = 125
			
		if !Input.is_action_pressed("ui_left") && Input.is_action_pressed("ui_right") && Input.is_action_pressed("ui_up"):
			rot.y = 45
		
		get_node('Animation').rotation_degrees = rot;
		get_node('CollisionShape').rotation_degrees = rot;
		
		direction = direction.normalized()
		direction = direction * speed * delta
			
		velocity.x = direction.x
		velocity.z = direction.z
		
		if velocity.y == 0 && Input.is_action_just_pressed("jump"):
			velocity.y = jspd
	else:
		velocity.x = 0
		velocity.z = 0
		
	velocity.y += gravity
	
	velocity = move_and_slide(velocity, Vector3(0,1,0), 5.0)
	