#pragma once
#include"dirEntry.h"
#include"Sector.h"
#include"DAT.h"
#include"Disk.h"
class Disk;

class FCB
{
public:
	Disk *d;
	dirEntry fileDesc;
	DATtype FAT;
	Sector Buffer;
	bool editLock;
	string IOstatus;
	unsigned int currRecNr;
	unsigned int currSecNr;
	unsigned int currRecNrInBuff;
	string lastErrorMessage;
	FCB();
	FCB(Disk *disk);
	~FCB();
	void flushFile();
	void closeFile();
	void read(char *dest, unsigned int status = 0);
	void write(char *data);
	void seek(unsigned int from, int recordCount);
	void updateCancel();
	void deleteRecord();
	void updateRecord(char* update);
	void addRecord(char *record);

	//level 4
	string& GetLastErrorMessage();//returns the message of the last error
	int FCB::UpdatePlaceByRecordNumber(int num);//changes the curremt place by the record number
	int GetSectorNumberByIndex(int num);//get the sector number by the index
	void SetLastErrorMessage(string lastErrorMessage);//sets the message of the error that last occured
	bool isLast(){ return currRecNr == fileDesc.eofRecNr; }//checks if the current record is the last one
};