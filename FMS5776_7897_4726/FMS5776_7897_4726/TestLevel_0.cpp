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
	//friend class Test_Level_1;
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
//class Test_Level_1
//{
//public:
//	static void printDAT(Disk &d)
//	{
//		cout << "DAT:" << d.dat.Dat << endl << endl;
//	}
//	static void printFAT(DATtype &f)
//	{
//		cout << f << endl << endl;
//	}
//	static void CheckFunctions(string diskName,string ownerName)
//	{
//		Disk d;
//		cout << "\npre createdisk: " << endl;
//		TestLevel_0::printDiskInfo(d);
//		cout << "post createdisk: " << endl;
//		d.createDisk(diskName, ownerName);
//		TestLevel_0::printDiskInfo(d);
//		printDAT(d);
//		DATtype FAT1,FAT2,FAT3,FAT4;
//		printFAT(FAT1);
//		d.alloc(FAT1, 30, 0);
//		d.alloc(FAT2, 20, 0);
//		d.alloc(FAT3, 60, 0);
//		d.alloc(FAT4, 10, 0);
//		cout << "FAT1:" << endl;
//		printFAT(FAT1);
//		cout << "FAT2:" << endl;
//		printFAT(FAT2);
//		cout << "FAT3:" << endl;
//		printFAT(FAT3);
//		cout << "FAT4:" << endl;
//		printFAT(FAT4);
//		d.dealloc(FAT2);
//		cout << "FAT2" << endl;
//		printFAT(FAT2);
//		d.allocextend(FAT4, 25, 0);
//		printDAT(d);
//		cout << d.howmuchempty();
//		cout << d.howmuchused();
//		string name = d.vhd.diskOwner;
//		d.format(name);
//		printDAT(d);
//		d.unmountDisk();
//	}
//};