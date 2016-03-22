#include "VolumeHeader.h"


VolumeHeader::VolumeHeader()
{
	sectorNr = 0;
	diskName[0] = NULL;
	diskOwner[0] = NULL;
	prodDate[0] = NULL;
	ClusQty = 0;
	dataClusQty = 0;
	addrDAT = 0;
	addrRootDir = 0;
	addrDATcpy = 0;
	addrRootDirCpy = 0;
	addrDataStart = 0;
	formatDate[0] = NULL;
	isFormated = false;
}