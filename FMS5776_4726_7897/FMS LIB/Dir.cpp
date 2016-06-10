#include "Dir.h"
#include<string.h>

SectorDir::SectorDir()
{
	for (int i = 0; i < 15; i++)
	{
		DirEntry[i].Filename[0] = NULL;
	}
}
dirEntry& SectorDir::operator[](int index){ return DirEntry[index]; }

void RootDir::SetClus(unsigned int ClusNumber)
{
	lsbSector.sectorNr = ClusNumber * 2;
	msbSector.sectorNr = lsbSector.sectorNr + 1;
}
void RootDir::clear()
{
	for (int i = 0; i < 15; i++)
	{
		lsbSector.DirEntry[i].entryStatus = 0;
		msbSector.DirEntry[i].entryStatus = 0;
	}
}
bool RootDir::searchFile(const char name[12])//useless
{
	for (int i = 0; i < 15; i++)
		if (strcmp(lsbSector[i].Filename, name) == 0 || strcmp(msbSector[i].Filename, name) == 0)
			return true;
	return false;
}
bool RootDir::IsFull()
{
	for (int i = 0; i < 15; i++)
		if (lsbSector[i].entryStatus != 1 || msbSector[i].entryStatus != 1)
			return false;
	return true;
}
void RootDir::WriteEntry(dirEntry & dir)
{
	for (int i = 0; i < 15; i++)
	{
		if (lsbSector[i].entryStatus != 1)
		{
			//lsbSector[i].copy(dir); need to check
			lsbSector[i] = dir;
			return;
		}
		if (msbSector[i].entryStatus != 1)
		{
			//msbSector[i].copy(dir);
			msbSector[i] = dir;
			return;
		}
	}
	throw "The disk is full!";
}

dirEntry* RootDir::getEntry(const char name[12])

{
	for (int i = 0; i < 15; i++)
	{
		if (strcmp(lsbSector[i].Filename, name) == 0)
			return &lsbSector[i];
		if (strcmp(msbSector[i].Filename, name) == 0)
			return &msbSector[i];
	}
	return NULL;
}

RootDir::RootDir()
{

}