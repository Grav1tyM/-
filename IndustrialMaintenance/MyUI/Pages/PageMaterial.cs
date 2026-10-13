using System;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PageMaterial : UserControl
    {
        private readonly MaterialRequirementDAL _dal = new MaterialRequirementDAL();
        private readonly UITextBox txtKeyword = new UITextBox();
        private readonly UIDataGridView dgv = new UIDataGridView();

        public PageMaterial()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;
            Panel bar = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };
            txtKeyword.Location = new Point(8, 10); txtKeyword.Size = new Size(220, 29);
            txtKeyword.Watermark = "工单号/配件/状态";
            UIButton btnSearch = new UIButton { Text = "查询", Location = new Point(240, 10), Size = new Size(80, 29) };
            UIButton btnIssue = new UIButton { Text = "领料出库", Location = new Point(330, 10), Size = new Size(100, 29) };
            UIButton btnCancel = new UIButton { Text = "取消待领", Location = new Point(440, 10), Size = new Size(100, 29), FillColor = Color.FromArgb(230, 150, 60) };
            btnSearch.Click += (s, e) => LoadData();
            btnIssue.Click += BtnIssue_Click;
            btnCancel.Click += BtnCancel_Click;
            bar.Controls.AddRange(new Control[] { txtKeyword, btnSearch, btnIssue, btnCancel });
            dgv.Dock = DockStyle.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgv.BackgroundColor = Color.White;
            Controls.Add(dgv);
            Controls.Add(bar);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = _dal.GetAll(txtKeyword.Text.Trim());
                string[] h = { "Id", "工单号", "配件Id", "配件编码", "配件名称", "规格", "单位", "应领数量", "库存快照", "缺料", "状态", "备注", "生成时间" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
                if (dgv.Columns.Contains("PartId")) dgv.Columns["PartId"].Visible = false;
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void BtnIssue_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            try { _dal.Issue(id); UIMessageTip.ShowOk("领料成功"); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            _dal.Cancel(id);
            LoadData();
        }
    }
}
