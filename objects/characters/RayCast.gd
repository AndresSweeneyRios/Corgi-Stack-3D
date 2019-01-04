extends RayCast

func _ready():
	add_exception(find_node("../../../Portal"))
	add_exception(find_node("../../../Door"))
	pass

func _process(delta):
	pass
