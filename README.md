# M5N

A simple swap2-based gomoku server/broker, written in C#, compatible with CPython 3.11

## Requirement

### Compiling

- .NET SDK 8.0 or above
- GCC

### Runtime

- CPython 3.11

## Ruleset

Each player can do:

- dye : Choose colour
- put : Put stone

Let player A, B.

1. A put 3-times.
2. B choose:
    1. B dye.
    2. B put 2-times. A dye.
3. Continue with gomoku-ruleset.

## Disclaimer

- **Virtual-environment or such things are not supported.**
- `set_colour`, `set_stone`, `choose_colour`, `place_stone` is reserved function/handler.
  - Do not rename these functions.
  - Do not change function signature. Or may cause the client to crash.
- **Only tested on pure python (without any packages)**

## How to write client

Below is boilerplate for M5N.

```python

# Constants; Do not edit
NONE  = 0
WHITE = 1
BLACK = 2

# Your states
my_map = [[NONE]*15]*15
my_colour = NONE

### HANDLERS ###

def set_colour(colour: int) -> None:
    global my_colour
    my_colour = colour

def set_stone(x: int, y: int, colour: int) -> None:
    global my_map
    my_map[x][y] = colour

# Select your colour
def choose_colour() -> None:
    # calculating...
    return WHITE -or- BLACK

# Place your stone on map
def place_stone() -> None:
    # calculating...
    return (x, y)

################

```

## How to run client

Let your source codes are in `./app` folder.

Then, you can run client by:

```sh
M5N.Slave <port> ./app
```

## How to run server

```sh
M5N.Master <port>
```

Server will be listening on specified port number. 
