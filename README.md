# M5N

A simple swap2-based gomoku server/broker, written in C#, compatible with CPython 3.11

## Requirement

### Compiling

- .NET SDK 8.0 or above
- GCC (MinGW on Windows)
- CPython 3.11

### Runtime

- CPython 3.11 (Only with python backend.)

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
- `set_colour`, `set_stone`, `choose_colour`, `place_stone`, `make_decision`, `victory`, `defeat` is reserved function/handler.
  - Do not rename these functions.
  - Do not change function signature. Or may cause the client to crash.
- **Only tested on pure python (without any packages)**

## How to write client

See working [sample code](test/python/main.py) with python.
You can prove application manually with sample-code!

...or you can use C/C++ with same constants:
See [sample code](test/c/impl.cpp) with C++.

## How to run client

Let your (python) source codes are in `./app` folder.

Then, you can run client by:

```sh
M5N.Slave <endpoint> Python ./app <module>
```

M5N also supports native-backend built into dll/so.
To use native-backend, Use below command:

```sh
M5N.Slave <endpoint> C path/to/your/binary.dll
```

These syntax is mnemonic with a below rule:

```sh
M5N.Slave <endpoint> <engine> [<engine-option>+] <module>
```

## How to run server

```sh
M5N.Master <port>
```

Server will be listening on specified port number. 
