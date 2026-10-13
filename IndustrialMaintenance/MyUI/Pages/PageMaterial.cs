using System;
using System.IO;
using System.Text;
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
            Panel bar = new Panel { Dock = DockStyle.Top, Height = 48 };
            txtKeyword.Location = new Point(8, 10);
            txtKeyword.Size = new Size(220, 29);
            txtKeyword.Watermark = "如 WO20260921001";
            UIButton btnSearch = new UIButton { Text = "查询", Location = new Point(240, 10), Size = new Size(80, 29) };
            UIButton btnRefresh = new UIButton { Text = "刷新", Location = new Point(330, 10), Size = new Size(80, 29) };
            UIButton btnIssue = new UIButton { Text = "确认领料", Location = new Point(430, 10), Size = new Size(100, 29), FillColor = Color.FromArgb(110, 190, 40) };
            UIButton btnCancel = new UIButton { Text = "取消清单", Location = new Point(540, 10), Size = new Size(100, 29), FillColor = Color.FromArgb(230, 150, 60) };
            UIButton btnCsv = new UIButton { Text = "导出CSV", Location = new Point(650, 10), Size = new Size(100, 29) };
            btnSearch.Click += (s, e) => LoadData();
            btnRefresh.Click += (s, e) => { txtKeyword.Text = ""; LoadData(); };
            btnIssue.Click += BtnIssue_Click;
            btnCancel.Click += BtnCancel_Click;
            btnCsv.Click += BtnCsv_Click;
            bar.Controls.AddRange(new Control[] { txtKeyword, btnSearch, btnRefresh, btnIssue, btnCancel, btnCsv });

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
                if (dgv.Columns.Contains("Status"))
                {
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        string st = Convert.ToString(row.Cells["Status"].Value);
                        if (st == "Pending") row.Cells["Status"].Value = "待领";
                        else if (st == "Issued") row.Cells["Status"].Value = "已领";
                        else if (st == "Cancelled") row.Cells["Status"].Value = "已取消";
                        string sh = Convert.ToString(row.Cells["IsShortage"].Value);
                        if (sh == "1") row.Cells["IsShortage"].Value = "缺料";
                        else if (sh == "0") row.Cells["IsShortage"].Value = "充足";
                    }
                }
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private int? SelectedId()
        {
            if (dgv.CurrentRow == null) return null;
            return Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
        }

        private void BtnIssue_Click(object sender, EventArgs e)
        {
            int? id = SelectedId();
            if (id == null) return;
            try { _dal.Issue(id.Value); UIMessageTip.ShowOk("领料成功"); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            int? id = SelectedId();
            if (id == null) return;
            _dal.Cancel(id.Value);
            LoadData();
        }

        private void BtnCsv_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) { UIMessageTip.ShowWarning("没有数据"); return; }
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV|*.csv";
                dlg.FileName = "领料清单.csv";
                if (dlg.ShowDialog() != DialogResult.OK) return;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    if (!dgv.Columns[i].Visible) continue;
                    if (sb.Length > 0 && !sb.ToString().EndsWith("\r\n") && i > 0) sb.Append(",");
                    // simpler rebuild
                }
                sb.Clear();
                bool first = true;
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (!col.Visible) continue;
                    if (!first) sb.Append(",");
                    sb.Append(col.HeaderText);
                    first = false;
                }
                sb.AppendLine();
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    first = true;
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        if (!col.Visible) continue;
                        if (!first) sb.Append(",");
                        string v = Convert.ToString(row.Cells[col.Index].Value) ?? "";
                        if (v.Contains(",") || v.Contains("\"")) v = "\"" + v.Replace("\"", "\"\"") + "\"";
                        sb.Append(v);
                        first = false;
                    }
                    sb.AppendLine();
                }
                File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));
                UIMessageTip.ShowOk("已导出");
            }
        }
    }
}
