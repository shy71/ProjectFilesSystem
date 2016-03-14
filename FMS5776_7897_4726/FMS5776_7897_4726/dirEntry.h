#pragma once
struct dirEntry
{
	char Filename[12];
	char fileOwner[12];
	unsigned int fileAddr;
	char crDate[10];
	unsigned int fileSize;
	unsigned int eofRecNr;
	unsigned int maxRecSize;
	unsigned int actualRecSize;
	char recFormat[2];
	unsigned int keyOffset;
	unsigned int keySize;
	char keyType[2];
	unsigned char entryStatus;
};