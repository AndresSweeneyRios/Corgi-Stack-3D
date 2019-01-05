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