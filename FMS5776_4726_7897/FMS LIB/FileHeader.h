#pragma once

#include "dirEntry.h"
#include "DAT.h"

struct FileHeader
{
	unsigned int 	sectorNr;
	dirEntry 	fileDesc;
	DATtype 	FAT;
	char 	emptyArea[752];
	FileHeader();//header of a file 
};
