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
	if (fileDesc.recFormat == "F")
	{
		//read from buffer the current record
		dest = new char[fileDesc.actualRecSize];
		for (int i = fileDesc.actualRecSize*currRecNrInBuff; i < fileDesc.actualRecSize*(currRecNrInBuff + 1); i++)//think of changing
			dest[i] = Buffer[i];
		if (status == 0)
			GoToNextRecord();
		else
			editLock = true;
	}
	//else//varying size of records
	//{
	//	int index = 0;
	//	for (int i = 0; i < currRecNrInBuff; i++)
	//	{
	//		for (; Buffer[index] != '~'; index++);
	//	}
	//	index++;//to pass the ~
	//	int size = 0;
	//	for (int i = index; Buffer[i] != '~'; i++, size++);
	//	dest = new char[size];
	//	for (int i = index; i < size; i++)
	//		dest[i] = Buffer[i];
	//	//deal with a situation where he put in ~ in the data or eliminate that option from the user
	//	//or leave that for the higher levels to deal with
	//	if (status == 0)//read only
	//		GoToNextRecord();
	//	else // read & write
	//	{
	//		if (IOstatus == "I")
	//			throw "You can't open it for editing since it's read only.";
	//		editLock = true;
	//	}
	//}
}
void FCB::write(char *record)
{
	if (editLock)
		throw "You can't write while locked in edit mode!";
	if (IOstatus == "I")
		throw "You can't edit these files because the file is read only.";
	//flushFile();
	if (fileDesc.recFormat == "F")
	{
		for (int i = 0; i < fileDesc.maxRecSize; i++)
			Buffer[currRecNrInBuff*fileDesc.maxRecSize + i] = record[i];
		if (currRecNr > fileDesc.eofRecNr)
			fileDesc.eofRecNr=currRecNr;
	}
	flushFile();
	GoToNextRecord();
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
		currRecNr = currRecNrInBuff = 0;
		for (int i = 0; i < 3200 && !FAT[i / 2]; i++)
			currSecNr = i;
		currSecNr++;
		for (int i = 0; i < recordCount; i++)
			GoToNextRecord();
		/*if (fileDesc.recFormat == "F")
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
		break;*/
		break;
	case 1:
		/*currRecNr += recordCount;
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
		break;*/
		for (int i = 0; i < recordCount; i++)
			GoToNextRecord();
		break;
	case 2:

		if (recordCount > 0)
			throw "You are trying to access area not yet in use";
		for (int i = 0; i < fileDesc.eofRecNr - recordCount; i++)
			GoToNextRecord();
		/*
		if (fileDesc.recFormat == "F")
		{
			currRecNr = (fileDesc.eofRecNr + recordCount ) % (fileDesc.eofRecNr + 1);
			int Secindex = 0;
			for (; Secindex < 3200 && !FAT[Secindex / 2]; Secindex++)
				Secindex++;
			if (Secindex >= 3200)
				throw "There isn't any space";
			for (int i = 0; i < ((fileDesc.eofRecNr - 1) / (1020 / fileDesc.maxRecSize)) + 1 && Secindex < 3200;Secindex++)
				if (FAT[Secindex / 2])
					i++;
			currSecNr = Secindex;
			
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
		*/
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
	if (FAT.count() == 0)
		throw "You have no space so you can't move to the next record...";
	if (fileDesc.recFormat == "F")
	{
		if (currRecNrInBuff < 1020 / fileDesc.maxRecSize - 1)//In next sector
		{
			currRecNr++;
			currRecNrInBuff++;
		}
		else
		{
			bool lastSector = true;
			for (int i = currSecNr + 1; i < 3200; i++)
				if (FAT[i / 2])
					lastSector = false;
			if (lastSector)
			{
				//go back to begining
				for (int i = 0; i < 3200 && !FAT[i / 2]; i++)
					currSecNr;
				currSecNr++;
				currRecNr = currRecNrInBuff = 0;
			}
			else
			{
				currRecNr++;
				for (int i = currSecNr + 1; i < 3200 && !FAT[i / 2]; i++)
					currSecNr = i;
				currRecNrInBuff = 0;
			}
		}
	}
	//else // varying sizes
	//{
	//	if (currRecNr != fileDesc.eofRecNr)
	//	{
	//		int index = 0;
	//		for (int i = 0; index < 1020 && i < currRecNrInBuff; index++)
	//		{
	//			if (Buffer[i] == '~')
	//				i++;
	//		}
	//		if (index + 1 == 1020 || Buffer[index + 1] == NULL)//in next sector
	//		{
	//			for (int i = currSecNr + 1; i < 3200 && !FAT[i / 2]; i++)
	//				currSecNr = i;
	//			currSecNr++;
	//			currRecNr++;
	//			currRecNrInBuff = 0;
	//		}
	//		else
	//		{
	//			currRecNr++; 
	//			currRecNrInBuff++;
	//		}
	//	}
	//	else//it's the last record
	//	{
	//		//go back to begining
	//		for (int i = 0; i < 3200 && !FAT[i / 2]; i++)
	//			currSecNr;
	//		currSecNr++;
	//		currRecNr = currRecNrInBuff = 0;
	//	}
	//}
}
void FCB::addRecord(char *record)
{
	flushFile();
	seek(2, 0);//go to the last record
	//check if this is the last record in the sector
	bool isLast = true;
	if (fileDesc.recFormat == "F" && currRecNrInBuff < 1020 / fileDesc.maxRecSize - 1)
		isLast = false;
	else if (fileDesc.recFormat != "F")
	{
		int index = 0;
		for (int i = 0; i < currRecNrInBuff && index < 1020; index++)
			if (Buffer[index] == '~')
				i++;
		if(currRecNrInBuff > 0)
			index++;
		if (1019 - index < strlen(record))
		{
			isLast = false;
			Buffer[1019] = '~';
		}
	}
	if (!isLast)//isn't last
	{	
		fileDesc.eofRecNr++;
		seek(2, 0);//go to end
		write(record);
	}
	else // is the last
	{
		//check if there is another sector afterwards
		bool nextSector = false;
		for (int i = currSecNr + 1; i < 3200; i++)
			if (FAT[i / 2])
			{
				nextSector = true;
				break;
			}
		if (nextSector)
		{				
			fileDesc.eofRecNr++;
			GoToNextRecord();
			write(record);
		}
		else
			throw "No more space to add another record";
	}
}