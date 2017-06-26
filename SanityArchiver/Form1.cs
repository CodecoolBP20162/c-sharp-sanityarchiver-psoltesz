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
                item.naviBox.Size = new Size(ClientRectangle.Size.Width / naviBoxes.Count, ClientRectangle.Size.Height);
                item.naviBox.Location = new Point(startingWidth, menuStrip1.Size.Height + 1);

                startingWidth = startingWidth + ClientRectangle.Size.Width / naviBoxes.Count;
            }
        }

        private void CreateNewNaviBox()
        {
            string newNaviBoxName = "naviBox" + (naviBoxes.Count + 1).ToString();
            ListView newListView = new ListView();
            NavigatorBox newNaviBox = new NavigatorBox(newListView);
            newNaviBox.naviBox.Name = newNaviBoxName;
            newNaviBox.naviBox.View = View.Details;
            int numberOfNaviboxes = CalculateNumberOfNaviBoxes();
            newNaviBox.naviBox.Size = new Size(ClientRectangle.Size.Width / (naviBoxes.Count + numberOfNaviboxes), ClientRectangle.Size.Height);
            newNaviBox.naviBox.Location = new Point(ClientRectangle.Location.X, menuStrip1.Size.Height + 1);
            newNaviBox.Setup();

            naviBoxes.Add(newNaviBox);
            Controls.Add(newNaviBox.naviBox);

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
