using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void newPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewNaviBox();
        }

        private void RefreshView()
        {
            int startingWidth = CalculateStartingWidth();

            foreach (NavigatorBox item in naviBoxes)
            {
                item.naviBox.Size = new Size(ClientRectangle.Size.Width / naviBoxes.Count, ClientRectangle.Size.Height);
                item.naviBox.Location = new Point(startingWidth, menuStrip1.Size.Height + 1);
                item.GetCurrentDirectory();
                item.naviBox.Columns[0].Width = -2;
                item.naviBox.Invalidate();

                startingWidth = startingWidth + ClientRectangle.Size.Width / naviBoxes.Count;
            }
        }

        private void SanityCommanderForm_SizeChanged(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void CreateNewNaviBox()
        {
            string newNaviBoxName = "naviBox" + (naviBoxes.Count + 1).ToString();
            ListView newListView = new ListView();
            NavigatorBox newNaviBox = new NavigatorBox(newListView);
            newNaviBox.naviBox.Name = newNaviBoxName;
            newNaviBox.naviBox.View = View.Details;
            if (naviBoxes.Count == 0)
            {
                newNaviBox.naviBox.Size = new Size(ClientRectangle.Size.Width / (naviBoxes.Count + 1), ClientRectangle.Size.Height);
                newNaviBox.naviBox.Location = new Point(ClientRectangle.Location.X, menuStrip1.Size.Height + 1);
            } else
            {
                newNaviBox.naviBox.Size = new Size(ClientRectangle.Size.Width / naviBoxes.Count, ClientRectangle.Size.Height);
                newNaviBox.naviBox.Location = new Point(ClientRectangle.Size.Width / naviBoxes.Count, menuStrip1.Size.Height + 1);
            }
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
            if (naviBoxes.Count == 0)
            {
                return ClientRectangle.Location.X;
            }
            return ClientRectangle.Location.X / naviBoxes.Count;
        }
    }
}
