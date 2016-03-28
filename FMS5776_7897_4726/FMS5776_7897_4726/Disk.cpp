#include "Disk.h"
#include"Dir.h"
#include<iostream>
#include<ctime>
#include<Windows.h>
#include<string>
#include<string.h>
#include<sstream>
using namespace std;

string GetTime()
{
	time_t t = time(0);   // get time now
	struct tm timeinfo;
	localtime_s(&timeinfo, &t);
	stringstream temp;
	temp << timeinfo.tm_mday << "/" << timeinfo.tm_mon + 1 << "/" << timeinfo.tm_year + 1900;
	return temp.str();
}
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
	writePlusCpy(vhd.addrDAT, vhd.addrDATcpy, dat);
	writeDir(vhd.addrRootDir, vhd.addrRootDirCpy, rootDir);
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
	ifstream file(name, ios::binary | ios::in);
	if (file.is_open())
		throw "You can't create a disk with a name that is already taken!";
	file.close();
	dskfl.open(name, ios::binary | ios::out);//error
	//create VHD Data
	vhd.sectorNr = 0;
	strcpy_s(vhd.diskName, name.c_str());
	strcpy_s(vhd.diskOwner, owner.c_str());
	strcpy_s(vhd.prodDate, GetTime().c_str());
	vhd.ClusQty = 1600;
	vhd.dataClusQty = 1596;
	vhd.addrDAT = 1;
	vhd.addrRootDir = 1;
	vhd.addrDATcpy = 4;
	vhd.addrRootDirCpy = 3;
	vhd.addrDataStart = 8;
	vhd.isFormated = false;
	writeSector(0, (Sector*)(&vhd));
	//create RootDir data
	rootDir.SetClus(vhd.addrRootDir);
	writeDir(vhd.addrRootDir, vhd.addrRootDirCpy, rootDir);
	dat.Dat[vhd.addrRootDir].flip();
	dat.Dat[vhd.addrRootDirCpy].flip();
	dat.Dat[0].flip();
	//Create Dat Data
	dat.Dat[(int)vhd.addrDATcpy / 2].flip();
	writePlusCpy(vhd.addrDAT, vhd.addrDATcpy, dat);
	Sector sec;
	for (int i = 0; i < 1600; i++)
		if (dat.Dat[i])
		{
			sec.sectorNr = i * 2;
			writeSector(i*2, &sec);
			sec.sectorNr = i * 2 + 1;
			writeSector(i * 2 + 1, &sec);
		}
	dskfl.close();
	dskfl.open(name, ios::binary | ios::out | ios::in);
	unmountDisk();
}
void Disk::writeDir(unsigned int add, unsigned int addcpy, RootDir root)
{
	writeSector(add * 2, (Sector*)&root.lsbSector);
	root.lsbSector.sectorNr = addcpy * 2;
	writeSector(addcpy * 2, (Sector*)&root.lsbSector);
	writeSector(add * 2 + 1, (Sector*)&root.msbSector);
	root.msbSector.sectorNr = addcpy * 2 + 1;
	writeSector(addcpy * 2 + 1, (Sector*)&root.msbSector);
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
void Disk::format(string & owner)
{
	if (vhd.diskOwner != owner.c_str())
		throw "You can't format a disk which not belongs to you!";
	dat.Dat.set();
	writePlusCpy(vhd.addrDAT, vhd.addrDATcpy,dat);
	rootDir.clear();
	writeDir(vhd.addrRootDir, vhd.addrRootDirCpy, rootDir);
	vhd.isFormated = true;//when set off?
	strcpy_s(vhd.formatDate, GetTime().c_str());
	writeSector(0, (Sector *)&vhd);
}
void Disk::writePlusCpy(unsigned int sor, unsigned int cpy, DAT sec)
{
	writeSector(sor,(Sector*)&sec);
	sec.sectorNr = cpy;
	writeSector(cpy, (Sector*)&sec);
	Sector sec2;
	sec2.sectorNr = cpy + 1;
	writeSector(cpy + 1, &sec2);
}
void Disk::alloc(DATtype & fat, unsigned int num, unsigned int type, unsigned int index)
{
	DATtype UsedData;
	try
	{
		UsedData.reset();
		int j = 0;
		if (type == 0)
		{
			for (int i = index; i < 3200; i++)
			{
				if (dat.Dat[i])
				{
					for (j = 1; j < num - 1 && i + j < 3200; j++)
					{
						if (!dat.Dat[i + j])
							break;
						if (j + 1 == num - 1)
						{
							for (int k = i; k < i + j; k++)
							{
								dat.Dat[k].flip();//to change into boolen opertor
								fat[k].flip();
								UsedData[k].flip();
								return;
							}
						}
					}
					i += j;
				}
			}
			if (num == 1)
				throw "doesn't have enough space!";

			alloc(fat, num / 2 + (num % 2), type,index);
			alloc(fat, num / 2, type,index);//need to handle when the first one works but the second one doesn't!!!
		}
	}
	catch (char *error)
	{
		dat.Dat |= UsedData;
		fat &= ~UsedData;
		/*for (int i = 0; i < 3200; i++)
		{
			if (UsedData[i])
			{
				fat[i].flip();
				dat.Dat[i].flip();
			}
		}*/
		throw error;
	}
}
void Disk::allocextend(DATtype &fat, unsigned int num, unsigned int type)
{
	int index = 0;
	for (int i = 0; i < 3200; i++)
	{
		if (fat[i])
			index = i;
	}
	alloc(fat, num, type,index);
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
int Disk::howmuchused()
{
	return vhd.ClusQty - howmuchempty();
}
