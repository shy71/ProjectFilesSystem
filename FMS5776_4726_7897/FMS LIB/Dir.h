#pragma once
#include "dirEntry.h"
struct SectorDir
{
	int sectorNr;
	SectorDir();
	dirEntry DirEntry[15];
	dirEntry& operator[](int index);

};
struct RootDir
{
	SectorDir msbSector;
	SectorDir lsbSector;
	RootDir();
	void clear();
	//set the cluster number
	void SetClus(unsigned int);
	//search for a specific file by name
	bool searchFile(const char name[12]);
	//get an entry
	dirEntry* getEntry(const char name[12]);
	//checks if the rootdir is full and can't add anymore
	bool IsFull();
	//add a new entry for a new file
	void WriteEntry(dirEntry & dir);
};
