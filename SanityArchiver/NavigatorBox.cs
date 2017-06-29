﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SanityArchiver
{
    public class NavigatorBox
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
            NaviBox.AllowColumnReorder = true;
            AddHeader();
            ListContent();
            NaviBox.Refresh();
            NaviBox.Click += new EventHandler(NaviBoxEvent_Click);
            NaviBox.DoubleClick += new EventHandler(NaviBoxEvent_DoubleClick);
            NaviBox.KeyDown += new KeyEventHandler(NaviBoxEvent_KeyDown);
            NaviBox.KeyUp += new KeyEventHandler(NaviBoxEvent_KeyUp);
        }

        public void NavigateOneDirectoryUp()
        {
            WillNavigateIntoDirectory(false);
            CurrentDirectoryPath = currentDirectoryInfo.FullName + @"\";
            ListContent();
        }

        private static bool IsItADirectory(string path)
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
            if (IsItADirectory(CurrentSelection.FullName))
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
                    CustomDialogs.ErrorMessage("ERROR: Could not open file.", "Error");
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
                catch { CustomDialogs.ErrorMessage("ERROR: Cannot navigate above disk root.", "Error"); }
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

            if (key == Keys.F5)
            {
                NavigatorBoxStatic.ShowFileProperties(CurrentDirectoryPath + NaviBox.SelectedItems[0].Text);
            }
            
            if (key == Keys.Delete)
            {
                if (IsItADirectory(SanityCommanderForm.selectedFilePath))
                {
                    NavigatorBoxStatic.DeleteDirectory(SanityCommanderForm.selectedFilePath);
                }
                NavigatorBoxStatic.DeleteFile(SanityCommanderForm.selectedFilePath);
            }

            return false;
        }

        public bool KeyUp(Keys key)
        {
            if (key == Keys.Up || key == Keys.Down)
            {
                return true;
            }
            return false;
        }

        private void SelectItem()
        {
            SanityCommanderForm.selectedFilePath = CurrentDirectoryPath + NaviBox.SelectedItems[0].Text;
            Console.WriteLine(SanityCommanderForm.selectedFilePath);
        }

        private void NaviBoxEvent_DoubleClick(object sender, EventArgs e)
        {
            SelectAction();
        }

        private void NaviBoxEvent_Click(object sender, EventArgs e)
        {
            SelectItem();
        }

        private void NaviBoxEvent_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyPressed(e.KeyCode))
            {
                SelectAction();
            }
        }

        private void NaviBoxEvent_KeyUp(object sender, KeyEventArgs e)
        {
            if (KeyUp(e.KeyCode))
            {
                SelectItem();
            }
        }

        private void AddHeader()
        {
            if (NaviBox.Columns.Count == 0)
            {
                NaviBox.Columns.Add(CurrentDirectoryPath, 0, HorizontalAlignment.Left);
                NaviBox.Columns.Add("Ext", 0, HorizontalAlignment.Center);
                NaviBox.Columns.Add("Size", 0, HorizontalAlignment.Center);
                NaviBox.Columns.Add("Creation Date", 0, HorizontalAlignment.Center);
            }
        }

        public void ListContent()
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
                string size = (String.Format("{0:N0} KB", NavigatorBoxStatic.DirSize(new DirectoryInfo(item.FullName)) / 1024));
                if (size != "0 KB") row[2] = size; else row[2] = "Unknown";
                row[3] = item.LastWriteTime.ToShortDateString() + item.LastWriteTime.ToLongTimeString();
                ListViewItem lvitem = new ListViewItem(row);
                lvitem.BackColor = Color.LightGoldenrodYellow;
                NaviBox.Items.Add(lvitem);
            }

            foreach (FileInfo item in files)
            {
                string[] row = new string[4];
                row[0] = item.Name; // Path.GetFileNameWithoutExtension(item.Name);
                row[1] = item.Extension.ToUpper();
                row[2] = String.Format("{0:N0} KB", NavigatorBoxStatic.FileSize(new FileInfo(item.FullName)) / 1024);
                row[3] = item.CreationTime.ToShortDateString() + item.CreationTime.ToLongTimeString();
                ListViewItem lvitem = new ListViewItem(row);
                lvitem.BackColor = Color.LightCyan;
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
                CustomDialogs.ErrorMessage("ERROR: Access denied.", "Error");
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
    }
}
