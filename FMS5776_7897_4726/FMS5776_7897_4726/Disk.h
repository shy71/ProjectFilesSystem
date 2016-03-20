#pragma once
using namespace std;
#include "VolumeHeader.h"
#include "DAT.h"
#include "Sector.h"
#include "Dir.h"
#include <fstream>

struct Disk
{
	VolumeHeader vhd;
	DAT dat;
	RootDir rootDir;
	bool mounted;
	fstream dskfl;
	unsigned int currDiskSectorNr;

	char buffer[sizeof(Sector)];

public:
	Disk();
	Disk(string &, string &, char);
	~Disk();

	void createDisk(string &, string &);
	void mountDisk(string &);
	void unmountDisk();
	void recreateDisk(string &);
	fstream* getdskfl();
	void seekToSector(unsigned int);
	void writeSector(unsigned int, Sector*);
	void wirteSector(Sector*);
	void readSector(int, Sector*);
	void readSector(Sector*);

};
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
void Disk::createDisk(string & filename,string & owner)
{
	ifstream file(filename);
	if (file.good)
		throw "You cant create a disk with a name that is already taken!";
	ofstream ofile(filename);//error
	vhd


}