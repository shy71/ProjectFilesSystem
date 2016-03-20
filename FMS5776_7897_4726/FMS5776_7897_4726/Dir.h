#pragma once
#include "dirEntry.h"
struct SectorDir
{
	int sectorNr;
	dirEntry DirEntry[15];
	
};
struct RootDir
{
	SectorDir msbSector;
	SectorDir lsbSector;
	
	RootDir(void);
	~RootDir(void);
};