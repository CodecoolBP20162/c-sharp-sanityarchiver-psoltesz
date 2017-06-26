using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanityArchiver
{
    class NavigatorBox
    {
        public ListView naviBox;

        public NavigatorBox(ListView naviBox)
        {
            this.naviBox = naviBox;
        }

        public void NavigateToFolder()
        {
          
        }

        public void GetCurrentDirectory()
        {
            if (naviBox.Columns.Count == 0)
            {
                string directory = Directory.GetCurrentDirectory();
                naviBox.Columns.Add(directory, 20, HorizontalAlignment.Left);
            }
        }
    }
}
