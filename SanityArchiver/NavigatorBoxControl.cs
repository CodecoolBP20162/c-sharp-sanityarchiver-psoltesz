using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SanityArchiver
{
    class NavigatorBoxControl
    {
        public static List<NavigatorBox> naviBoxes = new List<NavigatorBox>();
        public static string SelectionPath { get; set; }
        public static string TargetPath { get; set; }
        public static string CopyTarget { get; set; }

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
            foreach (NavigatorBox item in naviBoxes)
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

        public static long FileSize(FileInfo f)
        {
            long size = 0;
            size += f.Length;
            return size;
        }

        public static void CopyFile(string sourcePath, string targetPath)
        {
            try
            {
                File.Copy(sourcePath, targetPath, true);
                RefreshNaviBoxContent();
            }
            catch { CustomDialog.ErrorMessage("Copy unsuccessful.", "Error"); }
        }

        public static void CopyDirectory(string sourcePath, string targetPath)
        {
            try
            {
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                string fileName;
                string destFile;
                string[] files = Directory.GetFiles(sourcePath);
                foreach (string s in files)
                {
                    fileName = Path.GetFileName(s);
                    destFile = Path.Combine(targetPath, fileName);
                    File.Copy(s, destFile, true);
                }
                RefreshNaviBoxContent();
            }
            catch { CustomDialog.ErrorMessage("Copy unsuccessful.", "Error"); }
        }

        public static FileInfo MakeFileInfoFromPath()
        {
            FileInfo fi = null;
            try
            {
                fi = new FileInfo(SelectionPath);
            }
            catch { }
            return fi;
        }

        public static void CheckIfEncryptable()
        {
            if (!IsItADirectory(SelectionPath))
            {
                try
                {
                    FileAndDirOperations.EncryptFile(SelectionPath);
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
            if (!IsItADirectory(SelectionPath))
            {
                try
                {
                    FileAndDirOperations.DecryptFile(SelectionPath);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            CustomDialog.ErrorMessage("Selection is not a file: it cannot be decrypted.", "Error");
        }
    }
}
