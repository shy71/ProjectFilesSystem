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
	//support functions
	void GoToNextRecord();
	void MoveRecord(int num);

	//level 4
	string& GetLastErrorMessage();
	int FCB::UpdatePlaceByRecordNumber(int num);
	int GetSectorNumberByIndex(int num);
	void SetLastErrorMessage(string lastErrorMessage);
	bool isLast(){ return currRecNr == fileDesc.eofRecNr; }
};