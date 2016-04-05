#pragma once
#include"dirEntry.h"
#include"Sector.h"
#include"DAT.h"
#include"Disk.h"
class FCB
{

public:
	Disk* d;
	dirEntry fileDesc;
	DATtype FAT;
	Sector Buffer;
	unsigned int currRecNr;
	unsigned int currSecNr;
	unsigned int currRecNrInBuff;
	FCB();
	FCB(Disk *disk);
	~FCB();
	void flushFile();
	void closeFile();
	void read(char *dest, unsigned int status = 0);
};