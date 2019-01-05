extends Camera

var camera
var target
var t
var raycast
var parent
var hidden = []
var window_focused = false
var deltamove = Vector2()
var maxpitch = 30
var minpitch = -40
var sensitivity = 0.3
var origin = Vector3(0,0,0)
var speed = 100
var zoom_speed = 5
var pivot_peed = 7

func _notification(what):
	if is_current():
	    if what == MainLoop.NOTIFICATION_WM_FOCUS_IN:
	        window_focused = true
	    elif what == MainLoop.NOTIFICATION_WM_FOCUS_OUT:
	        window_focused = false
	
func _input(event):
	if is_current():
		if event is InputEventMouseMotion:
			deltamove += event.relative * sensitivity
	pass

func _ready():
	camera = self
	t = get_node("../")
	target = get_node("../Raycast Wrap")
	parent = get_node('../../')
	raycast = get_node('../RayCast')
	window_focused = true
	pass

func _physics_process(delta):
	
	global_transform.origin = raycast.get_collision_point()
	look_at(parent.get_transform().origin,Vector3(0,1,0))
	translate(Vector3(0,0,-8))
	
	if is_current():
		get_parent().show()
		
		if window_focused:
			if deltamove.x > pivot_peed:
				deltamove.x = pivot_peed
			if deltamove.x < -pivot_peed:
				deltamove.x = -pivot_peed
				
			parent.rotation_degrees.y -= deltamove.x
			
			parent.move_and_slide(Vector3(0,0,0),Vector3(0,1,0))
			if parent.is_on_wall():
				parent.rotation_degrees.y += deltamove.x
			
			t.rotation_degrees.x -= deltamove.y
			
			deltamove = Vector2()
			
			if t.rotation_degrees.x > maxpitch:
				t.rotation_degrees.x = maxpitch
			if t.rotation_degrees.x < minpitch:
				t.rotation_degrees.x = minpitch
				
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
		
		else:
			deltamove = Vector2()
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		
		if Input.is_action_just_pressed("pause"):
			if window_focused:
				window_focused = false
			else:
				window_focused = true
				get_viewport().warp_mouse(Vector2(get_viewport().size.x/2,get_viewport().size.y/2))
		else:
			get_parent().hide()
	pass