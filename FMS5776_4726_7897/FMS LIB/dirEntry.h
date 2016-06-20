#pragma once

struct dirEntry
{
	char Filename[12];
	char fileOwner[12];
	unsigned int fileAddr;
	char crDate[10];
	char recFormat[2];//changed place to save bits which will later allow us to store more sectors in the file
	unsigned int fileSize;
	unsigned int eofRecNr;//the record after the last written one
	unsigned int maxRecSize;//the maximum size of a record
	unsigned int actualRecSize;
	unsigned int keyOffset;//from where the key field is written from the begining of a record
	unsigned int keySize;
	char keyType[2];
	unsigned char entryStatus;
	dirEntry();
	void copy(dirEntry & dir);//copies an entry
};
