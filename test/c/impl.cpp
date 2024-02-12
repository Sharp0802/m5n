#include "pch.h"

#include <iostream>

#define EXPORT extern "C" __declspec(dllexport)

typedef unsigned char  byte;
typedef unsigned short ushort;


constexpr byte NONE  = 0;
constexpr byte WHITE = 1;
constexpr byte BLACK = 2;

constexpr ushort COLOUR = 0;
constexpr ushort COORD  = 1;

static byte s_colour;
static byte s_map[15][15];

typedef struct {
	byte X;
	byte Y;
} vec2;

EXPORT void SetColour(byte colour)
{
	std::cout << "set colour: " << (int)colour << std::endl;
	s_colour = colour;
}

EXPORT byte ChooseColour()
{
	std::string in;
	std::cout << "colour> ";
	std::cin >> in;
	return in == "WHITE" ? WHITE : BLACK;
}

EXPORT void SetStone(byte x, byte y, byte colour)
{
	std::cout << "set stone: " << (int)x << ',' << (int)y << ',' << (int)colour << std::endl;
	s_map[x][y] = colour;
}

EXPORT vec2 PlaceStone()
{
	int x, y;
	std::cout << "stone> ";
	std::cin >> x >> y;
	std::cout << x << ',' << y << std::endl;
	return { static_cast<byte>(x), static_cast<byte>(y) };
}

EXPORT ushort MakeDecision()
{
	std::string in;
	std::cout << "decision> ";
	std::cin >> in;
	return in == "COORD" ? COORD : COLOUR;
}

EXPORT void Victory()
{
	std::cout << "victory" << std::endl;
}

EXPORT void Defeat()
{
	std::cout << "defeat" << std::endl;
}
