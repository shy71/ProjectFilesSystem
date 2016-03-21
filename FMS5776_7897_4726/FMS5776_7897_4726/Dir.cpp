#include "Dir.h"

void RootDir::SetClus(unsigned int ClusNumber)
{
	lsbSector.sectorNr = ClusNumber * 2;
	msbSector.sectorNr = lsbSector.sectorNr + 1;
}