using System;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PageBom : UserControl
    {
        private readonly BomRuleDAL _dal = new BomRuleDAL();
        private readonly UITextBox txtFilter = new UITextBox();
        private readonly UIDataGridView dgv = new UIDataGridView();

        public PageBom()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;
            Panel bar = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };
            txtFilter.Location = new Point(8, 10); txtFilter.Size = new Size(180, 29); txtFilter.Watermark = "按维修部位筛选";
            UIButton btn = new UIButton { Text = "查询", Location = new Point(200, 10), Size = new Size(80, 29) };
            UIButton btnAll = new UIButton { Text = "全部", Location = new Point(290, 10), Size = new Size(80, 29) };
            btn.Click += (s, e) => LoadData();
            btnAll.Click += (s, e) => { txtFilter.Text = ""; LoadData(); };
            bar.Controls.AddRange(new Control[] { txtFilter, btn, btnAll });
            dgv.Dock = DockStyle.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            Controls.Add(dgv);
            Controls.Add(bar);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string rp = string.IsNullOrWhiteSpace(txtFilter.Text) ? null : txtFilter.Text.Trim();
                dgv.DataSource = _dal.GetAll(rp);
                string[] h = { "Id", "维修部位", "配件Id", "配件编码", "配件名称", "基准用量", "损耗率", "最小领用", "单位", "备注" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
                if (dgv.Columns.Contains("PartId")) dgv.Columns["PartId"].Visible = false;
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }
    }
}
