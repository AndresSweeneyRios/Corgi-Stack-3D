extends StaticBody

func _ready():
	pass

func _process(delta):
	if get_node('../').pressed:
		shape_owner_set_disabled(get_shape_owners()[0],true)
		hide()
	else:
		shape_owner_set_disabled(get_shape_owners()[0],false)
		show()
	pass
