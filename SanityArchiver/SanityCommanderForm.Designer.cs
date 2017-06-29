namespace SanityArchiver
{
    partial class SanityCommanderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SanityCommanderForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unpackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closePanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameF2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteDELToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem,
            this.newPanelToolStripMenuItem,
            this.closePanelToolStripMenuItem,
            this.renameF2ToolStripMenuItem,
            this.packToolStripMenuItem,
            this.unpackToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.deleteDELToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1084, 50);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem
            // 
            this.toolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.toolStripMenuItem.Name = "toolStripMenuItem";
            this.toolStripMenuItem.Size = new System.Drawing.Size(37, 46);
            this.toolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // packToolStripMenuItem
            // 
            this.packToolStripMenuItem.Name = "packToolStripMenuItem";
            this.packToolStripMenuItem.Size = new System.Drawing.Size(67, 46);
            this.packToolStripMenuItem.Text = "Pack (F3)";
            this.packToolStripMenuItem.Click += new System.EventHandler(this.packToolStripMenuItem_Click);
            // 
            // unpackToolStripMenuItem
            // 
            this.unpackToolStripMenuItem.Name = "unpackToolStripMenuItem";
            this.unpackToolStripMenuItem.Size = new System.Drawing.Size(82, 46);
            this.unpackToolStripMenuItem.Text = "Unpack (F4)";
            this.unpackToolStripMenuItem.Click += new System.EventHandler(this.unpackToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(95, 46);
            this.propertiesToolStripMenuItem.Text = "Properties (F5)";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // newPanelToolStripMenuItem
            // 
            this.newPanelToolStripMenuItem.Name = "newPanelToolStripMenuItem";
            this.newPanelToolStripMenuItem.Size = new System.Drawing.Size(98, 46);
            this.newPanelToolStripMenuItem.Text = "New Panel (F1)";
            this.newPanelToolStripMenuItem.Click += new System.EventHandler(this.newPanelToolStripMenuItem1_Click);
            // 
            // closePanelToolStripMenuItem
            // 
            this.closePanelToolStripMenuItem.Name = "closePanelToolStripMenuItem";
            this.closePanelToolStripMenuItem.Size = new System.Drawing.Size(142, 46);
            this.closePanelToolStripMenuItem.Text = "Close Panel (CTRL+ F1)";
            this.closePanelToolStripMenuItem.Click += new System.EventHandler(this.closePanelToolStripMenuItem_Click);
            // 
            // renameF2ToolStripMenuItem
            // 
            this.renameF2ToolStripMenuItem.Name = "renameF2ToolStripMenuItem";
            this.renameF2ToolStripMenuItem.Size = new System.Drawing.Size(85, 46);
            this.renameF2ToolStripMenuItem.Text = "Rename (F2)";
            // 
            // deleteDELToolStripMenuItem
            // 
            this.deleteDELToolStripMenuItem.Name = "deleteDELToolStripMenuItem";
            this.deleteDELToolStripMenuItem.Size = new System.Drawing.Size(83, 46);
            this.deleteDELToolStripMenuItem.Text = "Delete (DEL)";
            this.deleteDELToolStripMenuItem.Click += new System.EventHandler(this.deleteDELToolStripMenuItem_Click);
            // 
            // SanityCommanderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 537);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SanityCommanderForm";
            this.Text = "Sanity Commander";
            this.Load += new System.EventHandler(this.SanityCommanderForm_Load);
            this.SizeChanged += new System.EventHandler(this.SanityCommanderForm_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unpackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newPanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closePanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameF2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteDELToolStripMenuItem;
    }
}

