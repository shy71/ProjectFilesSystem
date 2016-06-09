using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

namespace FMS_adapter
{
   public class cppToCsharpAdapter
    {
        const string dllPath = "FMS DLL.dll";
        #region CREATE / DESTROY / GET ERROR => OF FCB / DISK
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr MakeDiskObject();

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeleteDiskObject(ref IntPtr THIS);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetLastDiskErrorMessage(IntPtr THIS);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetLastFcbErrorMessage(IntPtr THIS);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeleteFcbObject(ref IntPtr THIS);
        #endregion
        #region LEVEL 0 FUNCTIONS
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateDisk(IntPtr THIS, string diskName, string diskOwner);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MountDisk(IntPtr THIS, string diskName);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void UnmountDisk(IntPtr THIS);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RecreateDisk(IntPtr THIS, string diskOwner);
        #endregion
        #region LEVEL 1 FUNCTIONS
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Format(IntPtr THIS, string diskOwner);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int HowMuchEmpty(IntPtr THIS);
        #endregion
        #region LEVEL 2 FUNCTIONS
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateFile(IntPtr THIS, string fileName, string fileOwner, string FinalOrVar,
                                uint recSize, uint fileSize,
                                string keyType, uint keyOffset, uint keySize = 4);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DelFile(IntPtr THIS, string fileName, string fileOwner);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExtendFile(IntPtr THIS, string fileName, string fileOwner, uint size);
        #endregion
        #region LEVEL 3 FUNCTIONS
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenFile(IntPtr THIS, string fileName, string fileOwner, string openMode);
        #endregion
        #region LEVEL 4 FUNCTIONS
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetVolumeHeader(IntPtr THIS, IntPtr pvhd);
        #endregion
        #region FCB FUNCTIONS
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseFile(IntPtr THIS);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReadRecord(IntPtr THIS, IntPtr dest, uint readForUpdate = 0);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteRecord(IntPtr THIS, IntPtr source);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SeekRecord(IntPtr THIS, uint from, int pos);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateRecCancel(IntPtr THIS);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeleteRecord(IntPtr THIS);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateRecord(IntPtr THIS, IntPtr source);
        #endregion
    }
    public class Disk
    {
        IntPtr myDiskPointer;
        #region CONSTRUCTORS & DESTRUCTORS
        public Disk()
        {
            this.myDiskPointer = cppToCsharpAdapter.MakeDiskObject();
        }
        ~Disk()
        {
            if (myDiskPointer != null)
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
                cppToCsharpAdapter.RecreateDisk(this.myDiskPointer, owner);
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
        public int HowMuchEmpty()
        {
            try
            {
               return cppToCsharpAdapter.HowMuchEmpty(this.myDiskPointer);
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
                cppToCsharpAdapter.CreateFile(this.myDiskPointer, fileName, fileOwner, FinalOrVar, recSize, fileSize, keyType, keyOffset, keySize);
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
        public void DelFile(string fileName, string fileOwner)
        {
            try
            {
                cppToCsharpAdapter.DelFile(this.myDiskPointer, fileName, fileOwner);
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
        public void ExtendFile(string fileName, string fileOwner, uint size)
        {
            try
            {
                cppToCsharpAdapter.ExtendFile(this.myDiskPointer, fileName, fileOwner, size);
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
                return cppToCsharpAdapter.OpenFile(this.myDiskPointer, fileName, fileOwner, openMode);
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
        #region LEVEL 4 FUNCTIONS
        public VolumeHeader GetVolumeHeader()
        {
            try
            {

                VolumeHeader v = new VolumeHeader();
                int structSize = Marshal.SizeOf(v.GetType()); //Marshal.SizeOf(typeof(Student));  
                IntPtr buffer = Marshal.AllocHGlobal(structSize);
                Marshal.StructureToPtr(v, buffer, true);

                // ... send buffer to dll 
                cppToCsharpAdapter.GetVolumeHeader(this.myDiskPointer, buffer);
                Marshal.PtrToStructure(buffer, v);

                // free allocate 
                Marshal.FreeHGlobal(buffer);

                return v;
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
        public List<string> GetFilesNames() { return null; }//לעשות

        #endregion
    }
    public class FCB
    {
        private IntPtr myFCBPointer;

        #region DESTRUCTOR
        public FCB(IntPtr p)
        {
            myFCBPointer = p;
        }
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
                Marshal.PtrToStructure(buffer, dest);
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
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class VolumeHeader
    {
        uint sectorNr;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        string diskName;
        public string DiskName { get { return diskName; } }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        string diskOwner;
        public string DiskOwner { get { return diskOwner; } }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        string prodDate;
        public string ProdDate { get { return prodDate; } }

        uint clusQty;
        public uint ClusQty { get { return clusQty; } }

        uint dataClusQty;
        public uint DataClusQty { get { return dataClusQty; } }

        uint addrDAT;
        public uint AddrDAT { get { return addrDAT; } }

        uint addrRootDir;
        public uint AddrRootDir { get { return addrRootDir; } }

        uint addrDATcpy;
        public uint AddrDATcpy { get { return addrDATcpy; } }

        uint addrRootDirCpy;
        public uint AddrRootDirCpy { get { return addrRootDirCpy; } }

        uint addrDataStart;
        public uint aAddrDataStart { get { return addrDataStart; } }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        string formatDate;
        public string FormatDate { get { return formatDate; } }

        [MarshalAs(UnmanagedType.I1)]
        bool isFormated;
        public bool IsFormated { get { return isFormated; } }


        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 944)]
        string emptyArea;
    }



    public class Program
    {
        public static string ToStringProperty(object t)
        {
            string str = "";
            foreach (PropertyInfo item in t.GetType().GetProperties())
                str += "\n" + item.Name + ": " + item.GetValue(t, null);
            return str;
        }
        static void Main(string[] args)
        {
            try
            {
                int structSize = Marshal.SizeOf(typeof(VolumeHeader));
                Console.WriteLine("Marshal.SizeOf(typeof(VolumeHeader) == " + structSize);
                Disk d = new Disk();
                Console.WriteLine("\nMake Disk:");
                Console.WriteLine(ToStringProperty(d.GetVolumeHeader()));
                d.Createdisk("disk1", "oshri");
                Console.WriteLine("\nCreate Disk:");
                Console.WriteLine(ToStringProperty(d.GetVolumeHeader()));
                d.MountDisk("disk1");
                d.Format("oshri");
                Console.WriteLine("\nFormat Disk:");
                Console.WriteLine(ToStringProperty(d.GetVolumeHeader()));


                d.CreateFile("File1", "Ezra", "F", 20, 20, "I", 0);
                FCB fcb = new FCB(d.OpenFile("File1", "Ezra", "IO"));
                fcb.WriteRecord("hello");

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }
    }
}
