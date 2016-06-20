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
	void flushFile();//flushes whart's in the buffer into the disk
	void closeFile();//closes the file	
	void read(char *dest, unsigned int status = 0);//reads the current record and saves it in dest
	void write(char *data);//writes a record in the current place
	void seek(unsigned int from, int recordCount);//seeks the current record to a specific place
	void updateCancel();//cancels update mode
	void deleteRecord();//deletes the current record ONLY WORKS IN UPDATE MODE
	void updateRecord(char* update);//updates the current record ONLY WORKS IN UPDATE MODE
	void addRecord(char *record);//adds a new record to the file
	//support functions
	void GoToNextRecord();//moves to the next record
	void MoveRecord(int num);//moves to a record by a number

	//level 4
	string& GetLastErrorMessage();//returns the message of the last error
	int FCB::UpdatePlaceByRecordNumber(int num);//changes the curremt place by the record number
	int GetSectorNumberByIndex(int num);//get the sector number by the index
	void SetLastErrorMessage(string lastErrorMessage);//sets the message of the error that last occured
	bool isLast(){ return currRecNr == fileDesc.eofRecNr; }//checks if the current record is the last one
};