#pragma once
#include "dirEntry.h"
struct SectorDir
{
	int sectorNr;
	dirEntry DirEntry[14];
	char unUse[12];
};
struct RootDir
{
	SectorDir msbSector;
	SectorDir lsbSector;
	
	RootDir(void);
	~RootDir(void);
};