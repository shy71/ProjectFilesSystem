#include "Dir.h"

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
RootDir::RootDir()
{
	
}