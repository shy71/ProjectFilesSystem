#pragma once
struct Sector
{
	unsigned int sectorNr;
	char RawData[1020];

	char& operator[](int index);//returns the char from the data in the place chosen
};