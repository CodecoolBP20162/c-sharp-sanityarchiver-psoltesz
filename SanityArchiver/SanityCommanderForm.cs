using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;



namespace SanityArchiver
{
    public partial class SanityCommanderForm : Form
    {
        public static List<NavigatorBox> naviBoxes = new List<NavigatorBox>();
        public static string SelectionPath { get; set; }
        public static string TargetPath { get; set; }
        public static string CopyTarget { get; set; }

        public SanityCommanderForm()
        {
            InitializeComponent();
        }

        private void CreateNewNaviBox()
        {
            string newNaviBoxName = "naviBox" + naviBoxes.Count.ToString();
            ListView newListView = new ListView();
            NavigatorBox newNaviBox = new NavigatorBox(newListView);
            newNaviBox.NaviBox.Name = newNaviBoxName;
            newNaviBox.NaviBox.View = View.Details;
            int numberOfNaviboxes = NavigatorBoxOperations.CalculateNumberOfNaviBoxes();
            newNaviBox.NaviBox.Size = new Size(ClientRectangle.Size.Width / (naviBoxes.Count + numberOfNaviboxes), ClientRectangle.Size.Height);
            newNaviBox.NaviBox.Location = new Point(ClientRectangle.Location.X, menuStrip1.Size.Height + 1);
            newNaviBox.Setup();

            naviBoxes.Add(newNaviBox);
            Controls.Add(newNaviBox.NaviBox);

            RefreshNaviboxView();
        }

        private void DeleteNaviBox()
        {
            if (naviBoxes.Count > 1)
            {
                naviBoxes.RemoveAt(naviBoxes.Count - 1);
                string controlToDelete = "naviBox" + (naviBoxes.Count).ToString();
                Control ctn = Controls[controlToDelete];
                Controls.Remove(ctn);
            }
            else
            {
                CustomDialog.ErrorMessage("You only have one panel.", "Error");
            }
            RefreshNaviboxView();
        }

        private void RefreshNaviboxView()
        {
            int startingWidth = CalculateStartingWidth();

            foreach (NavigatorBox item in naviBoxes)
            {
                item.NaviBox.Size = new Size(ClientRectangle.Size.Width / naviBoxes.Count, ClientRectangle.Size.Height - (NavigatorBoxOperations.GetTaskbarHeight() + 10));
                item.NaviBox.Location = new Point(startingWidth, menuStrip1.Size.Height + 1);

                startingWidth = startingWidth + ClientRectangle.Size.Width / naviBoxes.Count;

                int csw = item.NaviBox.ClientSize.Width;
                for (int i = 1; i < item.NaviBox.Columns.Count; i++)
                {
                    item.NaviBox.Columns[i].Width = -1;
                    csw -= item.NaviBox.Columns[i].Width;
                }

                item.NaviBox.Columns[0].Width = csw;
            }
        }

        private int CalculateStartingWidth()
        {
            return ClientRectangle.Location.X / naviBoxes.Count;
        }

        private void SanityCommanderForm_Load(object sender, EventArgs e)
        {
            CreateNewNaviBox();
            CreateNewNaviBox();
            RefreshNaviboxView();
        }

        private void SanityCommanderForm_SizeChanged(object sender, EventArgs e)
        {
            RefreshNaviboxView();
        }

        private void newPanelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CreateNewNaviBox();
        }

        private void closePanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteNaviBox();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigatorBoxOperations.ShowFileProperties(SelectionPath);
        }

        private void packToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigatorBoxOperations.PackFile(NavigatorBoxOperations.MakeFileInfoFromPath());
        }

        private void unpackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigatorBoxOperations.UnpackFile(NavigatorBoxOperations.MakeFileInfoFromPath());
        }

        private void deleteDELToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigatorBoxOperations.DeleteFile(SelectionPath);
        }

        private void encryptFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigatorBoxOperations.CheckIfEncryptable();
        }

        private void decryptFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigatorBoxOperations.CheckIfDecryptable();
        }
    }
}
