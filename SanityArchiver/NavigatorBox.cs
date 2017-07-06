using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
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
            CurrentDirectoryPath = @"C:\";
            currentDirectoryInfo = new DirectoryInfo(CurrentDirectoryPath);
        }

        public void Setup()
        {
            NaviBox.FullRowSelect = true;
            NaviBox.View = View.Details;
            NaviBox.AllowColumnReorder = true;
            NaviBox.LabelEdit = true;
            NaviBox.AllowDrop = true;
            AddHeader();
            ListContent();
            NaviBox.Refresh();

            NaviBox.Click += new EventHandler(NaviBoxEvent_Click);
            NaviBox.DoubleClick += new EventHandler(NaviBoxEvent_DoubleClick);
            NaviBox.KeyDown += new KeyEventHandler(NaviBoxEvent_KeyDown);
            NaviBox.KeyUp += new KeyEventHandler(NaviBoxEvent_KeyUp);
            NaviBox.AfterLabelEdit += new LabelEditEventHandler(NaviBoxEvent_AfterLabelEdit);
            NaviBox.DragDrop += new DragEventHandler(NaviBoxEvent_DragDrop);
            NaviBox.DragEnter += new DragEventHandler(NaviBoxEvent_DragEnter);
            NaviBox.ItemDrag += new ItemDragEventHandler(NaviBoxEvent_ItemDrag);
            NaviBox.DragOver += new DragEventHandler(NaviBoxEvent_DragOver);
            NaviBox.DragLeave += new EventHandler(NaviBoxEvent_DragLeave);
        }

        private void ClearContent()
        {
            NaviBox.Items.Clear();
        }

        public void NavigateOneDirectoryUp()
        {
            WillNavigateIntoDirectory(false);
            CurrentDirectoryPath = currentDirectoryInfo.FullName + @"\";
            NavigatorBoxControl.RefreshNaviBoxContent();
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
            try
            {
                NavigatorBoxControl.SelectionPath = CurrentDirectoryPath + NaviBox.SelectedItems[0].Text;
            }
            catch { }
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

        private DirectoryInfo[] GetSubdirectories()
        {
            DirectoryInfo[] subDirectories;
            try
            {
                subDirectories = currentDirectoryInfo.GetDirectories();
                return subDirectories;
            }
            catch (UnauthorizedAccessException)
            {
                CustomDialog.ErrorMessage("Access denied. You might not see every folder you need.", "Error");
                subDirectories = new DirectoryInfo[0];
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
            catch (UnauthorizedAccessException)
            {
                CustomDialog.ErrorMessage("Access denied. You might not see every file you need.", "Error");
                files = new FileInfo[0];
                return files;
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
                    ListContent();
                }
                catch { CustomDialog.ErrorMessage("ERROR: Cannot navigate above disk root.", "Error"); }
            }
        }

        public void ChooseNavigateOrExecute()
        {
            CurrentSelection = new FileInfo(CurrentDirectoryPath + NaviBox.SelectedItems[0].Text);
            if (NavigatorBoxControl.IsItADirectory(CurrentSelection.FullName))
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
                    CustomDialog.ErrorMessage("ERROR: Could not open file.", "Error");
                }
            }
        }

        public void ListContent()
        {
            List<FileSystemInfo> allItems = new List<FileSystemInfo>();
            // if GetSubdirectories of GetFiles return null, problems may happen. They should return an empty array I guess.
            allItems.AddRange(GetSubdirectories());
            allItems.AddRange(GetFiles());

            ClearContent();
            NaviBox.Columns[0].Text = CurrentDirectoryPath;

            if (CurrentDirectoryPath != @"C:\\")
            {
                NaviBox.Items.Add("..");
            }

            var imageList = new ImageList();
            NaviBox.SmallImageList = imageList;
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\Resources\");
            String[] ImageFiles = Directory.GetFiles(path);

            foreach (var file in ImageFiles)
            {
                //Add images to Imagelist
                imageList.Images.Add(Image.FromFile(file));
            }

            foreach (FileSystemInfo item in allItems)
            {
                string[] row = new string[4];
                row[0] = item.Name;

                Color backColor;
                int imageIndex = 0;

                if (item.GetType().Equals(Type.GetType("System.IO.DirectoryInfo")))
                {
                    row[1] = "<DIR>";
                    string size = (String.Format("{0:N0} KB", NavigatorBoxControl.DirSize(new DirectoryInfo(item.FullName)) / 1024));
                    if (size != "0 KB") row[2] = size; else row[2] = "Unknown";
                    row[3] = item.LastWriteTime.ToShortDateString() + item.LastWriteTime.ToLongTimeString();
                    backColor = Color.LightGoldenrodYellow;
                    imageIndex = 1;
                }
                else
                {
                    row[1] = item.Extension.ToUpper();
                    row[2] = String.Format("{0:N0} KB", NavigatorBoxControl.FileSize(new FileInfo(item.FullName)) / 1024);
                    row[3] = item.CreationTime.ToShortDateString() + item.CreationTime.ToLongTimeString();
                    backColor = Color.LightCyan;
                    imageIndex = 0;
                }
                ListViewItem listViewItem = new ListViewItem(row);
                listViewItem.BackColor = backColor;
                listViewItem.ImageIndex = imageIndex;

                NaviBox.Items.Add(listViewItem);
            }

            NaviBox.Items[0].Selected = true;
            NaviBox.Items[0].Focused = true;
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

            if (key == Keys.F2 && NaviBox.SelectedItems.Count > 0)
            {
                NaviBox.SelectedItems[0].BeginEdit();
            }

            if (key == Keys.F3)
            {
                FileAndDirOperations.PackFile(NavigatorBoxControl.MakeFileInfoFromPath());
            }

            if (key == Keys.F4)
            {
                FileAndDirOperations.UnpackFile(NavigatorBoxControl.MakeFileInfoFromPath());
            }

            if (key == Keys.F5)
            {
                NavigatorBoxControl.ShowFileProperties(NavigatorBoxControl.SelectionPath);
            }

            if (key == Keys.Delete)
            {
                if (NavigatorBoxControl.IsItADirectory(NavigatorBoxControl.SelectionPath))
                {
                    FileAndDirOperations.DeleteDirectory(NavigatorBoxControl.SelectionPath);
                }
                FileAndDirOperations.DeleteFile(NavigatorBoxControl.SelectionPath);
            }

            if (key == Keys.Escape)
            {
                Application.Exit();
            }
            return false;
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

        private void NaviBoxEvent_DragLeave(object sender, EventArgs e)
        {
            NaviBox.InsertionMark.Index = -1;
        }

        private void NaviBoxEvent_ItemDrag(object sender, ItemDragEventArgs e)
        {
            NavigatorBoxControl.SelectionPath = CurrentDirectoryPath + NaviBox.SelectedItems[0].Text;
            NaviBox.DoDragDrop(NaviBox.SelectedItems, DragDropEffects.Move);
        }

        private void NaviBoxEvent_DragEnter(object sender, DragEventArgs e)
        {
            int len = e.Data.GetFormats().Length - 1;
            int i;
            for (i = 0; i <= len; i++)
            {
                if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection"))
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void NaviBoxEvent_DragDrop(object sender, DragEventArgs e)
        {
            if (NaviBox.SelectedItems.Count == 0)
            {
                return;
            }
            Point cp = NaviBox.PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = NaviBox.GetItemAt(cp.X, cp.Y);

            if (dragToItem == null)
            {
                return;
            }

            int dragIndex = dragToItem.Index;
            ListViewItem[] selection = new ListViewItem[NaviBox.SelectedItems.Count];

            for (int i = 0; i <= NaviBox.SelectedItems.Count - 1; i++)
            {
                selection[i] = NaviBox.SelectedItems[i];
            }
            for (int i = 0; i < selection.GetLength(0); i++)
            {
                ListViewItem dragItem = selection[i];
                int itemIndex = dragIndex;
                if (itemIndex == dragItem.Index)
                {
                    return;
                }
                if (dragItem.Index < itemIndex)
                    itemIndex++;
                else
                    itemIndex = dragIndex + i;
            }

            int targetIndex = NaviBox.InsertionMark.Index;

            if (targetIndex == -1)
            {
                return;
            }

            if (NaviBox.InsertionMark.AppearsAfterItem)
            {
                targetIndex++;
            }

            if (NavigatorBoxControl.IsItADirectory(NavigatorBoxControl.SelectionPath))
            {
                NavigatorBoxControl.CopyDirectory(NavigatorBoxControl.SelectionPath, NavigatorBoxControl.CopyTarget);
                return;
            }
            NavigatorBoxControl.CopyFile(NavigatorBoxControl.SelectionPath, NavigatorBoxControl.CopyTarget);
        }

        private void NaviBoxEvent_DragOver(object sender, DragEventArgs e)
        {
            Point targetPoint = NaviBox.PointToClient(new Point(e.X, e.Y));

            int targetIndex = NaviBox.InsertionMark.NearestIndex(targetPoint);

            if (targetIndex > -1)
            {
                Rectangle itemBounds = NaviBox.GetItemRect(targetIndex);
                if (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2))
                {
                    NaviBox.InsertionMark.AppearsAfterItem = true;
                }
                else
                {
                    NaviBox.InsertionMark.AppearsAfterItem = false;
                }
            }
            NaviBox.InsertionMark.Index = targetIndex;
            // setting target path to the location of the cursor
            Point cp = NaviBox.PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = NaviBox.GetItemAt(cp.X, cp.Y);
            try
            {
                if (NavigatorBoxControl.IsItADirectory(NavigatorBoxControl.SelectionPath))
                {
                    DirectoryInfo srcDirInfo = new DirectoryInfo(NavigatorBoxControl.SelectionPath);
                    DirectoryInfo tarDirInfo = new DirectoryInfo(dragToItem.ListView.Columns[0].Text);
                    NavigatorBoxControl.CopyTarget = tarDirInfo.FullName + dragToItem.Text + @"\" + srcDirInfo.Name;
                }
                else
                {
                    FileInfo srcFileInfo = new FileInfo(NavigatorBoxControl.SelectionPath);
                    FileInfo tarFileInfo = new FileInfo(dragToItem.ListView.Columns[0].Text);
                    NavigatorBoxControl.CopyTarget = tarFileInfo.FullName + dragToItem.Text + @"\" + srcFileInfo.Name;
                }
            }
            catch { }
        }

        private void NaviBoxEvent_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            string newName = CurrentDirectoryPath + e.Label;

            if (e.Label == null)
                return;
            ASCIIEncoding AE = new ASCIIEncoding();
            char[] temp = e.Label.ToCharArray();

            for (int x = 0; x < temp.Length; x++)
            {
                byte[] bc = AE.GetBytes(temp[x].ToString());

                if (bc[0] > 47 && bc[0] < 58)
                {
                    e.CancelEdit = true;
                    MessageBox.Show("The text for the item cannot contain numerical values.");
                    return;
                }
            }
            if (NavigatorBoxControl.IsItADirectory(NavigatorBoxControl.SelectionPath))
            {
                FileAndDirOperations.RenameDirectory(NavigatorBoxControl.SelectionPath, newName);
            }
            FileAndDirOperations.RenameFile(NavigatorBoxControl.SelectionPath, newName);
            NavigatorBoxControl.SelectionPath = newName;
        }
    }
}
