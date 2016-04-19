#include "FCB.h"
#pragma region Constructors_Destructor
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
	if (fileDesc.recFormat == "F")
	{
		d->readSector(currSecNr, &Buffer);
		//read from buffer the current record
		dest = new char[fileDesc.actualRecSize + 1];
		for (int i = fileDesc.actualRecSize*currRecNrInBuff; i < fileDesc.actualRecSize; i++)
			dest[i] = Buffer[i];
		dest[fileDesc.actualRecSize] = NULL;
		if (status == 0)
		{
			if (currRecNr < fileDesc.eofRecNr - 1)
			{
				if (currRecNrInBuff < 1020 / fileDesc.maxRecSize)//check if it's before the end or at the end of the sector
				{
					currRecNr++;
					currRecNrInBuff++;
				}
				else
				{
					for (int i = currSecNr + 1; i < 1600; i++)
						if (FAT[i])
							currSecNr = i * 2 + 1;
					currRecNr++;
					currRecNrInBuff = 0;
				}
			}
			else
			{
				currRecNr = currRecNrInBuff = 0;
				for (int i = 0; i < 1600; i++)
					if (FAT[i])
						currSecNr = i * 2 + 1;
			}
		}
		else
		{
			if (IOstatus == "I")
				throw "You can't open it for editing since it's read only.";
			editLock = true;
		}
	}
	else//גודל משתנה
	{
		int index = 0;
		for (int i = 0; i < currRecNrInBuff; i++)
		{
			for (; Buffer[index] != '~'; index++);
		}
		index++;//to pass the ~
		int size = 0;
		for (int i = index; Buffer[i] != '~'; i++, size++);
		dest = new char[size + 1];
		for (int i = index; i < size; i++)
			dest[i] = Buffer[i];
		dest[size] = NULL;
		//deal with a situation where bhe put in ~ in the data
		if (status == 0)//read only
		{
			if (currRecNr < fileDesc.eofRecNr - 1)
			{
				if (index + size < 1020)//check if it's before the end or at the end of the sector
				{
					currRecNr++;
					currRecNrInBuff++;
				}
				else
				{
					for (int i = currSecNr + 1; i < 1600; i++)
						if (FAT[i])
							currSecNr = i * 2 + 1;
					currRecNr++;
					currRecNrInBuff = 0;
				}
			}
			else
			{
				currRecNr = currRecNrInBuff = 0;
				for (int i = 0; i < 1600; i++)
					if (FAT[i])
						currSecNr = i * 2 + 1;
			}
		}
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
		if (strlen(record) != fileDesc.maxRecSize)
			throw "The record length isn't the right size.";
		for (int i = 0; i < fileDesc.maxRecSize; i++)
			Buffer[currRecNrInBuff*fileDesc.maxRecSize + i] = record[i];
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
			if (size < strlen(record) || startOfRecord + size >= 1020)
			{
				throw "The record is too large";
			}
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
	fileDesc.eofRecNr++;
}
void FCB::sync(unsigned int from, int recordCount)//check for situation where he gave too big recordcount
{
	//deal with deleted files problem
	switch (from)
	{
	case 0://from beginning
		if (fileDesc.recFormat == "F")
		{
			if (recordCount < 0)
				throw "You are trying to access area before the begining of the file";
			currRecNr = recordCount;
			currRecNrInBuff = 0;
			currSecNr = -1;
			for (int i = 0; i < 1600; i++)//get first sector
				if (FAT[i])
				{
					currSecNr = i * 2 + 1;
					break;
				}
			while (recordCount > 1020 / fileDesc.maxRecSize)
			{
				recordCount -= 1020 / fileDesc.maxRecSize;
				for (int i = currSecNr + 1; i < 1600; i++)
					if (FAT[i/2])
						currSecNr = i;
			}
			currRecNrInBuff = recordCount;
		}
		else
		{
			if (recordCount == 0)
			{
				currRecNr = currRecNrInBuff = 0;
				for (int i = 0; i < 1600; i++)
				{
					if (FAT[i])
					{
						currSecNr = i * 2 + 1;//the second sector in the cluster since the first is being used for the header
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
							currSecNr = i;
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
			currRecNr = fileDesc.eofRecNr + recordCount;
			currSecNr = -1;
			for (int i = 0; i < 1600; i++)//get last sector
				if (FAT[i])
					currSecNr = i * 2 + 1;
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
