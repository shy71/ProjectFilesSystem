#pragma once
struct Sector
{
	unsigned int sectorNr;
	int RawData[1020];

	Sector(void);
	~Sector(void);
	int& operator[](int index);
};