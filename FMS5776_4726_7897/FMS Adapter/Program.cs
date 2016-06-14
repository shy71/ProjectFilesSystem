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

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDirEntry(IntPtr THIS, IntPtr pvhd);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetFileNames(IntPtr THIS);
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
        #region VHD FUNCTIONS
        

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetName(IntPtr THIS);
        #endregion
    }
    public class Disk
    {
        IntPtr myDiskPointer;
        VolumeHeader vhd;
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
                vhd = GetVolumeHeader();
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
        public FCB OpenFile(string fileName, string fileOwner, string openMode)
        {
            try
            {
                return new FCB(cppToCsharpAdapter.OpenFile(this.myDiskPointer, fileName, fileOwner, openMode));
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
        public string[] GetFileNames()
        {
            try
            {
                //VolumeHeader v = new VolumeHeader();
                //int structSize = Marshal.SizeOf(v.GetType()); //Marshal.SizeOf(typeof(Student));  
                //IntPtr buffer = Marshal.AllocHGlobal(structSize);
                //Marshal.StructureToPtr(v, buffer, true);

                //// ... send buffer to dll 
                //cppToCsharpAdapter.GetVolumeHeader(this.myDiskPointer, buffer);
                //Marshal.PtrToStructure(buffer, v);

                //// free allocate 
                //Marshal.FreeHGlobal(buffer);

                //return v;
                return null;
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
        //VolumeHeader GetVolumeHeader()
        //{
        //    try
        //    {

        //        VolumeHeader v = new VolumeHeader();
        //        //int structSize = Marshal.SizeOf(v.GetType()); //Marshal.SizeOf(typeof(Student));  
        //        //IntPtr buffer = Marshal.AllocHGlobal(structSize);
        //        //Marshal.StructureToPtr(v, buffer, true);

        //        // ... send buffer to dll 
        //        Marshal.PtrToStructure(cppToCsharpAdapter.GetVolumeHeader(this.myDiskPointer), v);

        //        // free allocate 
        //        //Marshal.FreeHGlobal(buffer);

        //        return v;
        //    }
        //    catch (SEHException)
        //    {
        //        IntPtr cString = cppToCsharpAdapter.GetLastDiskErrorMessage(this.myDiskPointer);
        //        string message = Marshal.PtrToStringAnsi(cString);
        //        throw new Exception(message);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public string GetName() { return vhd.DiskName; }
        public string GetOwner() { return vhd.DiskOwner; }
        public string GetFormatDate() { return vhd.FormatDate; }
        public string GetCreationDate() { return vhd.ProdDate; }
        public List<string> GetFilesNames()
        { //למחוק ולעשות באמת
           var s= Marshal.PtrToStringAnsi(cppToCsharpAdapter.GetFileNames(myDiskPointer)).Split(',').ToList();
           s.RemoveAt(s.Count-1);
           if (s.FirstOrDefault()==null||s.First() == "")
               s.Clear();
           return s;
        }//לעשות

        #endregion
    }
    public class FCB
    {
        private IntPtr myFCBPointer;
        public DirEntry GetDirEntry()
        {
            try
            {

                DirEntry v = new DirEntry();
                int structSize = Marshal.SizeOf(v.GetType()); //Marshal.SizeOf(typeof(Student));  
                IntPtr buffer = Marshal.AllocHGlobal(structSize);
                Marshal.StructureToPtr(v, buffer, true);

                // ... send buffer to dll 
                cppToCsharpAdapter.GetDirEntry(this.myFCBPointer, buffer);
                Marshal.PtrToStructure(buffer, v);

                // free allocate 
                Marshal.FreeHGlobal(buffer);

                return v;
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
        public void ReadRecord(out string dest,int size, uint readForUpdate = 0)
        {
            try
            {
                IntPtr buffer = Marshal.AllocHGlobal(size);
                cppToCsharpAdapter.ReadRecord(this.myFCBPointer, buffer, readForUpdate);
                //copy to dest
                dest= Marshal.PtrToStringAnsi(buffer);
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
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class DirEntry
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        string filename;
        public string Filename { get { return filename; } }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        string fileOwner;
        public string FileOwner { get { return filename; } }

        uint fileAddr;
        public uint FileAddr { get { return fileAddr; } }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        string crDate;
        public string CrDate { get { return crDate; } }
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    //public class VolumeHeader
    //{
    //    uint sectorNr;

    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
    //    string diskName;
    //    public string DiskName { get { return diskName; } }

    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
    //    string diskOwner;
    //    public string DiskOwner { get { return diskOwner; } }

    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    //    string prodDate;
    //    public string ProdDate { get { return prodDate; } }

    //    uint clusQty;
    //    public uint ClusQty { get { return clusQty; } }

    //    uint dataClusQty;
    //    public uint DataClusQty { get { return dataClusQty; } }

    //    uint addrDAT;
    //    public uint AddrDAT { get { return addrDAT; } }

    //    uint addrRootDir;
    //    public uint AddrRootDir { get { return addrRootDir; } }

    //    uint addrDATcpy;
    //    public uint AddrDATcpy { get { return addrDATcpy; } }

    //    uint addrRootDirCpy;
    //    public uint AddrRootDirCpy { get { return addrRootDirCpy; } }

    //    uint addrDataStart;
    //    public uint aAddrDataStart { get { return addrDataStart; } }

    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    //    string formatDate;
    //    public string FormatDate { get { return formatDate; } }

    //    [MarshalAs(UnmanagedType.I1)]
    //    bool isFormated;
    //    public bool IsFormated { get { return isFormated; } }


    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 944)]
    //    string emptyArea;
    //}

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        string recFormat;
        public string RecFormat { get { return recFormat; } }//changed place to save bits which will later allow us to store more sectors in the file
	    uint fileSize;
        public uint FileSize { get { return fileSize; } }
	    uint eofRecNum;
        public uint EofRecNum { get { return eofRecNum; } }
	    uint maxRecSize;
        public uint MaxRecSize { get { return maxRecSize; } }
	    uint actualRecSize;
        public uint ActualRecSize { get { return actualRecSize; } }
	    uint keyOffset;
        public uint KeyOffset { get { return keyOffset; } }
	    uint keySize;
        public uint KeySize { get { return keySize; } }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        string keyType;
        public string KeyType { get { return keyType; } }
        uint entryStatus;
        public uint EntryStatus { get { return entryStatus; } }
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

            int structSize = Marshal.SizeOf(typeof(VolumeHeader));
            Console.WriteLine("Marshal.SizeOf(typeof(VolumeHeader) == " + structSize);
            Disk d = new Disk();
            d.Createdisk("work", "oshri");
            d.MountDisk("work");
            d.CreateFile("File1", "Ezra", "F", 20, 20, "I", 0);
            FCB fcb = d.OpenFile("File1", "Ezra", "I");
            fcb.WriteRecord("shy");
            d.CreateFile("Folders", "Ezra", "F", 20, 20, "I", 0);
            d.CreateFile("Gmara", "Ezra", "F", 20, 20, "I", 0);
            d.UnmountDisk();



            //catch (Exception e)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(e.Message);
            //    Console.ResetColor();
            //}

        }
    }
}
