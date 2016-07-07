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
	//the header for the disk
	VolumeHeader vhd;
	DAT dat;
	//the entries of the files in the disk
	RootDir rootDir;
	bool mounted;
	fstream dskfl;
	unsigned int currDiskSectorNr;
	string lastErrorMessage;

	string end = ".dsk";

	void Disk::writePlusCpy(unsigned int sor, unsigned int cpy, DAT sec);
	void Update();

public:
	Disk();
	Disk(string, string, char);
	~Disk();
	void createDisk(string, string);//creates a disk
	void mountDisk(string);//mounts a existing disk 
	void unmountDisk();//unmounts a existing disk
	void recreateDisk(string);//recreates a certain disk
	fstream* getdskfl();// returns the file where the disk is saved in
	void seekToSector(unsigned int);//seeks to a specific sector
	void writeSector(unsigned int, Sector*);//writes a sector of data at a specific place
	void writeSector(Sector*);//writes a sector at the current place
	void readSector(unsigned int, Sector*);//reads a specific sector sector
	void readSector(Sector*);//reads the current sector

	//level 1
	void format(string);//formats the disk
	int howmuchempty();//checks how much space is empty
	void dealloc(DATtype& FAT);//releases memory that was being saved for a file in the disk
	void alloc(DATtype & fat, unsigned int num, unsigned int type, unsigned int index = 0);//allocates memory for a file in the disk
	void allocextend(DATtype &, unsigned int, unsigned int);//extends the memory allocated for a file in the disk
	int howmuchused();//returns how much space was used
	void writeDir(unsigned int add, unsigned int addcpy, RootDir root);//writes an entry for a file

	//level 2
	void createfile(string, string, string, unsigned int, unsigned int, string, unsigned int, unsigned int length = -1);//creates a new file
	void delfile(string, string);//deletes a file
	void extendfile(string, string, unsigned int);//extends a file

	//level 3
	FCB *openfile(string, string, string);//opens a file and returns the fcb of that file

	//level 4
	string& Disk::GetLastErrorMessage();//returns the messafge of the last error that occured (used for higher level)
	void Disk::SetLastErrorMessage(string lastErrorMessage);//sets the message of the last error that occured
	VolumeHeader GetVolumeHeader();//returns the volume header of the disk
	char* GetFileNames();//return the names of all the files in the disk
	string getEnd(){ return end; }//returns the end extension of the file that the disk is saved in
	void setEnd(string end){ this->end = end; }//sets the extension 
};