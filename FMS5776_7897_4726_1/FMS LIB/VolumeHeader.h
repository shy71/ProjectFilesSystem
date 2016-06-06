#pragma once
#include <ctime>
#include <string.h>
using namespace std;
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
