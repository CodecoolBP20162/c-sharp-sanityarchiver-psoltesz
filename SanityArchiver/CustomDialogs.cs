using System.Drawing;
using System.Windows.Forms;

namespace SanityArchiver
{
    public static class CustomDialogs
    {
        public static int ShowDialog(string text, string caption)
        {
            Form newDialog = new Form();
            newDialog.StartPosition = FormStartPosition.CenterParent;
            newDialog.Width = 500;
            newDialog.Height = 100;
            newDialog.Text = caption;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            NumericUpDown inputBox = new NumericUpDown() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { newDialog.Close(); };
            newDialog.Controls.Add(confirmation);
            newDialog.Controls.Add(textLabel);
            newDialog.Controls.Add(inputBox);
            newDialog.ShowDialog();
            return (int)inputBox.Value;
        }

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
