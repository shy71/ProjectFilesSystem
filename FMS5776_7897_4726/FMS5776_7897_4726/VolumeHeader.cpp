#include "VolumeHeader.h"


VolumeHeader::VolumeHeader()
{
	sectorNr = 0;
	diskName[0] = '\0';
	diskOwner[0] = '\0';
	prodDate[0] = '\0';
	ClusQty = 0;
	dataClusQty = 0;
	addrDAT = 0;
	addrRootDir = 0;
	addrDATcpy = 0;
	addrRootDirCpy = 0;
	addrDataStart = 0;
	formatDate[0] = '\0';
	isFormated = false;
}