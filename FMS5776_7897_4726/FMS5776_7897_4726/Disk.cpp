#include "Disk.h"



//
//
//Disk::~Disk()
//{
//}

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
		vhd = *reinterpret_cast<VolumeHeader*>(buffer);
		seekToSector(vhd.addrDAT);
		dat = *reinterpret_cast<DAT*>(buffer);
		seekToSector(vhd.addrRootDir);
		rootDir = *reinterpret_cast<RootDir*>(buffer);
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
	//DAT defulat const
}
Disk::~Disk()
{
	if (mounted)
		unmountDisk();
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
void Disk::recreateDisk(string &diskOwner)
{
	if (mounted == false)
	{
		//dskfl=(*getdskfl());
		//where does the address come from???
	}
}
fstream* Disk::getdskfl()
{
	return &dskfl;
}
void Disk::seekToSector(unsigned int)
{

}
void Disk::writeSector(unsigned int, Sector*)
{

}
void Disk::wirteSector(Sector*)
{

}
void Disk::readSector(int, Sector*)
{}
void Disk::readSector(Sector*)
{}
void Disk::writeBuffer(unsigned int)
{}