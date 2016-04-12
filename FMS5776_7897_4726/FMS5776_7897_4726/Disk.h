#pragma once
using namespace std;
#include "VolumeHeader.h"
#include "DAT.h"
#include "Sector.h"
#include "FCB.h"
#include "Dir.h"
#include <fstream>
class FCB;
struct Disk
{
	VolumeHeader vhd;
	DAT dat;
	RootDir rootDir;
	bool mounted;
	fstream dskfl;
	unsigned int currDiskSectorNr;


	void Disk::writePlusCpy(unsigned int sor, unsigned int cpy, DAT sec);
	void Update();

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

	//level 1
	void format(string &);
	int howmuchempty();
	void dealloc(DATtype& FAT);
	void alloc(DATtype & fat, unsigned int num, unsigned int type, unsigned int index = 0);
	void allocextend(DATtype &, unsigned int, unsigned int);
	int howmuchused();
	void writeDir(unsigned int add, unsigned int addcpy, RootDir root);

	//level 2
	void createfile(string , string , string , unsigned int, unsigned int, string , unsigned int, unsigned int length=-1);
	void delfile(string &, string &);
	void extendfile(string &, string &, unsigned int);

	//level 3
	FCB *openfile(string &, string &, string &);
};