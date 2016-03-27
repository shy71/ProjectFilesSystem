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

	void writePlusCpy(unsigned int, unsigned int, Sector*);

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
	void writeSector(Sector*);
	void readSector(unsigned int, Sector*);
	void readSector(Sector*);
	int howmuchempty();
	void dealloc(DATtype& FAT);

	//level 1
	void format(string &);
};