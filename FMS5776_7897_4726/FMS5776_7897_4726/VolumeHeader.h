#pragma once
#include <ctime>
struct VolumeHeader
{
	unsigned int sectorNr;
	char diskName[12];
	char diskOwner[12];
	char prodDate[10];
	unsigned int ClusQty;
	unsigned int dataClusQty;
	unsigned int addrDAT;
	unsigned int addrRootDir;
	unsigned int addrDATcpy;
	unsigned int addrRootDirCpy;
	unsigned int addrDataStart;
	char formatDate[10];
	bool isFormated;
	char emptyArea[944];
	VolumeHeader();
	VolumeHeader(char name[12], char owner[12], unsigned int , unsigned int, unsigned int);

};
VolumeHeader::VolumeHeader()
{
	addrDAT = 1;
	addrRootDir = 1;
	isFormated = false;
}

VolumeHeader::VolumeHeader(char name[12], char owner[12], unsigned int  rootdir = 1, unsigned int  rootdircpy = 3, unsigned int datcpy = 5)
{
	time_t t = time(0);   // get time now
	struct tm * now = localtime(&t);
	sectorNr = 0;
	strcpy(diskName, name);
	strcpy(diskOwner,owner);
	strcpy(prodDate, (now->tm_mday + '/' + now->tm_mon + '/' + now->tm_year));
	
}