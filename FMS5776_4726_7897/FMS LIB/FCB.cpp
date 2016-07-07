#include "FCB.h"
#pragma region Constructors Destructor
FCB::FCB()
{
	d = NULL;
	editLock = false;
	IOstatus = "I";
	currRecNr = currRecNrInBuff = currSecNr = -1;
}
FCB::FCB(Disk *disk)
{
	d = disk;
	editLock = false;
	IOstatus = "I";
	currRecNr = currRecNrInBuff = currSecNr = -1;
}
FCB::~FCB()
{
}
#pragma endregion

void FCB::flushFile()
{
	if (IOstatus == "I")
		return;
	if (editLock)
		throw "You can't flush the buffer while locked in edit mode!";
	*((d->rootDir).getEntry(fileDesc.Filename)) = fileDesc;
	d->currDiskSectorNr = currSecNr;
	if (d->currDiskSectorNr > 0 && d->currDiskSectorNr < 3200)
		d->writeSector(d->currDiskSectorNr, &Buffer);
	d->Update();
}
void FCB::closeFile()//eof record is changed every edit of a record so it's dealt with already
{
	if (editLock)
		throw "You can't close the file while locked in edit mode!";
	flushFile();
	d = NULL;
	currRecNr = currRecNrInBuff = currSecNr = -1;
	IOstatus = "I";
	FAT.reset();
}
void FCB::read(char *dest, unsigned int status)//finish function
{
	if (editLock)
		throw "You can't read while locked in edit mode!";
	if (IOstatus == "I" && status == 1)
		throw "You can't open it for editing since it's read only.";
	//flushFile();
	d->readSector(currSecNr, &Buffer);
	if (fileDesc.recFormat[0] == 'F')
	{
		if (fileDesc.eofRecNr == currRecNr)
			throw "File Is Finshed!";
		//read from buffer the current record
		strncpy(dest,Buffer.RawData + fileDesc.maxRecSize*currRecNrInBuff, fileDesc.maxRecSize);
		//	dest = new char[fileDesc.maxRecSize];
		//for (int i = fileDesc.maxRecSize*currRecNrInBuff; i < fileDesc.actualRecSize*(currRecNrInBuff + 1); i++)//think of changing
		//	dest[i] = Buffer[i];
		if (status == 0)
			UpdatePlaceByRecordNumber(currRecNr + 1);
		else
			editLock = true;
	}

}
void FCB::write(char *record)//ρεσ?
{
	if (editLock)
		throw "You can't write while locked in edit mode!";
	if (IOstatus=="I")
		throw "You can't edit these files because the file is read only.";
	//flushFile();
	if (fileDesc.recFormat[0] == 'F')
	{
		for (int i = 0; i < fileDesc.maxRecSize; i++)
			Buffer[currRecNrInBuff*fileDesc.maxRecSize + i] = record[i];
		if (currRecNr == fileDesc.eofRecNr)
			fileDesc.eofRecNr++;
	}
	flushFile();
	UpdatePlaceByRecordNumber(currRecNr + 1);
	//else //the file has varrying sized records
	//{
	//	int startOfRecord = 0;
	//	for (int i = 0; startOfRecord < 1020 && i < currRecNrInBuff;startOfRecord++)
	//		if (Buffer[startOfRecord] == '~')
	//		{
	//			i++;
	//			startOfRecord++;
	//		}
	//	if (startOfRecord < 1020)
	//	{
	//		int size;
	//		for (size = 0; Buffer[size + startOfRecord] != '~' && size + startOfRecord < 1019; size++);
	//		if (size < strlen(record))
	//			throw "The record is too large";
	//		if (size + startOfRecord < 1018 && Buffer[size + startOfRecord + 2] == NULL || size + startOfRecord >= 1019)//it's the last record
	//		{
	//			if (strlen(record) < 1019 - startOfRecord)
	//				for (int i = 0; i < strlen(record); i++)
	//					Buffer[startOfRecord + i] = record[i];
	//			Buffer[startOfRecord + strlen(record)] = '~';
	//			for (int i = startOfRecord + strlen(record) + 1; i < 1020; i++)
	//				Buffer[i] = NULL;
	//		}
	//		else if (strlen(record) < size)
	//		{
	//			for (int i = 0; i < strlen(record); i++)
	//				Buffer[startOfRecord + i] = record[i];
	//			Buffer[startOfRecord + strlen(record)] = '~';
	//			//pull everything back
	//			for (int i = startOfRecord + size, j = startOfRecord + strlen(record); j < 1020; i++, j++)
	//			{
	//				if (i < 1020)
	//					Buffer[j] = Buffer[i];
	//				else
	//					Buffer[j] = NULL;
	//			}
	//		}
	//		else //it's the right size
	//		{
	//			for (int i = startOfRecord; i < size; i++)
	//				Buffer[i] = record[i];
	//		}
	//	}
	//}
}
void FCB::seek(unsigned int from, int recordCount)//check for situation where he gave too big recordcount
{
	//deal with deleted files problem
	flushFile();
	switch (from)
	{
	case 0://from beginning
		if (recordCount < 0)
			throw "You cant move backward from the start of the file";
		UpdatePlaceByRecordNumber(recordCount);

		break;
	case 1:
		UpdatePlaceByRecordNumber(currRecNr + recordCount);
		break;
	case 2:
		if (recordCount > 0)
			throw "You cant move foreword from the start of the file";
		UpdatePlaceByRecordNumber(fileDesc.eofRecNr + recordCount);
		break;
	default:
		throw "The starting point you entered is invalid";
	}
	//flushFile();
}
int FCB::UpdatePlaceByRecordNumber(int num)
{
	flushFile();
	if (num<0)
		throw "File Out Of Range";
	if (num>fileDesc.eofRecNr)
		throw "File Out Of Range";
	currRecNr = num;
	currSecNr = GetSectorNumberByIndex((num / ((int)(1020 / fileDesc.maxRecSize)) + 1));
	d->readSector(currSecNr,&Buffer);
	currRecNrInBuff = num % ((int)(1020 / fileDesc.maxRecSize));

}
int FCB::GetSectorNumberByIndex(int num)
{
	int count = 0;
	for (int i = 0; i < 1600; i++)
		if (FAT[i])
		{

			if (count++ == num)
				return i * 2;
			if (count++ == num)
				return i * 2 + 1;
		}
	throw "File Out Of Range";
}
#pragma region Edit Mode Functions
void FCB::updateCancel()
{
	if (IOstatus == "I")
		throw "This file has been opened in read only status";
	if (!editLock)
		throw "You can't cancel an update, cause it isn't in update state";
	editLock = false;
}
void FCB::deleteRecord()
{
	if (IOstatus == "I")
		throw "This file has been opened in read only status";
	if (!editLock)
		throw "You can't delete the current record since it's not in update state";
	editLock = false;
	for (int i = fileDesc.keyOffset; i < fileDesc.keyOffset + fileDesc.keySize; i++)
		Buffer[currRecNrInBuff*fileDesc.maxRecSize+i] = 0;
	flushFile();
	UpdatePlaceByRecordNumber(currRecNr + 1);

}
void FCB::updateRecord(char *update)
{
	if (IOstatus == "I")
		throw "This file has been opened in read only status";
	if (!editLock)
		throw "You are not in edit mode so you can't update the current record";
	editLock = false;
	write(update);
}
#pragma endregion
void FCB::addRecord(char *record)//need to be d
{
	flushFile();
	seek(2, 0);//go to the last record
	//check if this is the last record in the sector
		//fileDesc.eofRecNr++;
		//seek(2, 0);//go to end
		write(record);

}

string& FCB::GetLastErrorMessage()
{
	return this->lastErrorMessage;
}

void FCB::SetLastErrorMessage(string lastErrorMessage)
{
	this->lastErrorMessage = lastErrorMessage;
}