#include"Disk.h"
#include"FCB.h"


extern "C"
{
//DISK FUNCTIONS:

	#pragma region LEVEL 0 FUNCTIONS
		__declspec(dllexport) void UnmountDisk(Disk* THIS)
		{
			try
			{
				THIS->unmountDisk();
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void CreateDisk(Disk* THIS, char* diskName, char* diskOwner)
		{
			try
			{
				THIS->createDisk(diskName, diskOwner);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void MountDisk(Disk* THIS, char* fName)
		{
			try
			{
				THIS->mountDisk(fName);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void RecreateDisk(Disk* THIS, char* owner)
		{
			try
			{
				THIS->recreateDisk(owner);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
	#pragma endregion
	#pragma region LEVEL 1 FUNCTIONS
		__declspec(dllexport) void Format(Disk* THIS, char* owner)
		{
			try
			{
				THIS->format(owner);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) int HowMuchEmpty(Disk* THIS)
		{
			try
			{
				return THIS->howmuchempty();
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
	#pragma endregion
	#pragma region LEVEL 2 FUNCTIONS
		__declspec(dllexport) void CreateFile(Disk* THIS, char* fname, char* fowner, char* filetype, unsigned int recSize, unsigned int secCount, char* keyType, unsigned int keyOffset, unsigned int keySize)
		{
			try
			{
				THIS->createfile(fname, fowner, filetype, recSize, secCount, keyType, keyOffset, keySize);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void DelFile(Disk* THIS, char* fname, char* username)
		{
			try
			{
				THIS->delfile(fname, username);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void ExtendFile(Disk* THIS, char* fname, char* username, unsigned int numToAdd)
		{
			try
			{
				THIS->extendfile(fname, username, numToAdd);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
	#pragma endregion
	#pragma region LEVEL 3 FUNCTIONS
		__declspec(dllexport) FCB* OpenFile(Disk* THIS, char* fname, char* fowner, char* openMode)
		{
			try
			{
				return THIS->openfile(fname, fowner, openMode);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
	#pragma endregion
	#pragma region LEVEL 4 FUNCTIONS
		__declspec(dllexport)  void  GetVolumeHeader(Disk* THIS, VolumeHeader* buffer)
		{
			memcpy_s(buffer, sizeof(VolumeHeader), &THIS->GetVolumeHeader(), sizeof(VolumeHeader));
		}
	#pragma endregion

//FCB FUNCTIONS:

	#pragma region BASIC FUNCTIONS
		__declspec(dllexport) void CloseFile(FCB* THIS)
		{
			try
			{
				THIS->closeFile();
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void ReadRecord(FCB* THIS, char *dest, unsigned int status = 0)
		{
			try
			{
				THIS->read(dest, status);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void WriteRecord(FCB* THIS, char *data)
		{
			try
			{
				THIS->write(data);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void SeekRecord(FCB* THIS, unsigned int from, int recordCount)
		{
			try
			{
				THIS->seek(from, recordCount);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
	#pragma endregion
	#pragma region UPDATE FUNCTIONS
		__declspec(dllexport) void UpdateRecCancel(FCB* THIS)
		{
			try
			{
				THIS->updateCancel();
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void DeleteRecord(FCB* THIS)
		{
			try
			{
				THIS->deleteRecord();
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
		__declspec(dllexport) void UpdateRecord(FCB* THIS, char *update)
		{
			try
			{
				THIS->updateRecord(update);
			}
			catch (exception ex)
			{
				THIS->SetLastErrorMessage(ex.what());
				throw ex;
			}
		}
	#pragma endregion

//CONSTRUCTORS AND DESTRUCTORS:

	#pragma region CREATE OR DELETE DISK / FCB 
		__declspec(dllexport) Disk* MakeDiskObject()
		{
			return new Disk();
		}
		__declspec(dllexport) void DeleteDiskObject(Disk*& THIS)
		{
			if (THIS != NULL)
				delete  THIS;
			THIS = NULL;
		}
		__declspec(dllexport) const char* GetLastDiskErrorMessage(Disk* THIS)
		{
			const char* str = THIS->GetLastErrorMessage().c_str();
			return str;
		}
		__declspec(dllexport) void  DeleteFcbObject(FCB*& THIS)
		{
			if (THIS != NULL)
				delete  THIS;
			THIS = NULL;
		}
		__declspec(dllexport) const  char* GetLastFcbErrorMessage(FCB* THIS)
		{
			const char* str = THIS->GetLastErrorMessage().c_str();
			return str;
		}
	#pragma endregion
}