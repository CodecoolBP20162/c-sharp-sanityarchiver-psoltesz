using System;
using System.IO;
using System.Windows.Forms;

namespace SanityArchiver
{
    class NavigatorBox
    {
        public ListView naviBox;
        public string currentDirectoryName;
        DirectoryInfo currentDirectoryInfo;

        public NavigatorBox(ListView naviBox)
        {
            this.naviBox = naviBox;
            currentDirectoryName = Directory.GetCurrentDirectory();
            currentDirectoryInfo = new DirectoryInfo(currentDirectoryName);
        }

        public void Setup()
        {
            naviBox.FullRowSelect = true;
            naviBox.View = View.Details;
            naviBox.GridLines = true;
            AddHeader();
            ListContent();
            naviBox.Columns[0].Width = -2;
            naviBox.Invalidate();
            naviBox.DoubleClick += new EventHandler(NaviBox_MouseDoubleClick);
        }

        public void NavigateOneFolderUp()
        {
            currentDirectoryInfo = Directory.GetParent(Environment.CurrentDirectory);
            currentDirectoryName = currentDirectoryInfo.FullName;
            ListContent();
        }

        private void NaviBox_MouseDoubleClick(object sender, EventArgs e)
        {
            NavigateOneFolderUp();
        }

        private void AddHeader()
        {
            if (naviBox.Columns.Count == 0)
            {
                naviBox.Columns.Add(currentDirectoryName, 20, HorizontalAlignment.Left);
            }
        }

        private void ListContent()
        {
            DirectoryInfo[] subDirectories = currentDirectoryInfo.GetDirectories();
            FileInfo[] files = currentDirectoryInfo.GetFiles();

            ClearContent();

            naviBox.Columns[0].Text = currentDirectoryName;
            naviBox.Items.Add("..");

            foreach (DirectoryInfo item in subDirectories)
            {
                naviBox.Items.Add(item.Name);
            }

            foreach (FileInfo item in files)
            {
                naviBox.Items.Add(item.Name);
            }

            naviBox.Invalidate();
        }

        private void ClearContent()
        {
            foreach (ListViewItem item in naviBox.Items)
            {
                item.Remove();
            }
        }
    }
}
