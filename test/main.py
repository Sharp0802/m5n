
NONE = 0
WHITE = 1
BLACK = 2

COLOUR = 0
COORD = 1

my_map = [[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15,[0]*15]
my_colour = NONE


def view_map():
    for y in range(15):
        for x in range(15):
            if my_map[x][y] == WHITE:
                print('o', end='')
            elif my_map[x][y] == BLACK:
                print('x', end='')
            else:
                print(' ', end='')
        print('')


def set_colour(colour):
    print("set_colour:", colour)

    global my_colour
    my_colour = colour


def set_stone(x, y, colour):
    print("set_stone:", x, y, colour)

    global my_map
    my_map[x][y] = colour
    view_map()


def choose_colour():
    print("select_colour")

    return WHITE if input("colour>") == "WHITE" else BLACK


def place_stone():
    print("place_stone")

    x,y = map(int, input("x,y>").split(','))

    return x, y


def make_decision():
    print("make_decision")

    return COLOUR if input("decision>") == "COLOUR" else COORD


def victory():
    print("victory")


def defeat():
    print("defeat")
