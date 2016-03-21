#include "Disk.h"


Disk::Disk(string &fname, string &diskOwner, char action)
{
    ifstream infile(fname);
	if (action == 'c')//create new disk
	{
		if (infile.good())
			throw "There already is a file with that name";
		createDisk(fname, diskOwner);
		mountDisk(fname);
	}
	else if (action == 'm')
	{
		if (infile.good())
			mountDisk(fname);
		else
			throw "There is no disk with that name...";
	}
}

void Disk::mountDisk(string &fname)
{
	dskfl.open(fname, ios::binary | ios::in);
	if (dskfl.good())
	{
		mounted = true;
		seekToSector(0);
		vhd = *(VolumeHeader*)(buffer);
		seekToSector(vhd.addrDAT);
		dat = *(DAT*)(buffer);
		seekToSector(vhd.addrRootDir);
		rootDir = *(RootDir*)(buffer);
	}
	else
		throw "The disk couldn't be mounted since it doesn't exist";
}

void Disk::unmountDisk()
{
	writeSector(0, (Sector*)(&vhd));
	writeSector(vhd.addrDAT, (Sector*)(&dat));
	writeSector(vhd.addrDATcpy, (Sector*)(&dat));
	writeSector(vhd.addrRootDir, (Sector*)(&rootDir));
	writeSector(vhd.addrRootDirCpy, (Sector*)(&rootDir));
	dskfl.close();
	mounted = false;
}
Disk::Disk()
{
	mounted = false;
}
Disk::~Disk()
{
	if (mounted)
		unmountDisk();
	if (dskfl.good())
		dskfl.close();
}
void Disk::createDisk(string & name, string & owner)//FIX
{
	ifstream file(name,ios::binary | ios::in);
	if (file.good())
		throw "You can't create a disk with a name that is already taken!";
	dskfl.open(name, ios::binary | ios::out);//error
	//create VHD Data
	time_t t = time(0);   // get time now
	struct tm timeinfo;
	localtime_s(&timeinfo, &t);
	vhd.sectorNr = 0;
	strcpy_s(vhd.diskName, name.c_str());
	strcpy_s(vhd.diskOwner, owner.c_str());
	strcpy_s(vhd.prodDate, ((char*)timeinfo.tm_mday + '/' + timeinfo.tm_mon + '/' + timeinfo.tm_year));
	vhd.ClusQty = 1600;
	vhd.dataClusQty = 1596;
	vhd.addrDAT = 1;
	vhd.addrRootDir = 1;
	vhd.addrDATcpy = 4;
	vhd.addrRootDir = 3;
	vhd.addrDataStart = 8;
	vhd.isFormated = false;
	writeSector(0,(Sector*)(&vhd));
	//create RootDir data
	rootDir.SetClus(vhd.addrRootDir);
	writeSector(vhd.addrRootDir * 2, (Sector*)&rootDir.lsbSector);
	writeSector(vhd.addrRootDirCpy * 2, (Sector*)&rootDir.lsbSector);
	writeSector(vhd.addrRootDir * 2 + 1, (Sector*)&rootDir.msbSector);
	writeSector(vhd.addrRootDirCpy * 2 + 1, (Sector*)&rootDir.msbSector);
	dat.Dat[vhd.addrRootDir].flip();
	dat.Dat[vhd.addrRootDirCpy].flip();
	dat.Dat[0].flip();
	//Create Dat Data
	writeSector(vhd.addrDAT, (Sector*)&dat);
	writeSector(vhd.addrDATcpy, (Sector*)&dat);
	dat.Dat[(int)vhd.addrDATcpy / 2].flip();

	Sector sec;
	for (int i = 0; i < 1600; i++)
	{
		if (dat.Dat[i] == 1)
		{
			sec.sectorNr = i * 2;
			writeSector(sec.sectorNr, &sec);
			sec.sectorNr = i * 2 + 1;
			writeSector(sec.sectorNr, &sec);
		}
	}
	unmountDisk();
}
void Disk::recreateDisk(string &owner)
{
	if (mounted == false)
	{
		strcpy_s(vhd.diskOwner, owner.c_str());
		writeSector(0, (Sector*)(&vhd));
		vhd.isFormated = false;
	}
}
fstream* Disk::getdskfl()
{
	return &dskfl;
}
void Disk::seekToSector(unsigned int numofsector)
{
	dskfl.seekp(numofsector*sizeof(Sector), ios::beg);
	dskfl.seekg(numofsector*sizeof(Sector), ios::beg);
}
void Disk::writeSector(unsigned int numofsector, Sector* sec)
{
	seekToSector(numofsector);
	writeSector(sec);
}
void Disk::writeSector(Sector* sec)
{
	dskfl.write((char *)sec, sizeof(Sector));
}
void Disk::readSector(unsigned int numofsector, Sector* sec)
{
	seekToSector(numofsector);
	readSector(sec);
}
void Disk::readSector(Sector* sec)
{
	dskfl.read((char *)sec, sizeof(Sector));
}
