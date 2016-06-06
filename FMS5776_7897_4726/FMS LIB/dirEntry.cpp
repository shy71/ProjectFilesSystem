#include "dirEntry.h"
#include <iostream>

dirEntry::dirEntry()
{
	entryStatus = '0';
	Filename[0] = NULL;
	fileOwner[0] = NULL; 
	fileAddr = -1;
	crDate[0] = NULL;
	recFormat[0] = NULL;
	fileSize = eofRecNr = maxRecSize = actualRecSize = keyOffset = keySize = 0;
	entryStatus = NULL;
	entryStatus=keyType[0] = keyType[1] = NULL; //???
}
void dirEntry::copy(dirEntry & dir)
{
	entryStatus = dir.entryStatus;
	strcpy_s(Filename, dir.Filename);
	strcpy_s(fileOwner, dir.fileOwner);
	strcpy_s(crDate, dir.crDate);
	strcpy_s(recFormat, dir.recFormat);
	fileAddr = dir.fileAddr;
	fileSize = dir.fileSize;
	eofRecNr = dir.eofRecNr;
	maxRecSize = dir.maxRecSize;
	actualRecSize = dir.actualRecSize;
	keyOffset = dir.keyOffset;
	keySize = dir.keySize;
}