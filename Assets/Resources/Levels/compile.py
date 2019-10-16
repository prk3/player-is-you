import sys
import cv2

color_to_id_map = {
	"000000":  0, # empty

	"408040":  1, # decoration bush
	"404080":  2, # decoration tile
	"804040":  3, # decoration ?

	"808080":  4, # connector is

	"ff80ff":  5, # entity player
	"ffbf80":  6, # entity rock
	"80ffff":  7, # entity water
	"8080ff":  8, # entity wall
	"ffff80":  9, # entity flag
	"ff8080": 10, # entity skull
	"bfff80": 11, # entity cloud

	"ff00ff": 12, # subject player
	"ff8000": 13, # subject rock
	"00ffff": 14, # subject water
	"0000ff": 15, # subject water
	"ffff00": 16, # subject flag
	"ff0000": 17, # subject skull
	"80ff00": 18, # subject cloud

	"800080": 19, # trait me
	"804000": 20, # trait move
	"008080": 21, # trait sink
	"000080": 22, # trait stop
	"808000": 23, # trait win
	"800000": 24, # trait lose
	"408000": 25, # trait float
}

def get_padded_id_from_color(r, g, b):
	game_id = color_to_id_map["{:06x}".format((r << 16) + (g << 8) + b)]
	return "{:02}".format(game_id)


assert(len(sys.argv) == 3)

img = cv2.imread(sys.argv[1])

height, width, channels = img.shape

ids = "".join([get_padded_id_from_color(
	img[y][x][2],
	img[y][x][1],
	img[y][x][0]
) for y in range(height) for x in range(width)])

output = "{}\n{}\n{}\n".format(width, height, ids)

text_file = open(sys.argv[2], "w")
text_file.write(output)
text_file.close()

