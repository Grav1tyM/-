using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PagePlaceholder : UserControl
    {
        public PagePlaceholder(string title, string tip)
        {
            BackColor = Color.FromArgb(40, 44, 52);
            var lbl = new UILabel
            {
                Text = title + "\r\n\r\n" + tip,
                Font = new Font("微软雅黑", 14F),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Style = UIStyle.Custom
            };
            Controls.Add(lbl);
        }
    }
}
