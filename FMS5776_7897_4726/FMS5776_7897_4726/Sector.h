#pragma once
struct Sector
{
	unsigned int sectorNr;
	int RawData[1020];
	int& operator[](int index);
};