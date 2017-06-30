using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SanityArchiver
{
    class NavigatorBoxOperations
    {
        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        public static int GetTaskbarHeight()
        {
            return Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
        }

        public static void RefreshNaviBoxContent()
        {
            foreach (NavigatorBox item in SanityCommanderForm.naviBoxes)
            {
                item.ListContent();
            }
        }

        public static bool ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);
        }

        public static void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch { CustomDialog.ErrorMessage("Error: Access denied.", "Error"); }
            RefreshNaviBoxContent();
        }

        public static void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path);
            }
            catch { CustomDialog.ErrorMessage("Error: Access denied.", "Error"); }
            RefreshNaviBoxContent();
        }


        public static bool IsItADirectory(string path)
        {
            bool isDirectory = false;
            try
            {
                FileAttributes fa = File.GetAttributes(path);
                if ((fa & FileAttributes.Directory) != 0)
                {
                    isDirectory = true;
                }
                return isDirectory;
            }
            catch { }
            return isDirectory;
        }

        public static void EncryptFile(string FileName)
        {

            File.Encrypt(FileName);
            CustomDialog.ErrorMessage("File encrypted successfully.", "Notification");
        }

        public static void DecryptFile(string FileName)
        {
            File.Decrypt(FileName);
            CustomDialog.ErrorMessage("File decrypted successfully.", "Notification");
        }

        public static long FileSize(FileInfo f)
        {
            long size = 0;
            size += f.Length;
            return size;
        }


        public static int CalculateNumberOfNaviBoxes()
        {
            if (SanityCommanderForm.naviBoxes.Count == 0)
            {
                return 1;
            }
            return 0;
        }

        public static void CopyFile(string oldPath, string newPath)
        {
            try
            {
                File.Copy(oldPath, newPath, true);
                RefreshNaviBoxContent();
            }
            catch { CustomDialog.ErrorMessage("Copy unsuccessful.", "Error"); }
        }

        public static FileInfo MakeFileInfoFromPath()
        {
            FileInfo fi = null;
            try
            {
                fi = new FileInfo(SanityCommanderForm.SelectedFilePath);
            }
            catch { }
            return fi;
        }

        public static void RenameFile(string oldName, string newName)
        {
            try
            {
                if (File.Exists(newName))
                {
                    File.Delete(newName);
                }
                File.Move(oldName, newName);
                RefreshNaviBoxContent();
            }
            catch { CustomDialog.ErrorMessage("Rename unsuccessful.", "Error"); }
        }

        public static void RenameDirectory(string oldName, string newName)
        {
            try
            {
                if (Directory.Exists(newName))
                {
                    Directory.Delete(newName);
                }
                Directory.Move(oldName, newName);
                RefreshNaviBoxContent();
            }
            catch { CustomDialog.ErrorMessage("Rename unsuccessful.", "Error"); }
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            try
            {
                FileInfo[] fis = d.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    size += fi.Length;
                    return size;
                }
            }
            catch { }
            return size;
        }

        public static void CheckIfEncryptable()
        {
            if (!IsItADirectory(SanityCommanderForm.SelectedFilePath))
            {
                try
                {
                    EncryptFile(SanityCommanderForm.SelectedFilePath);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            CustomDialog.ErrorMessage("Selection is not a file: it cannot be encrypted.", "Error");
        }

        public static void CheckIfDecryptable()
        {
            if (!IsItADirectory(SanityCommanderForm.SelectedFilePath))
            {
                try
                {
                    DecryptFile(SanityCommanderForm.SelectedFilePath);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            CustomDialog.ErrorMessage("Selection is not a file: it cannot be decrypted.", "Error");
        }

        public static void PackFile(FileInfo fi)
        {
            if (fi != null)
            {
                using (FileStream inFile = fi.OpenRead())
                {
                    if ((File.GetAttributes(fi.FullName)
                        & FileAttributes.Hidden)
                        != FileAttributes.Hidden & fi.Extension != ".gz")
                    {
                        using (FileStream outFile =
                                    File.Create(fi.FullName + ".gz"))
                        {
                            using (GZipStream Compress =
                                new GZipStream(outFile,
                                CompressionMode.Compress))
                            {
                                inFile.CopyTo(Compress);

                                Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                                    fi.Name, fi.Length.ToString(), outFile.Length.ToString());
                            }
                        }
                    }
                }
                RefreshNaviBoxContent();
            }
            else { CustomDialog.ErrorMessage("Selection cannot be packed.", "Error"); }
        }

        public static void UnpackFile(FileInfo fi)
        {
            if (fi != null)
            {
                using (FileStream inFile = fi.OpenRead())
                {
                    string curFile = fi.FullName;
                    string origName = curFile.Remove(curFile.Length -
                            fi.Extension.Length);

                    using (FileStream outFile = File.Create(origName))
                    {
                        using (GZipStream Decompress = new GZipStream(inFile,
                                CompressionMode.Decompress))
                        {
                            Decompress.CopyTo(outFile);

                            Console.WriteLine("Decompressed: {0}", fi.Name);
                        }
                    }
                }
                RefreshNaviBoxContent();
            }
            else { CustomDialog.ErrorMessage("Selection is not packed.", "Error"); }
        }
    }
}
