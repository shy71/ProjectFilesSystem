#pragma once

struct dirEntry
{
	char Filename[12];
	char fileOwner[12];
	unsigned int fileAddr;
	char crDate[10];
	char recFormat[2];//changed place to save bits which will later allow us to store more sectors in the file
	unsigned int fileSize;
	unsigned int eofRecNr;
	unsigned int maxRecSize;
	unsigned int actualRecSize;
	unsigned int keyOffset;
	unsigned int keySize;
	char keyType[2];
	unsigned char entryStatus;
	dirEntry();
	void copy(dirEntry & dir);
};
