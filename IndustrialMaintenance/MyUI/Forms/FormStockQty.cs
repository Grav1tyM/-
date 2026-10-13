using System;
using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace MyUI.Forms
{
    public class FormStockQty : UIForm
    {
        private readonly NumericUpDown nud = new NumericUpDown();
        public decimal Quantity { get { return nud.Value; } }

        public FormStockQty(string title)
        {
            Text = title;
            ClientSize = new Size(320, 160);
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Label l = new Label { Text = "数量", Location = new Point(30, 55), AutoSize = true };
            nud.Location = new Point(80, 50); nud.Size = new Size(160, 29);
            nud.Minimum = 0.01M; nud.Maximum = 999999; nud.DecimalPlaces = 2; nud.Value = 1;
            UIButton ok = new UIButton { Text = "确定", Location = new Point(60, 100), Size = new Size(90, 32) };
            UIButton cancel = new UIButton { Text = "取消", Location = new Point(170, 100), Size = new Size(90, 32) };
            ok.Click += (s, e) => DialogResult = DialogResult.OK;
            cancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.AddRange(new Control[] { l, nud, ok, cancel });
        }
    }
}
