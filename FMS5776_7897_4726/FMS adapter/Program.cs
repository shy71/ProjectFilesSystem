using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FMS_adapter
{
    class cppToCsharpAdapter 
    {
        const string dllPath = "FMS DLL.dll";
        #region CREATE / DESTROY / GET ERROR => OF FCB / DISK
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern IntPtr MakeDiskObject(); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void DeleteDiskObject(ref IntPtr THIS); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern IntPtr GetLastDiskErrorMessage( IntPtr THIS); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern IntPtr GetLastFcbErrorMessage(IntPtr THIS); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void DeleteFcbObject(ref IntPtr THIS);
        #endregion
        #region LEVEL 0 FUNCTIONS
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void CreateDisk(IntPtr THIS, string diskName, string diskOwner); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void MountDisk(IntPtr THIS, string diskName); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void UnmountDisk(IntPtr THIS); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void RecreateDisk(IntPtr THIS, string diskOwner);
        #endregion
        #region LEVEL 1 FUNCTIONS
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void Format(IntPtr THIS, string diskOwner); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern int HowMuchEmpty(IntPtr THIS);
        #endregion
        #region LEVEL 2 FUNCTIONS
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void CreateFile(IntPtr THIS, string fileName, string fileOwner, string FinalOrVar, 
                                uint recSize, uint fileSize, 
                                string keyType, uint keyOffset, uint keySize = 4); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void DelFile(IntPtr THIS, string fileName, string fileOwner); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void ExtendFile(IntPtr THIS, string fileName, string fileOwner, uint size);
        #endregion
        #region LEVEL 3 FUNCTIONS
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern IntPtr OpenFile(IntPtr THIS, string fileName, string fileOwner, string openMode);
        #endregion
        #region FCB FUNCTIONS
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void CloseFile(IntPtr THIS); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void ReadRecord(IntPtr THIS, IntPtr dest, uint readForUpdate = 0); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void WriteRecord(IntPtr THIS, IntPtr source); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void SeekRecord(IntPtr THIS, uint from, int pos); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void UpdateRecCancel(IntPtr THIS); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void DeleteRecord(IntPtr THIS); 
 
        [DllImport(dllPath , CallingConvention=CallingConvention.Cdecl)] 
        public static extern void UpdateRecord(IntPtr THIS, IntPtr source);
        #endregion
    } 
    class Disk
    {
        IntPtr myDiskPointer;

        public Disk()
        {
            this.myDiskPointer = cppToCsharpAdapter.MakeDiskObject();
        }
        public ~Disk()
        {
            if(myDiskPointer!=null)
                cppToCsharpAdapter.DeleteDiskObject(ref myDiskPointer);
        }

        public void Createdisk(string diskName, string diskOwner)
        {
            try
            {
                cppToCsharpAdapter.CreateDisk(this.myDiskPointer, diskName, diskOwner);
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastDiskErrorMessage(this.myDiskPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        public void MountDisk(string diskName)
        {
            try
            {
                cppToCsharpAdapter.MountDisk(this.myDiskPointer, diskName);
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastDiskErrorMessage(this.myDiskPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        public void UnmountDisk()
        {
            try
            {
                cppToCsharpAdapter.UnmountDisk(this.myDiskPointer);
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastDiskErrorMessage(this.myDiskPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        public void RecreateDisk(string owner)
        {
            try
            {
                cppToCsharpAdapter.RecreateDisk(this.myDiskPointer,owner);
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastDiskErrorMessage(this.myDiskPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        } 




    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
