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
        #region CONSTRUCTORS & DESTRUCTORS
        public Disk()
        {
            this.myDiskPointer = cppToCsharpAdapter.MakeDiskObject();
        }
        ~Disk()
        {
            if(myDiskPointer!=null)
                cppToCsharpAdapter.DeleteDiskObject(ref myDiskPointer);
        }
        #endregion
        #region LEVEL 0 FUNCTIONS
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
        #endregion
        #region LEVEL 1 FUNCTIONS
        public void Format(string owner)
        {
            try
            {
                cppToCsharpAdapter.Format(this.myDiskPointer, owner);
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
        public void HowMuchEmpty()
        {
            try
            {
                cppToCsharpAdapter.HowMuchEmpty(this.myDiskPointer);
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
        #endregion
        #region LEVEL 2 FUNCTIONS
        public void CreateFile(string fileName, string fileOwner, string FinalOrVar,
                                uint recSize, uint fileSize,
                                string keyType, uint keyOffset, uint keySize = 4)
        {
            try
            {
                cppToCsharpAdapter.CreateFile(this.myDiskPointer,fileName,fileOwner,FinalOrVar,recSize,fileSize,keyType,keyOffset,keySize);
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
        public void DelFile(string fileName,string fileOwner)
        {
            try
            {
                cppToCsharpAdapter.DelFile(this.myDiskPointer,fileName,fileOwner);
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
        public void ExtendFile(string fileName, string fileOwner,uint size)
        {
            try
            {
                cppToCsharpAdapter.ExtendFile(this.myDiskPointer, fileName, fileOwner,size);
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
        #endregion
        #region LEVEL 3 FUNCTIONS
        public IntPtr OpenFile(string fileName, string fileOwner, string openMode)
        {
            try
            {
                return cppToCsharpAdapter.OpenFile(this.myDiskPointer,fileName,fileOwner,openMode);
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
        #endregion
    }
    class FCB
    {
        private IntPtr myFCBPointer;
        
        #region DESTRUCTOR
        ~FCB()
        {
            if (myFCBPointer != null)
                cppToCsharpAdapter.DeleteFcbObject(ref myFCBPointer);
        }
        #endregion
        #region BASIC FUNCTIONS
        public void CloseFile()
        {
            try
            {
                cppToCsharpAdapter.CloseFile(this.myFCBPointer);
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastFcbErrorMessage(this.myFCBPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        public void ReadRecord(object dest, uint readForUpdate = 0)
        {
            try
            {
                IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(dest.GetType()));
                
                cppToCsharpAdapter.ReadRecord(this.myFCBPointer, buffer, readForUpdate);
                //copy to dest
                Marshal.FreeHGlobal(buffer);  
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastFcbErrorMessage(this.myFCBPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        public void WriteRecord(object source)
        {
            try
            {
                IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(source.GetType()));
                Marshal.StructureToPtr(source, buffer, true);
                cppToCsharpAdapter.WriteRecord(this.myFCBPointer, buffer);
                Marshal.FreeHGlobal(buffer);  
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastFcbErrorMessage(this.myFCBPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        public void SeekRecord(uint from, int pos)
        {
            try
            {
                cppToCsharpAdapter.SeekRecord(this.myFCBPointer, from, pos);
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastFcbErrorMessage(this.myFCBPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region UPDATE FUNCTIONS
        public void UpdateRecCancel()
        {
            try
            {
                cppToCsharpAdapter.UpdateRecCancel(this.myFCBPointer);
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastFcbErrorMessage(this.myFCBPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        public void DeleteRecord()
        {
            try
            {
                cppToCsharpAdapter.DeleteRecord(this.myFCBPointer);
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastFcbErrorMessage(this.myFCBPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        public void UpdateRecord(object source)
        {
            try
            {
                IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(source.GetType()));
                Marshal.StructureToPtr(source, buffer, true);
                cppToCsharpAdapter.UpdateRecord(this.myFCBPointer, buffer);
                Marshal.FreeHGlobal(buffer);  
            }
            catch (SEHException)
            {
                IntPtr cString = cppToCsharpAdapter.GetLastFcbErrorMessage(this.myFCBPointer);
                string message = Marshal.PtrToStringAnsi(cString);
                throw new Exception(message);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
