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
		vhd = reinterpret_cast<VolumeHeader&>(buffer);
		seekToSector(vhd.addrDAT);
		dat = reinterpret_cast<DAT&>(buffer);
		seekToSector(vhd.addrRootDir);
		rootDir = reinterpret_cast<RootDir&>(buffer);
	}
	else
		throw "The disk couldn't be mounted since it doesn't exist";
}

void Disk::unmountDisk()
{
	Sector vhds, dats, rootDirs;
	strcpy(vhds.RawData, reinterpret_cast<char*>(&vhd));
	writeSector(0, &vhds);
	strcpy(dats.RawData, reinterpret_cast<char*>(&dat));
	writeSector(0, &dats);
	strcpy(rootDirs.RawData, reinterpret_cast<char*>(&rootDir));
	writeSector(0, &rootDirs);
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
void Disk::createDisk(string & filename, string & owner)//FIX
{
	ifstream file(filename);
	if (file.good)
		throw "You can't create a disk with a name that is already taken!";
	ofstream ofile(filename);//error
}
void Disk::recreateDisk(string &diskOwner)
{
	if (mounted == false)
	{
		//dskfl=(*getdskfl());
		//where does the address come from???
	}
}
fstream* getdskfl()
{

}