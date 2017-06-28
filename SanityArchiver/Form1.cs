using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SanityArchiver
{
    public partial class SanityCommanderForm : Form
    {
        List<NavigatorBox> naviBoxes = new List<NavigatorBox>();

        public SanityCommanderForm()
        {
            InitializeComponent();
        }

        private void SanityCommanderForm_Load(object sender, EventArgs e)
        {
            CreateNewNaviBox();
            CreateNewNaviBox();
            RefreshView();
        }

        private void SanityCommanderForm_SizeChanged(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            int startingWidth = CalculateStartingWidth();

            foreach (NavigatorBox item in naviBoxes)
            {
                item.NaviBox.Size = new Size(ClientRectangle.Size.Width / naviBoxes.Count, ClientRectangle.Size.Height);
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

        private void CreateNewNaviBox()
        {
            string newNaviBoxName = "naviBox" + (naviBoxes.Count + 1).ToString();
            ListView newListView = new ListView();
            NavigatorBox newNaviBox = new NavigatorBox(newListView);
            newNaviBox.NaviBox.Name = newNaviBoxName;
            newNaviBox.NaviBox.View = View.Details;
            int numberOfNaviboxes = CalculateNumberOfNaviBoxes();
            newNaviBox.NaviBox.Size = new Size(ClientRectangle.Size.Width / (naviBoxes.Count + numberOfNaviboxes), ClientRectangle.Size.Height);
            newNaviBox.NaviBox.Location = new Point(ClientRectangle.Location.X, menuStrip1.Size.Height + 1);
            newNaviBox.Setup();

            naviBoxes.Add(newNaviBox);
            Controls.Add(newNaviBox.NaviBox);

            RefreshView();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private int CalculateStartingWidth()
        {
            return ClientRectangle.Location.X / naviBoxes.Count;
        }

        private int CalculateNumberOfNaviBoxes()
        {
            if (naviBoxes.Count == 0)
            {
                return 1;
            }
            return 0;
        }

        private void newPanelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CreateNewNaviBox();
        }
    }
}
