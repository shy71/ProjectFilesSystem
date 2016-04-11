#include "FCB.h"


FCB::FCB()
{
	d = NULL;
	editLock = false;
	currRecNr = currRecNrInBuff = currSecNr = -1;
}
FCB::FCB(Disk *disk)
{
	d = disk;
	editLock = false;
	currRecNr = currRecNrInBuff = currSecNr = -1;
}
FCB::~FCB()
{
	delete d;
}
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
			editLock = true;
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
			editLock = true;
	}
}