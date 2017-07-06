using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SanityArchiver
{
    class FileAndDirOperations
    {
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
                NavigatorBoxControl.RefreshNaviBoxContent();
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
                NavigatorBoxControl.RefreshNaviBoxContent();
            }
            else { CustomDialog.ErrorMessage("Selection is not packed.", "Error"); }
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
                NavigatorBoxControl.RefreshNaviBoxContent();
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
                NavigatorBoxControl.RefreshNaviBoxContent();
            }
            catch { CustomDialog.ErrorMessage("Rename unsuccessful.", "Error"); }
        }

        public static void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch { CustomDialog.ErrorMessage("Error: Access denied.", "Error"); }
            NavigatorBoxControl.RefreshNaviBoxContent();
        }

        public static void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path);
            }
            catch { CustomDialog.ErrorMessage("Error: Access denied.", "Error"); }
            NavigatorBoxControl.RefreshNaviBoxContent();
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
    }
}
