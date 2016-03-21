#include "VolumeHeader.h"


VolumeHeader::VolumeHeader()
{
	addrDAT = 1;
	addrRootDir = 1;
	isFormated = false;
}

VolumeHeader::VolumeHeader(char name[12], char owner[12], unsigned int  rootdir = 1, unsigned int  rootdircpy = 3, unsigned int datcpy = 5)
{
	time_t t = time(0);   // get time now
	struct tm timeinfo;
	localtime_s(&timeinfo, &t);
	sectorNr = 0;
	strcpy_s(diskName, name);
	strcpy_s(diskOwner, owner);
	strcpy_s(prodDate, ((char*)timeinfo.tm_mday + '/' + timeinfo.tm_mon + '/' + timeinfo.tm_year));

}