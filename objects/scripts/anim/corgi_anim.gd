extends Spatial

var frames = []
var index = 0
var time = 0
var maxtime = 4
var parent

func _ready():
	frames = [get_node("Run1"),get_node("Idle"),get_node("Run2"),get_node("Run3"),get_node("Idle"),get_node("Run4")]
	parent = get_node("../")
	pass

func _process(delta):
	if get_node("../Target/Camera").is_current():
		if (parent.velocity.x != 0 || parent.velocity.z != 0):
			if time == 0 && index == 0:
				frames[1].hide()
				frames[index].show()
			
			time += delta*60
			
			if time >= maxtime:
				time = 0
				frames[index].hide()
				index += 1
				if index > frames.size()-1:
					index = 0
				frames[index].show()
		else:
			time = 0
			frames[index].hide()
			index = 0
			frames[1].show()
		pass