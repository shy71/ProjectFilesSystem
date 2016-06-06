#pragma once
struct Sector
{
	unsigned int sectorNr;
	char RawData[1020];

	char& operator[](int index);
};