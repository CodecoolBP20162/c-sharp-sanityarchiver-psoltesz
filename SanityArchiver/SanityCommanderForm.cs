using System;
using System.Drawing;
using System.Windows.Forms;



namespace SanityArchiver
{
    public partial class SanityCommanderForm : Form
    {
        public SanityCommanderForm()
        {
            InitializeComponent();
        }

        private void CreateNewNaviBox()
        {
            string newNaviBoxName = "naviBox" + NavigatorBoxControl.naviBoxes.Count.ToString();
            ListView newListView = new ListView();
            NavigatorBox newNaviBox = new NavigatorBox(newListView);
            newNaviBox.NaviBox.Name = newNaviBoxName;
            newNaviBox.NaviBox.View = View.Details;
            newNaviBox.Setup();

            NavigatorBoxControl.naviBoxes.Add(newNaviBox);
            Controls.Add(newNaviBox.NaviBox);

            RefreshNaviboxView();
        }

        private void DeleteNaviBox()
        {
            if (NavigatorBoxControl.naviBoxes.Count > 1)
            {
                NavigatorBoxControl.naviBoxes.RemoveAt(NavigatorBoxControl.naviBoxes.Count - 1);
                string controlToDelete = "naviBox" + (NavigatorBoxControl.naviBoxes.Count).ToString();
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

            foreach (NavigatorBox item in NavigatorBoxControl.naviBoxes)
            {
                item.NaviBox.Size = new Size(ClientRectangle.Size.Width / NavigatorBoxControl.naviBoxes.Count, ClientRectangle.Size.Height - (NavigatorBoxControl.GetTaskbarHeight() + 10));
                item.NaviBox.Location = new Point(startingWidth, menuStrip1.Size.Height + 1);

                startingWidth = startingWidth + ClientRectangle.Size.Width / NavigatorBoxControl.naviBoxes.Count;

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
            return ClientRectangle.Location.X / NavigatorBoxControl.naviBoxes.Count;
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
            NavigatorBoxControl.ShowFileProperties(NavigatorBoxControl.SelectionPath);
        }

        private void packToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileAndDirOperations.PackFile(NavigatorBoxControl.MakeFileInfoFromPath());
        }

        private void unpackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileAndDirOperations.UnpackFile(NavigatorBoxControl.MakeFileInfoFromPath());
        }

        private void deleteDELToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileAndDirOperations.DeleteFile(NavigatorBoxControl.SelectionPath);
        }

        private void encryptFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigatorBoxControl.CheckIfEncryptable();
        }

        private void decryptFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavigatorBoxControl.CheckIfDecryptable();
        }
    }
}
