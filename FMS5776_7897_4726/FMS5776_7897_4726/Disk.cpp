#include "Disk.h"
#include<iostream>
#include<sstream>

Disk::Disk(string &fname, string &diskOwner, char action)
{
    ifstream infile(fname);
	if (action == 'c')//create new disk
	{
		if (infile.is_open())
			throw "There already is a file with that name";
		createDisk(fname, diskOwner);
		mountDisk(fname);
	}
	else if (action == 'm')
	{
		if (infile.is_open())
			mountDisk(fname);
		else
			throw "There is no disk with that name...";
	}
}

void Disk::mountDisk(string &fname)
{
	dskfl.open(fname, ios::binary | ios::in);
	if (dskfl.is_open())
	{
		readSector(0, (Sector*)(&vhd));
		readSector(vhd.addrDAT, (Sector*)(&dat));
		readSector(vhd.addrRootDir, (Sector*)(&rootDir));
		mounted = true;
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
	if (currDiskSectorNr > 0 && currDiskSectorNr < 3200)
		writeSector(currDiskSectorNr, (Sector*)buffer);//&buffer?
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
	if (dskfl.is_open())
		dskfl.close();
}
void Disk::createDisk(string & name, string & owner)//FIX
{
	ifstream file(name,ios::binary | ios::in);
	if (file.is_open())
		throw "You can't create a disk with a name that is already taken!";
	file.close();
	dskfl.open(name, ios::binary | ios::out);//error
	//create VHD Data
	time_t t = time(0);   // get time now
	struct tm timeinfo;
	localtime_s(&timeinfo, &t);
	vhd.sectorNr = 0;
	strcpy_s(vhd.diskName, name.c_str());
	strcpy_s(vhd.diskOwner, owner.c_str());
	stringstream temp;
	temp << timeinfo.tm_mday << "/" << timeinfo.tm_mon + 1 << "/" << timeinfo.tm_year + 1900;
	strcpy_s(vhd.prodDate, temp.str().c_str());
	vhd.ClusQty = 1600;
	vhd.dataClusQty = 1596;
	vhd.addrDAT = 1;
	vhd.addrRootDir = 1;
	vhd.addrDATcpy = 4;
	vhd.addrRootDirCpy = 3;
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
	dskfl.close();
	dskfl.open(name, ios::binary | ios::out | ios::in);
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
void Disk::format(string & name)
{
	if (vhd.diskName == name.c_str());
}
int Disk::howmuchempty()
{
	return dat.Dat.count();
}
void Disk::dealloc(DATtype &FAT)
{
	dat.Dat |= FAT;
	FAT.reset();
}