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
	delete d;
}
#pragma endregion
void FCB::flushFile()
{
	if (IOstatus == "I")
		return;
	if (editLock)
		throw "You can't flush the buffer while locked in edit mode!";
	*d->rootDir.getEntry(fileDesc.Filename) = fileDesc;
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
	FAT.reset();
}
void FCB::read(char *dest, unsigned int status)//finish function
{
	if (editLock)
		throw "You can't read while locked in edit mode!";
	flushFile();
	d->readSector(currSecNr, &Buffer);
	if (fileDesc.recFormat == "F")
	{
		//read from buffer the current record
		dest = new char[fileDesc.actualRecSize];
		for (int i = fileDesc.actualRecSize*currRecNrInBuff; i < fileDesc.actualRecSize*(currRecNrInBuff + 1); i++)
			dest[i] = Buffer[i];
		if (status == 0)
			seek(1, 1);
		else
		{
			if (IOstatus == "I")
				throw "You can't open it for editing since it's read only.";
			editLock = true;
		}
	}
	else//varying size of records
	{
		int index = 0;
		for (int i = 0; i < currRecNrInBuff; i++)
		{
			for (; Buffer[index] != '~'; index++);
		}
		index++;//to pass the ~
		int size = 0;
		for (int i = index; Buffer[i] != '~'; i++, size++);
		dest = new char[size];
		for (int i = index; i < size; i++)
			dest[i] = Buffer[i];
		//deal with a situation where he put in ~ in the data or eliminate that option from the user
		//or leave that for the higher levels to deal with
		if (status == 0)//read only
			GoToNextRecord();
		else // read & write
		{
			if (IOstatus == "I")
				throw "You can't open it for editing since it's read only.";
			editLock = true;
		}
	}
}
void FCB::write(char *record)
{
	if (editLock)
		throw "You can't write while locked in edit mode!";
	if (IOstatus == "I")
		throw "You can't edit these files because the file is read only.";
	flushFile();
	if (fileDesc.recFormat == "F")
	{
		if (strlen(record) > fileDesc.maxRecSize)
			throw "The record length isn't the right size.";
		for (int i = 0; i < strlen(record); i++)
			Buffer[currRecNrInBuff*fileDesc.maxRecSize + i] = record[i];
		for (int i = strlen(record); i < fileDesc.maxRecSize; i++)
			Buffer[currRecNrInBuff*fileDesc.maxRecSize + i] = NULL;
	}
	else //the file has varrying sized records
	{
		int startOfRecord = 0;
		for (int i = 0; startOfRecord < 1020 && i < currRecNrInBuff;startOfRecord++)
			if (Buffer[startOfRecord] == '~')
			{
				i++;
				startOfRecord++;
			}
		if (startOfRecord < 1020)
		{
			int size;
			for (size = 0; Buffer[size + startOfRecord] != '~' && size + startOfRecord < 1020; size++);
			if (startOfRecord + size >= 1020)
				throw "The record is too large";
			if (size + startOfRecord + 1 < 1020 && Buffer[size + startOfRecord + 1] == NULL)//it's the last record
				if (strlen(record) < 1020 - startOfRecord)
					for (int i = 0; i < strlen(record); i++)
						Buffer[startOfRecord + i] = record[i];
			else if (strlen(record) < size)
			{
				for (int i = 0; i < strlen(record); i++)
					Buffer[startOfRecord + i] = record[i];
				//pull everything back
				for (int i = startOfRecord + size, j = startOfRecord + strlen(record); j < 1020; i++, j++)
				{
					if (i < 1020)
						Buffer[j] = Buffer[i];
					else
						Buffer[j] = NULL;
				}
			}
			else //it's the right size
			{
				for (int i = startOfRecord; i < size; i++)
					Buffer[i] = record[i];
			}
		}
	}
}


//NOT EDITED 
void FCB::seek(unsigned int from, int recordCount)//check for situation where he gave too big recordcount
{
	//deal with deleted files problem
	flushFile();
	switch (from)
	{
	case 0://from beginning
		if (fileDesc.recFormat == "F")
		{
			if (recordCount < 0)
				throw "You are trying to access area before the begining of the file";
			currRecNr = recordCount % (fileDesc.eofRecNr + 1);
			currRecNrInBuff = 0;
			currSecNr = -1;
			for (int i = 0; i < 3200; i++)//get first sector
				if (FAT[i/2])
				{
					currSecNr = i;
					break;
				}
			while (recordCount > 1020 / fileDesc.maxRecSize)
			{
				recordCount -= 1020 / fileDesc.maxRecSize;
				for (int i = currSecNr + 1; i < 1600; i++)
					if (FAT[i / 2])
					{
						currSecNr = i;
						break;
					}
			}
			currRecNrInBuff = recordCount;
		}
		else
		{
			if (recordCount == 0)
			{
				currRecNr = currRecNrInBuff = 0;
				for (int i = 0; i < 3200; i++)
				{
					if (FAT[i])
					{
						currSecNr = i+1;//the second sector in the cluster since the first is being used for the header
						break;
					}
				}
			}
			else
				throw "You can't jump there since it's the sizes of records vary";
		}
		break;
	case 1:
		currRecNr += recordCount;
		currRecNr %= (fileDesc.eofRecNr + 1);
		if (fileDesc.recFormat == "F")
		{
			if (currRecNrInBuff + recordCount < 1020 / fileDesc.maxRecSize)
				currRecNrInBuff += recordCount;
			else
			{
				recordCount -= (1020 / fileDesc.maxRecSize - currRecNrInBuff);
				while (recordCount > 1020 / fileDesc.maxRecSize)
				{
					recordCount -= 1020 / fileDesc.maxRecSize;
					for (int i = currSecNr + 1; i < 1600; i++)
						if (FAT[i / 2])
						{
							currSecNr = i;
							break;
						}
				}
				currRecNrInBuff = recordCount;
			}
		}
		else
			throw "You can't jump there since it's the sizes of records vary";
		break;
	case 2:
		if (fileDesc.recFormat == "F")
		{
			if (recordCount > 0)
				throw "You are trying to access area before the begining of the file";
			currRecNr = (fileDesc.eofRecNr + recordCount ) % (fileDesc.eofRecNr + 1);;
			currSecNr = -1;
			for (int i = 0; i < 1600; i++)//get last sector
				if (FAT[i])
					currSecNr = 2 * i + 1;
			while (-recordCount > 1020 / fileDesc.maxRecSize)
			{
				recordCount += 1020 / fileDesc.maxRecSize;
				for (int i = currSecNr - 1; i >= 0; i++)
					if (FAT[i / 2])
						currSecNr = i;
			}
			currRecNrInBuff = currRecNr % (1020 / fileDesc.maxRecSize);
		}
		else
		{
			if (recordCount == 0)
			{
				currRecNr = fileDesc.eofRecNr;
				for (int i = 0; i < 1600; i++)
					if (FAT[i])
						currSecNr = i * 2 + 1;//the last sector
				flushFile();
				currRecNrInBuff = -1;
				for (int i = 0; i < 1020; i++)
				{
					if (Buffer[i] == '~')
						currRecNrInBuff++;
				}
			}
			else
				throw "You can't jump there since it's the sizes of records vary";
		}
		break;
	default:
		throw "The starting point you entered is invalid";
	}
	flushFile();
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
		Buffer[i] = 0;
	GoToNextRecord();
}
void FCB::updateRecord(char *update)
{
	if (IOstatus == "I")
		throw "This file has been opened in read only status";
	if (!editLock)
		throw "You are not in edit mode so you can't update the current record";
	editLock = false;
	write(update);
	GoToNextRecord();
}
#pragma endregion
void FCB::GoToNextRecord()
{
	if (fileDesc.recFormat == "F")
	{
		seek(1, 1);
	}
	else
	{
		int index = 0;//index of begining of the next sector
		for (int i = 0; i < currRecNrInBuff + 1; i++)
		{
			for (; Buffer[index] != '~'; index++);
		}
		index++;//to pass the ~
		int size = 0;
		for (int i = index; Buffer[i] != '~' && size + index <1020; i++, size++);
		if (size + index < 1020)
		{
			currRecNr++;
			currRecNrInBuff++;
			for (int i = 0; i < fileDesc.keySize; i++)
				if (Buffer[i + index + fileDesc.keyOffset])
					return;
			//go to next cause this one is erased
			GoToNextRecord();
		}
		else // the record is in the next sector
		{
			while (!FAT[currSecNr])
			{
				currSecNr++;
				currSecNr %= 3200;
			}
			currRecNr = 0;
			currRecNrInBuff = 0;
			//check if it's an erased record
			for (int i = 0; i < fileDesc.keySize; i++)
				if (Buffer[i +fileDesc.keyOffset])
					return;
			//go to next cause this one is erased
			GoToNextRecord();
		}
	}
}