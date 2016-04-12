#include "Disk.h"
#include"Dir.h"
#include"FileHeader.h"
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
		readSector(vhd.addrRootDir*2, (Sector*)(&rootDir.lsbSector));
		readSector(vhd.addrRootDir*2+1, (Sector*)(&rootDir.msbSector));
		mounted = true;
		dskfl.close();
		dskfl.open(fname, ios::binary | ios::out | ios::in);
	}
	else
		throw "The disk couldn't be mounted since it doesn't exist";
}
void Disk::unmountDisk()
{
	writeSector(0, (Sector*)(&vhd));
	writePlusCpy(vhd.addrDAT, vhd.addrDATcpy, dat);
	writeDir(vhd.addrRootDir, vhd.addrRootDirCpy, rootDir);
	//&buffer?
	dskfl.close();
	mounted = false;
}
void Disk::Update()
{
	writeSector(0, (Sector*)(&vhd));
	writePlusCpy(vhd.addrDAT, vhd.addrDATcpy, dat);
	writeDir(vhd.addrRootDir, vhd.addrRootDirCpy, rootDir);
	//&buffer?
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
	if (strcmp(vhd.diskOwner,owner.c_str())!=0)
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
void Disk::alloc(DATtype & fat, unsigned int numofsector, unsigned int type, unsigned int index)
{
	int num = (numofsector / 2) + (numofsector % 2);
	DATtype UsedData=dat.Dat;
	try
	{
		UsedData.reset();
		int j = 0;
		if (type == 0)
		{
			for (int i = index; i < 1600; i++)
			{
				if (dat.Dat[i])
				{
					for (j = 0; j < num && i + j < 1600; j++)
					{
						if (!dat.Dat[i + j])
							break;
						if (j == num - 1)
						{	
							if (vhd.isFormated == true)
							{
								vhd.isFormated = false;
								Update();
							}
							for (int k = i; k <= i + j; k++)
							{
								dat.Dat[k].flip();//to change into boolen opertor
								fat[k].flip();
								UsedData[k].flip();

							}	

							return;
						}
					}
					i += j;
				}
			}
			if (num <=2)
				throw "doesn't have enough space!";
			//try saving the min
			alloc(fat, numofsector-2+(numofsector%2), type, index);
			alloc(fat, 1+(numofsector%2==0)?1:0, type, index);//need to handle when the first one works but the second one doesn't!!!
		}
	}
	catch (char *error)
	{
		dat.Dat = UsedData;
		fat &= ~UsedData;
		throw error;
	}
}
void Disk::allocextend(DATtype &fat, unsigned int num, unsigned int type)
{
	int index = 0;
	for (int i = 0; i < 1600; i++)
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

//Level 2
void Disk::createfile(string fileName, string fileOwner, string filetype, unsigned int recordSize, unsigned int sectorCount, string keyType, unsigned int keyOffset, unsigned int keySize)
{
	if (rootDir.IsFull())
		throw "The disk is full!";
	if (rootDir.getEntry(fileName.c_str()) != NULL)
		throw "A file with that name is already exits";
	FileHeader fheader;
	strcpy_s(fheader.fileDesc.Filename,fileName.c_str());
	strcpy_s(fheader.fileDesc.fileOwner, fileOwner.c_str());
	fheader.fileDesc.maxRecSize = recordSize;
	fheader.FAT.reset();
	strcpy_s(fheader.fileDesc.crDate, GetTime().c_str());
	fheader.fileDesc.entryStatus = 1;
	fheader.fileDesc.keyOffset = keyOffset;
	fheader.fileDesc.keySize = keySize;
	fheader.fileDesc.eofRecNr = 0;
	strcpy_s(fheader.fileDesc.keyType, keyType.c_str());
	if (filetype == "F")
	{
		strcpy_s(fheader.fileDesc.recFormat, "F");
		fheader.fileDesc.actualRecSize = recordSize;
	}
	else
		strcpy_s(fheader.fileDesc.recFormat, "V");
	alloc(fheader.FAT, sectorCount+1, 0);
	for (int i = 0; i<1600; i++)
		if (fheader.FAT[i])
		{
			fheader.fileDesc.fileAddr = i*2;
			break;
		}
	fheader.fileDesc.fileSize = sectorCount;
	fheader.fileDesc.eofRecNr = 0;
	fheader.sectorNr = fheader.fileDesc.fileAddr;
	writeSector(fheader.fileDesc.fileAddr, (Sector*)&fheader);
	//the last record for now is the first but this must change when records are added!!!
	rootDir.WriteEntry(fheader.fileDesc);
	Update();
	//save direntry in the file!!!
	//save the file
}

void Disk::delfile(string & fname, string & username)
{
	dirEntry *dir = rootDir.getEntry(fname.c_str());
	if (dir==NULL)
		throw "The file you asked to delete doesnt exist";
	if (strcmp(dir->fileOwner, username.c_str()) != 0)
		throw "only the file owner can delete his files";
	FileHeader fheader;
	readSector(dir->fileAddr, (Sector *)&fheader);
	dealloc(fheader.FAT);
	dir->entryStatus = 2;

	Update();
}
void Disk::extendfile(string & fname, string & username ,unsigned int numToAdd)
{
	dirEntry *dir = rootDir.getEntry(fname.c_str());
	if (dir == NULL)
		throw "The file you asked to extend doesnt exist";
	if (strcmp(dir->fileOwner, username.c_str()) != 0)
		throw "only the file owner can extend his files";
	FileHeader fheader;
	readSector(dir->fileAddr, (Sector *)&fheader);
	allocextend(fheader.FAT,numToAdd,0);
	dir->fileSize += numToAdd;
	fheader.fileDesc.fileSize = dir->fileSize;
	writeSector(dir->fileAddr, (Sector *)&fheader);

	Update();
}
FCB* Disk::openfile(string &fileName, string &UserName, string &IOstatus)
{
	dirEntry *dir=rootDir.getEntry(fileName.c_str());
	if (dir == NULL)
		throw "The file you are looking for doesn't exist.";
	if ((strcmp(dir->fileOwner, UserName.c_str()) == 0 && IOstatus == "IO") || IOstatus == "I")
	{
		FileHeader fheader;
		readSector(dir->fileAddr, (Sector *)&fheader);
		FCB* fcb = new FCB(this);
		fcb->currRecNr =fcb->currRecNrInBuff = 0;
		fcb->IOstatus = IOstatus;
		fcb->FAT = fheader.FAT;
		fcb->fileDesc = *dir;
		fcb->currSecNr=dir->fileAddr + 1;
		return fcb;
	}
	else
		throw "You don't have access to this file.";
}