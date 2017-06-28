using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SanityArchiver
{
    class NavigatorBox
    {
        public ListView NaviBox { get; set; }
        public string CurrentDirectoryPath { get; set; }
        public FileInfo CurrentSelection { get; set; }
        DirectoryInfo currentDirectoryInfo;

        public NavigatorBox(ListView naviBox)
        {
            NaviBox = naviBox;
            CurrentDirectoryPath = @"C:\\";
            currentDirectoryInfo = new DirectoryInfo(CurrentDirectoryPath);
        }

        public void Setup()
        {
            NaviBox.FullRowSelect = true;
            NaviBox.View = View.Details;
            NaviBox.GridLines = true;
            NaviBox.AllowColumnReorder = true;
            AddHeader();
            ListContent();
            NaviBox.Refresh();
            NaviBox.DoubleClick += new EventHandler(NaviBoxEvent_DoubleClick);
            NaviBox.KeyDown += new KeyEventHandler(NaviBoxEvent_KeyDown);
        }

        public void NavigateOneDirectoryUp()
        {
            WillNavigateIntoDirectory(false);
            CurrentDirectoryPath = currentDirectoryInfo.FullName + @"\";
            ListContent();
        }

        private static bool IsItAFolder(string path)
        {
            FileAttributes fa = File.GetAttributes(path);
            bool isDirectory = false;
            if ((fa & FileAttributes.Directory) != 0)
            {
                isDirectory = true;
            }
            return isDirectory;
        }

        public void ChooseNavigateOrExecute()
        {
            CurrentSelection = new FileInfo(CurrentDirectoryPath + NaviBox.SelectedItems[0].Text);
            if (IsItAFolder(CurrentSelection.FullName))
            {
                WillNavigateIntoDirectory(true);
                currentDirectoryInfo = new DirectoryInfo(CurrentDirectoryPath);
                ListContent();
            }
            else
            {
                currentDirectoryInfo = Directory.GetParent(CurrentDirectoryPath);
                try
                {
                    Process.Start(CurrentSelection.FullName);
                }
                catch
                {
                    MessageBox.Show("Error: Could not open file.");
                }
            }
        }

        private void WillNavigateIntoDirectory(bool navigateIn)
        {
            if (navigateIn)
            {
                CurrentDirectoryPath = CurrentDirectoryPath + NaviBox.SelectedItems[0].Text + @"\";
            }
            else
            {
                try
                {
                    currentDirectoryInfo = Directory.GetParent(CurrentDirectoryPath).Parent;
                }
                catch { MessageBox.Show("Error: Cannot navigate above root."); }
            }
        }

        private void SelectAction()
        {
            if (NaviBox.SelectedItems[0].Text == NaviBox.Items[0].Text && NaviBox.Items[0].Text == "..")
            {
                NavigateOneDirectoryUp();
            }
            else
            {
                ChooseNavigateOrExecute();
            }
        }

        public bool KeyPressed(Keys key)
        {
            if (key == Keys.Enter)
            {
                return true;
            }

            if (key == Keys.Back)
            {
                NavigateOneDirectoryUp();
            }
            return false;
        }

        private void NaviBoxEvent_DoubleClick(object sender, EventArgs e)
        {
            SelectAction();
        }

        private void NaviBoxEvent_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyPressed(e.KeyCode))
            {
                SelectAction();
            }
        }

        private void AddHeader()
        {
            if (NaviBox.Columns.Count == 0)
            {
                NaviBox.Columns.Add(CurrentDirectoryPath, 0, HorizontalAlignment.Left);
                NaviBox.Columns.Add("Ext", 0, HorizontalAlignment.Center);
                NaviBox.Columns.Add("Size", 0, HorizontalAlignment.Center);
                NaviBox.Columns.Add("Date", 0, HorizontalAlignment.Center);
            }
        }

        private void ListContent()
        {
            DirectoryInfo[] subDirectories = GetSubdirectories();
            FileInfo[] files = GetFiles();

            if (subDirectories == null || files == null)
                return;

            ClearContent();
            NaviBox.Columns[0].Text = CurrentDirectoryPath;

            if (CurrentDirectoryPath != @"C:\\")
            {
                NaviBox.Items.Add("..");
            }

            foreach (DirectoryInfo item in subDirectories)
            {
                string[] row = new string[4];
                row[0] = item.Name;
                row[1] = "<DIR>";
                string size = (String.Format("{0:N0} KB", DirSize(new DirectoryInfo(item.FullName)) / 1024));
                if (size != "0 KB") row[2] = size; else row[2] = "Unknown";
                row[3] = item.LastWriteTime.ToShortDateString() + item.LastWriteTime.ToLongTimeString();
                ListViewItem lvitem = new ListViewItem(row);
                NaviBox.Items.Add(lvitem);
            }

            foreach (FileInfo item in files)
            {
                string[] row = new string[4];
                row[0] = item.Name;
                row[1] = item.Extension.ToUpper();
                row[2] = String.Format("{0:N0} KB", FileSize(new FileInfo(item.FullName)) / 1024);
                row[3] = item.LastWriteTime.ToShortDateString() + item.LastWriteTime.ToLongTimeString();
                ListViewItem lvitem = new ListViewItem(row);
                NaviBox.Items.Add(lvitem);
            }

            NaviBox.Items[0].Selected = true;
            NaviBox.Items[0].Focused = true;
        }

        private DirectoryInfo[] GetSubdirectories()
        {
            DirectoryInfo[] subDirectories;
            try
            {
                subDirectories = currentDirectoryInfo.GetDirectories();
                return subDirectories;
            }
            catch
            {
                subDirectories = null;
                return subDirectories;
            }
        }

        private FileInfo[] GetFiles()
        {
            FileInfo[] files;
            try
            {
                files = currentDirectoryInfo.GetFiles();
                return files;
            }
            catch
            {
                MessageBox.Show("ERROR: Access denied.");
                currentDirectoryInfo = Directory.GetParent(CurrentDirectoryPath).Parent;
                CurrentDirectoryPath = currentDirectoryInfo.FullName;
                files = null;
                return files;
            }
        }

        private void ClearContent()
        {
            NaviBox.Items.Clear();
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

        public static long FileSize(FileInfo f)
        {
            long size = 0;
            size += f.Length;
            return size;
        }
    }
}
