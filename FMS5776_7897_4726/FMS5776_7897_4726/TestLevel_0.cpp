#include<iostream>
#include "Disk.h"
#include "FileHeader.h"
using namespace std;

class TestLevel_0
{
	static void printStructSize()
	{
		cout << "start" << endl;
		cout << "Size Of Disk -->" << sizeof(Disk) << endl;
		cout << "Size Of Sector -->" << sizeof(Sector) << endl;
		cout << "Size Of VolumeHeader -->" << sizeof(VolumeHeader) << endl;
		cout << "Size Of DAT -->" << sizeof(DAT) << endl;
		cout << "Size Of DirEntry -->" << sizeof(dirEntry) << endl;
		cout << "Size Of SectorDir -->" << sizeof(SectorDir) << endl;
		cout << "Size Of FileHeader -->" << sizeof(FileHeader) << endl;

		cout << "Size Of RootDir -->" << sizeof(RootDir) << endl;
	}

	static void printDiskInfo(Disk& d)
	{
		VolumeHeader* vh = &d.vhd;

		cout << "	disk name:        " << vh->diskName << endl;
		cout << "	Owner Name:       " << vh->diskOwner << endl;
		cout << "	prodDate:         " << vh->prodDate << endl;
		cout << "	formatDate:       " << vh->formatDate << endl;
		cout << "	isFormated:       " << vh->isFormated << endl;
		cout << "	addrDataStart:    " << vh->addrDataStart << endl;

		cout << "	ClusQty:          " << vh->ClusQty << endl;
		cout << "	dataClusQty:      " << vh->dataClusQty << endl;

		cout << "	addrDAT:          " << vh->addrDAT << endl;
		cout << "	addrRootDir:      " << vh->addrRootDir << endl;
		cout << "	addrDATcpy:       " << vh->addrDATcpy << endl;
		cout << "	addrRootDirCpy:   " << vh->addrRootDirCpy << endl << endl;

	}

	static void test_create(string diskName, string ownerName)
	{
		Disk d;
		cout << "\npre createdisk: " << endl;
		printDiskInfo(d);
		cout << "post createdisk: " << endl;
		d.createDisk(diskName, ownerName);
		printDiskInfo(d);
	}

	static void test_mount(string diskName)
	{
		Disk d;
		cout << "\npre mountdisk: " << endl;
		printDiskInfo(d);
		d.mountDisk(diskName);
		cout << "post mountdisk: " << endl;
		printDiskInfo(d);
		d.unmountDisk();
	}


	static void test_rwSector(string diskName)
	{
		Disk d;
		Sector sector;
		d.mountDisk(diskName);

		cout << "\nread sector: " << endl;
		d.readSector(8, &sector);
		strcpy_s(sector.RawData, "this is write temp sector");
		d.writeSector(8, &sector);
		d.unmountDisk();

	}
	friend class Test_Level_1;
public:
	static void test_0()
	{
		try
		{
			string diskName = "aas";
			string ownerName = "oshri";

			printStructSize();
			test_create(diskName, ownerName);
			test_mount(diskName);
		}
		catch (exception ex)
		{
			cout << ex.what() << endl;
		}
		catch (char * ex)
		{
			cout << ex << endl;
		}
	}
};
class Test_Level_1
{
public:
	static void printDATtype(DATtype &d)
	{
		for (int i = 0; i < 1600; i++)
		{
			cout << d[i];
		}
		cout << endl;

	}
	static void CheckFunctions(string diskName,string ownerName)
	{
		Disk d;
		cout << "\npre createdisk: " << endl;
		TestLevel_0::printDiskInfo(d);
		cout << "post createdisk: " << endl;
		d.mountDisk(diskName);
		TestLevel_0::printDiskInfo(d);


		d.createfile("shy file", "shy71", "F", 15, 30, "1", 10, 5);
		//printDATtype(d.dat.Dat);
		//DATtype FAT1,FAT2,FAT3,FAT4;

		//string name = d.vhd.diskOwner;
		//d.alloc(FAT1, 96*2,0);
		//printDATtype(d.dat.Dat);
		//d.alloc(FAT2, 800*2,0);
		//printDATtype(d.dat.Dat);
		//d.alloc(FAT1, 100*2,0);
		//printDATtype(d.dat.Dat);
		//d.alloc(FAT2, 600*2,0);
		//printDATtype(d.dat.Dat);
		//d.dealloc(FAT1);
		//printDATtype(d.dat.Dat);
		//d.alloc(FAT1,196*2,0);
		//printDATtype(d.dat.Dat);
		//d.format(name);
		//printDATtype(FAT1);
		//d.alloc(FAT1, 60, 0);
		//d.alloc(FAT2, 33, 0);
		//d.alloc(FAT3, 76, 0);
		//d.alloc(FAT4, 1000, 0);
		//cout << "FAT1:" << endl;
		//printDATtype(FAT1);
		//cout << "FAT2:" << endl;
		//printDATtype(FAT2);
		//cout << "FAT3:" << endl;
		//printDATtype(FAT3);
		//cout << "FAT4:" << endl;
		//printDATtype(FAT4);
		//d.dealloc(FAT2);
		//cout << "FAT2" << endl;
		//printDATtype(FAT2);
		//cout << "Dat:" << endl;
		//printDATtype(d.dat.Dat);
		//cout << "FAT4:" << endl;
		//printDATtype(FAT4);
		//d.allocextend(FAT3, 100, 0);
		//cout << "FAT3:" << endl;
		//printDATtype(FAT3);
		//cout << "Dat:" << endl;
		//printDATtype(d.dat.Dat);
		//cout << d.howmuchempty();
		//cout << d.howmuchused();

		//d.format(name);
		//printDATtype(d.dat.Dat);
		TestLevel_0::printDiskInfo(d);
		d.unmountDisk();
	}
};