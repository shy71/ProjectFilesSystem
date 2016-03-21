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
	entryStatus=keyType[0] = keyType[1] = NULL;
}