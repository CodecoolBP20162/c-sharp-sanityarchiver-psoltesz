using System.Drawing;
using System.Windows.Forms;

namespace SanityArchiver
{
    public static class CustomDialog
    {
        public static void ErrorMessage(string text, string caption)
        {
            Form newDialog = new Form();
            newDialog.StartPosition = FormStartPosition.CenterParent;
            newDialog.Width = 400;
            newDialog.Height = 140;
            newDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            newDialog.Text = caption;
            newDialog.MinimizeBox = false;
            newDialog.MaximizeBox = false;
            Label textLabel = new Label() { Text = text };
            textLabel.TextAlign = ContentAlignment.MiddleCenter;
            textLabel.Dock = DockStyle.Fill;
            Button confirmation = new Button() { Text = "Ok", Width = 100, Location = new Point(140, 70) };
            confirmation.Click += (sender, e) => { newDialog.Close(); };

            newDialog.Controls.Add(confirmation);
            newDialog.Controls.Add(textLabel);
            newDialog.ShowDialog();
        }
    }
}
