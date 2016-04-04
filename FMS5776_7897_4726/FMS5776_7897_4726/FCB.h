#pragma once
#include"dirEntry.h"
#include"Disk.h"
class FCB
{
public:
	Disk* d;
	dirEntry fileDesc;
	unsigned int currRecNr;
	unsigned int currSecNr;
	unsigned int currRecNrInBuff;
	FCB();
	~FCB();
};