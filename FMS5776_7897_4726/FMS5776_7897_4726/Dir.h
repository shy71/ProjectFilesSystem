#pragma once
#include "dirEntry.h"
struct SectorDir
{
	int sectorNr;
	dirEntry DirEntry[15];
	dirEntry& operator[](int index);
	
};
struct RootDir
{
	SectorDir msbSector;
	SectorDir lsbSector;
	RootDir();
	void clear();
	void SetClus(unsigned int);
	bool searchFile(const char name[12]);
	dirEntry* getEntry(const char name[12]);
	bool IsFull();
	void WriteEntry(dirEntry & dir);
};
