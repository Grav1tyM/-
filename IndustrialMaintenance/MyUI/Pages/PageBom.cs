using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PageBom : UserControl
    {
        private readonly BomRuleDAL _dal = new BomRuleDAL();
        private readonly PartDAL _partDal = new PartDAL();
        private readonly UIComboBox cboFilter = new UIComboBox();
        private readonly UIDataGridView dgv = new UIDataGridView();
        private readonly UIComboBox cboPartName = new UIComboBox();
        private readonly UITextBox txtCode = new UITextBox();
        private readonly NumericUpDown nudQty = new NumericUpDown();
        private readonly NumericUpDown nudLoss = new NumericUpDown();
        private readonly NumericUpDown nudMin = new NumericUpDown();
        private readonly UITextBox txtRemark = new UITextBox();
        private int _id;

        public PageBom()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;

            Panel left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            cboFilter.Location = new Point(8, 8); cboFilter.Size = new Size(160, 29);
            cboFilter.Items.Add("全部部位");
            foreach (DataRow r in _dal.GetRepairParts().Rows)
                cboFilter.Items.Add(r["RepairPart"].ToString());
            cboFilter.SelectedIndex = 0;
            UIButton btnRefresh = new UIButton { Text = "刷新", Location = new Point(180, 8), Size = new Size(80, 29) };
            btnRefresh.Click += (s, e) => LoadData();
            dgv.Location = new Point(8, 48);
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv.Size = new Size(700, 500);
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgv.BackgroundColor = Color.White;
            dgv.CellClick += Dgv_CellClick;
            left.Controls.Add(cboFilter);
            left.Controls.Add(btnRefresh);
            left.Controls.Add(dgv);
            left.Resize += (s, e) => { dgv.Size = new Size(left.Width - 16, left.Height - 60); };

            Panel right = new Panel { Dock = DockStyle.Right, Width = 280, BackColor = Color.FromArgb(36, 40, 48), Padding = new Padding(12) };
            Label title = new Label { Text = "规则编辑", ForeColor = Color.White, Font = new Font("微软雅黑", 12F, FontStyle.Bold), Location = new Point(12, 16), AutoSize = true };
            int y = 50;
            AddLabel(right, "维修部位", ref y);
            cboPartName.Location = new Point(12, y); cboPartName.Size = new Size(240, 29);
            cboPartName.Items.AddRange(new object[] { "轴承", "电机", "液压系统", "电气线路", "减速机", "皮带传动" });
            cboPartName.Text = "轴承";
            right.Controls.Add(cboPartName); y += 40;
            AddLabel(right, "配件编码", ref y);
            txtCode.Location = new Point(12, y); txtCode.Size = new Size(240, 29); txtCode.Watermark = "如 69000101";
            right.Controls.Add(txtCode); y += 40;
            AddLabel(right, "基准用量（每个单位）", ref y);
            nudQty.Location = new Point(12, y); nudQty.Size = new Size(240, 29); nudQty.DecimalPlaces = 2; nudQty.Maximum = 9999; nudQty.Value = 1;
            right.Controls.Add(nudQty); y += 40;
            AddLabel(right, "损耗率（0.05=5%）", ref y);
            nudLoss.Location = new Point(12, y); nudLoss.Size = new Size(240, 29); nudLoss.DecimalPlaces = 4; nudLoss.Maximum = 1; nudLoss.Increment = 0.01M;
            right.Controls.Add(nudLoss); y += 40;
            AddLabel(right, "最小领用量", ref y);
            nudMin.Location = new Point(12, y); nudMin.Size = new Size(240, 29); nudMin.DecimalPlaces = 2; nudMin.Maximum = 9999;
            right.Controls.Add(nudMin); y += 40;
            AddLabel(right, "备注", ref y);
            txtRemark.Location = new Point(12, y); txtRemark.Size = new Size(240, 60); txtRemark.Multiline = true; txtRemark.Watermark = "选填";
            right.Controls.Add(txtRemark); y += 72;

            UIButton btnAdd = new UIButton { Text = "新增规则", Location = new Point(12, y), Size = new Size(110, 32), FillColor = Color.FromArgb(110, 190, 40) };
            UIButton btnUpd = new UIButton { Text = "更新规则", Location = new Point(130, y), Size = new Size(110, 32) };
            UIButton btnDel = new UIButton { Text = "删除规则", Location = new Point(12, y + 40), Size = new Size(110, 32), FillColor = Color.FromArgb(230, 80, 80) };
            UIButton btnClear = new UIButton { Text = "清空表单", Location = new Point(130, y + 40), Size = new Size(110, 32), FillColor = Color.FromArgb(230, 150, 60) };
            btnAdd.Click += BtnAdd_Click;
            btnUpd.Click += BtnUpd_Click;
            btnDel.Click += BtnDel_Click;
            btnClear.Click += (s, e) => ClearForm();
            right.Controls.AddRange(new Control[] { btnAdd, btnUpd, btnDel, btnClear, title });

            Controls.Add(left);
            Controls.Add(right);
            LoadData();
        }

        private static void AddLabel(Control parent, string text, ref int y)
        {
            parent.Controls.Add(new Label { Text = text, ForeColor = Color.Silver, Location = new Point(12, y), AutoSize = true });
            y += 22;
        }

        private void LoadData()
        {
            try
            {
                string rp = cboFilter.Text == "全部部位" ? null : cboFilter.Text;
                dgv.DataSource = _dal.GetAll(rp);
                string[] h = { "Id", "维修部位", "配件Id", "配件编码", "配件名称", "规格", "基准用量", "损耗率", "最小领用", "单位", "备注" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
                if (dgv.Columns.Contains("PartId")) dgv.Columns["PartId"].Visible = false;
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgv.Rows[e.RowIndex];
            _id = Convert.ToInt32(row.Cells["Id"].Value);
            cboPartName.Text = Convert.ToString(row.Cells["RepairPart"].Value);
            txtCode.Text = Convert.ToString(row.Cells["PartCode"].Value);
            nudQty.Value = Convert.ToDecimal(row.Cells["QuantityPerUnit"].Value);
            nudLoss.Value = Convert.ToDecimal(row.Cells["LossRate"].Value);
            nudMin.Value = Convert.ToDecimal(row.Cells["MinQuantity"].Value);
            txtRemark.Text = Convert.ToString(row.Cells["Remark"].Value);
        }

        private BomRuleEntity Read()
        {
            PartEntity part = _partDal.GetByCode(txtCode.Text.Trim());
            if (part == null) throw new Exception("未找到配件编码：" + txtCode.Text.Trim());
            return new BomRuleEntity
            {
                Id = _id,
                RepairPart = cboPartName.Text.Trim(),
                PartId = part.Id,
                QuantityPerUnit = nudQty.Value,
                LossRate = nudLoss.Value,
                MinQuantity = nudMin.Value,
                Unit = part.Unit,
                Remark = txtRemark.Text.Trim()
            };
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try { _dal.Insert(Read()); UIMessageTip.ShowOk("已新增"); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void BtnUpd_Click(object sender, EventArgs e)
        {
            if (_id <= 0) { UIMessageTip.ShowWarning("请先选中规则"); return; }
            try { _dal.Update(Read()); UIMessageTip.ShowOk("已更新"); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            if (_id <= 0) return;
            if (!UIMessageBox.ShowAsk("确认删除规则？")) return;
            _dal.Delete(_id);
            ClearForm();
            LoadData();
        }

        private void ClearForm()
        {
            _id = 0;
            txtCode.Text = txtRemark.Text = "";
            nudQty.Value = 1;
            nudLoss.Value = 0;
            nudMin.Value = 0;
        }
    }
}
