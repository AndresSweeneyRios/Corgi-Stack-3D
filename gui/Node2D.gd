extends Node2D

var grid = 50
var dark = Color(0,0,0,0.5)

func _draw():
	var win = OS.window_size
	var corner = OS.window_size/2*-1
	var bar = Rect2(Vector2(0,0),Vector2(win.x-grid,grid))
	draw_rect(Rect2(Vector2(),win), dark, true)
	draw_rect(Rect2(Vector2(-10,-10),Vector2(50,50)), dark, true)
	pass