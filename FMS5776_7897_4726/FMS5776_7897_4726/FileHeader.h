#pragma once

#include "dirEntry.h"
#include "DAT.h"

struct FileHeader
{
	unsigned int 	sectorNr;
	dirEntry 	fileDesc;
	DATtype 	FAT;
	char 	emptyArea[744];
	FileHeader();
};
FileHeader::FileHeader()
{
	FAT.reset();
}
