#pragma once
#include<bitset>

typedef std::bitset<1600> DATtype;

struct DAT
{
	//number of the sector
	unsigned int sectorNr;
	DATtype Dat;
	char emptyArea[812];
	DAT();
};