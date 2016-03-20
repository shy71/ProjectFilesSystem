#pragma once
#include<bitset>

typedef std::bitset<1600> DATtype;

struct DAT
{
	unsigned int sectorNr;
	DATtype Dat;
	char emptyArea[820];
	DAT();
};
DAT::DAT()
{
	Dat.set();
	sectorNr = 1;
}

