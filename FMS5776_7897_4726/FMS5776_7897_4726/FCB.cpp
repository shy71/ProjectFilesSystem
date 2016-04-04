#include "FCB.h"


FCB::FCB()
{
	d = NULL;
	
	currRecNr = currRecNrInBuff = currSecNr = -1; 
}
FCB::FCB(Disk *disk)
{
	d = disk;
	currRecNr = currRecNrInBuff = currSecNr = -1;
}
FCB::~FCB()
{
	delete d;
}
void FCB::flushFile()
{
	*d->rootDir.getEntry(fileDesc.Filename)=fileDesc;
	d->currDiskSectorNr = currSecNr;
	if (d->currDiskSectorNr > 0 && d->currDiskSectorNr < 3200)
		d->writeSector(d->currDiskSectorNr, &Buffer);
	d->Update();
}
void FCB::closeFile()//eof record is changed every edit of a record so it's dealt with already
{
	flushFile();
}