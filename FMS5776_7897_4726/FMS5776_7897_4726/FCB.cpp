//#include "FCB.h"
//
//
//FCB::FCB()
//{
//	d = NULL;
//	
//	currRecNr = currRecNrInBuff = currSecNr = -1; 
//}
//FCB::FCB(Disk *disk)
//{
//	d = disk;
//	currRecNr = currRecNrInBuff = currSecNr = -1;
//}
//FCB::~FCB()
//{
//	delete d;
//}
//void FCB::flushFile()
//{
//	*d->rootDir.getEntry(fileDesc.Filename)=fileDesc;
//	d->currDiskSectorNr = currSecNr;
//	if (d->currDiskSectorNr > 0 && d->currDiskSectorNr < 3200)
//		d->writeSector(d->currDiskSectorNr, &Buffer);
//	d->Update();
//}
//void FCB::closeFile()//eof record is changed every edit of a record so it's dealt with already
//{
//	flushFile();
//	d = NULL;
//	currRecNr = currRecNrInBuff = currSecNr = -1;
//	FAT.reset();
//}
//void FCB::read(char *dest, unsigned int status)//finish function
//{
//	if (status == 0) // read only
//	{
//		if (fileDesc.recFormat == "F")
//		{
//			d->readSector(currSecNr, &Buffer);
//			//read from buffer the current record
//			dest = new char[fileDesc.actualRecSize + 1];
//			for (int i = fileDesc.actualRecSize*currRecNrInBuff; i < fileDesc.actualRecSize; i++)
//				dest[i] = Buffer[i];
//			dest[fileDesc.actualRecSize] = NULL;
//		}
//		else
//		{
//			//deal with different sizes of records
//		}
//	}
//	else // read & write
//	{
//
//	}
//}