using System;
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
            CurrentDirectoryPath = @"C:\\";
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
            NavigatorBoxOperations.RefreshNaviBoxContent();
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
                SanityCommanderForm.SelectedFilePath = CurrentDirectoryPath + NaviBox.SelectedItems[0].Text;
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
                CustomDialog.ErrorMessage("ERROR: Access denied.", "Error");
                currentDirectoryInfo = Directory.GetParent(CurrentDirectoryPath).Parent;
                CurrentDirectoryPath = currentDirectoryInfo.FullName;
                files = null;
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
            if (NavigatorBoxOperations.IsItADirectory(CurrentSelection.FullName))
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

            var imageList = new ImageList();
            NaviBox.SmallImageList = imageList;
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\Resources\");
            String[] ImageFiles = Directory.GetFiles(path);

            foreach (var file in ImageFiles)
            {
                //Add images to Imagelist
                imageList.Images.Add(Image.FromFile(file));
            }

            foreach (DirectoryInfo item in subDirectories)
            {
                string[] row = new string[4];
                row[0] = item.Name;
                row[1] = "<DIR>";
                string size = (String.Format("{0:N0} KB", NavigatorBoxOperations.DirSize(new DirectoryInfo(item.FullName)) / 1024));
                if (size != "0 KB") row[2] = size; else row[2] = "Unknown";
                row[3] = item.LastWriteTime.ToShortDateString() + item.LastWriteTime.ToLongTimeString();
                ListViewItem folderItem = new ListViewItem(row);
                folderItem.BackColor = Color.LightGoldenrodYellow;
                NaviBox.Items.Add(folderItem);
                folderItem.ImageIndex = 1;
            }

            foreach (FileInfo item in files)
            {
                string[] row = new string[4];
                row[0] = item.Name; // Path.GetFileNameWithoutExtension(item.Name);
                row[1] = item.Extension.ToUpper();
                row[2] = String.Format("{0:N0} KB", NavigatorBoxOperations.FileSize(new FileInfo(item.FullName)) / 1024);
                row[3] = item.CreationTime.ToShortDateString() + item.CreationTime.ToLongTimeString();
                ListViewItem fileItem = new ListViewItem(row);
                fileItem.BackColor = Color.LightCyan;
                NaviBox.Items.Add(fileItem);
                fileItem.ImageIndex = 0;
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
                NavigatorBoxOperations.PackFile(NavigatorBoxOperations.MakeFileInfoFromPath());
            }

            if (key == Keys.F4)
            {
                NavigatorBoxOperations.UnpackFile(NavigatorBoxOperations.MakeFileInfoFromPath());
            }

            if (key == Keys.F5)
            {
                NavigatorBoxOperations.ShowFileProperties(SanityCommanderForm.SelectedFilePath);
            }

            if (key == Keys.Delete)
            {
                if (NavigatorBoxOperations.IsItADirectory(SanityCommanderForm.SelectedFilePath))
                {
                    NavigatorBoxOperations.DeleteDirectory(SanityCommanderForm.SelectedFilePath);
                }
                NavigatorBoxOperations.DeleteFile(SanityCommanderForm.SelectedFilePath);
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
            // NavigatorBoxStatic.CopyFile(SanityCommanderForm.SelectedFilePath + @"\" + NaviBox.SelectedItems[0].Text , SanityCommanderForm.TargetPath + @"\" + NaviBox.SelectedItems[0].Text + @"\");
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
                SanityCommanderForm.TargetPath = SanityCommanderForm.SelectedFilePath + @"\" + dragToItem.Text;
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
            if (NavigatorBoxOperations.IsItADirectory(SanityCommanderForm.SelectedFilePath))
            {
                NavigatorBoxOperations.RenameDirectory(SanityCommanderForm.SelectedFilePath, newName);
            }
            NavigatorBoxOperations.RenameFile(SanityCommanderForm.SelectedFilePath, newName);
            SanityCommanderForm.SelectedFilePath = newName;
        }
    }
}
