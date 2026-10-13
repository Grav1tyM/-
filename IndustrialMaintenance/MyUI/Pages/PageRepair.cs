using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Pages
{
    /// <summary>
    /// 报修识别页（对齐截图：左侧表单 + 右侧照片 + 底部算料结果）
    /// </summary>
    public class PageRepair : UserControl
    {
        private readonly WorkOrderDAL _woDal = new WorkOrderDAL();
        private readonly MaterialRequirementDAL _matDal = new MaterialRequirementDAL();
        private readonly UITextBox txtDeviceNo = new UITextBox();
        private readonly UITextBox txtDeviceName = new UITextBox();
        private readonly UITextBox txtFault = new UITextBox();
        private readonly UITextBox txtReporter = new UITextBox();
        private readonly UIComboBox cboPriority = new UIComboBox();
        private readonly NumericUpDown nudQty = new NumericUpDown();
        private readonly UITextBox txtRepairPart = new UITextBox();
        private readonly PictureBox pic = new PictureBox();
        private readonly UILabel lblAi = new UILabel();
        private readonly UIDataGridView dgv = new UIDataGridView();
        private string _photoPath;
        private string _orderNo;

        public PageRepair()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;

            Panel left = new Panel { Dock = DockStyle.Left, Width = 360, Padding = new Padding(8) };
            UIGroupBox g1 = new UIGroupBox
            {
                Text = "① 填写报修信息",
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(48, 52, 60),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 11F)
            };
            int y = 36;
            AddField(g1, "设备编号", txtDeviceNo, ref y, "如 DEV-001 (支持扫码枪扫描)");
            AddField(g1, "设备名称", txtDeviceName, ref y, "如 3号传送带电机");
            AddField(g1, "故障描述", txtFault, ref y, "请描述设备故障现象", 60);
            AddField(g1, "报修人", txtReporter, ref y, "");
            txtReporter.Text = SessionContext.DisplayName;
            Label lp = new Label { Text = "优先级", ForeColor = Color.White, Location = new Point(12, y), AutoSize = true };
            cboPriority.Location = new Point(90, y - 4); cboPriority.Size = new Size(240, 29);
            cboPriority.Items.AddRange(new object[] { "Normal", "Urgent" });
            cboPriority.Text = "Normal";
            g1.Controls.Add(lp); g1.Controls.Add(cboPriority); y += 40;
            Label lq = new Label { Text = "维修数量", ForeColor = Color.White, Location = new Point(12, y), AutoSize = true };
            nudQty.Location = new Point(90, y - 4); nudQty.Size = new Size(80, 29); nudQty.Minimum = 1; nudQty.Value = 1;
            g1.Controls.Add(lq); g1.Controls.Add(nudQty); y += 40;
            AddField(g1, "维修部位", txtRepairPart, ref y, "AI识别或手工填写，如 轴承");

            UIButton btnPhoto = new UIButton { Text = "选择照片", Location = new Point(12, y), Size = new Size(100, 34) };
            UIButton btnAi = new UIButton { Text = "AI 识别", Location = new Point(122, y), Size = new Size(100, 34) };
            UIButton btnReset = new UIButton { Text = "重置", Location = new Point(232, y), Size = new Size(90, 34), FillColor = Color.FromArgb(230, 150, 60) };
            btnPhoto.Click += BtnPhoto_Click;
            btnAi.Click += BtnAi_Click;
            btnReset.Click += (s, e) => ResetForm();
            g1.Controls.Add(btnPhoto); g1.Controls.Add(btnAi); g1.Controls.Add(btnReset);
            left.Controls.Add(g1);

            Panel right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            UIGroupBox g2 = new UIGroupBox
            {
                Text = "② 维修部位照片",
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(48, 52, 60),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 11F)
            };
            pic.Dock = DockStyle.Fill;
            pic.BackColor = Color.Black;
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            lblAi.Text = "识别结果：等待识别...";
            lblAi.Dock = DockStyle.Bottom;
            lblAi.Height = 32;
            lblAi.ForeColor = Color.Orange;
            lblAi.Style = UIStyle.Custom;
            g2.Controls.Add(pic);
            g2.Controls.Add(lblAi);
            right.Controls.Add(g2);

            Panel bottom = new Panel { Dock = DockStyle.Bottom, Height = 280, Padding = new Padding(8) };
            UIButton btnCalc = new UIButton { Text = "智能算料", Location = new Point(8, 8), Size = new Size(120, 34), FillColor = Color.FromArgb(110, 190, 40) };
            UIButton btnCreate = new UIButton { Text = "生成工单与领料清单", Location = new Point(140, 8), Size = new Size(180, 34) };
            btnCalc.Click += BtnCalc_Click;
            btnCreate.Click += BtnCreate_Click;
            dgv.Location = new Point(8, 50);
            dgv.Size = new Size(Width - 40, 210);
            dgv.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            bottom.Controls.Add(btnCalc);
            bottom.Controls.Add(btnCreate);
            bottom.Controls.Add(dgv);

            Controls.Add(right);
            Controls.Add(left);
            Controls.Add(bottom);
        }

        private static void AddField(Control parent, string label, UITextBox box, ref int y, string tip, int h = 29)
        {
            Label l = new Label { Text = label, ForeColor = Color.White, Location = new Point(12, y), AutoSize = true };
            box.Location = new Point(90, y - 4);
            box.Size = new Size(240, h);
            box.Watermark = tip;
            if (h > 40) box.Multiline = true;
            parent.Controls.Add(l);
            parent.Controls.Add(box);
            y += h + 12;
        }

        private void BtnPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "图片|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _photoPath = dlg.FileName;
                    pic.ImageLocation = _photoPath;
                }
            }
        }

        private void BtnAi_Click(object sender, EventArgs e)
        {
            // 演示：根据故障描述关键词粗识别部位（可后续接真实模型）
            string fault = (txtFault.Text ?? "") + " " + Path.GetFileName(_photoPath ?? "");
            string part = "轴承";
            if (fault.Contains("液压") || fault.Contains("漏油")) part = "液压系统";
            else if (fault.Contains("电机") || fault.Contains("变频")) part = "电机";
            else if (fault.Contains("电气") || fault.Contains("接触器") || fault.Contains("过流")) part = "电气线路";
            else if (fault.Contains("皮带")) part = "皮带传动";
            else if (fault.Contains("减速")) part = "减速机";
            txtRepairPart.Text = part;
            lblAi.Text = "识别结果：" + part + "（演示规则识别）";
            UIMessageTip.ShowOk("识别完成：" + part);
        }

        private void BtnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureOrderDraft();
                int n = _matDal.CalculateMaterials(_orderNo, txtRepairPart.Text.Trim(), (int)nudQty.Value);
                dgv.DataSource = _matDal.GetByOrderNo(_orderNo);
                FormatMatGrid();
                UIMessageTip.ShowOk("算料完成，共 " + n + " 条");
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureOrderDraft(true);
                if (string.IsNullOrWhiteSpace(txtRepairPart.Text))
                {
                    UIMessageBox.ShowWarning("请先填写或识别维修部位");
                    return;
                }
                _matDal.CalculateMaterials(_orderNo, txtRepairPart.Text.Trim(), (int)nudQty.Value);
                dgv.DataSource = _matDal.GetByOrderNo(_orderNo);
                FormatMatGrid();
                UIMessageBox.ShowSuccess("已生成工单 " + _orderNo + " 及领料清单");
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void EnsureOrderDraft(bool forceUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(txtDeviceNo.Text))
                throw new Exception("请填写设备编号");
            if (string.IsNullOrEmpty(_orderNo))
            {
                WorkOrderEntity wo = new WorkOrderEntity
                {
                    OrderNo = _woDal.GenerateOrderNo(),
                    DeviceNo = txtDeviceNo.Text.Trim(),
                    DeviceName = txtDeviceName.Text.Trim(),
                    RepairPart = txtRepairPart.Text.Trim(),
                    FaultDesc = txtFault.Text.Trim(),
                    PhotoPath = _photoPath,
                    RepairQuantity = (int)nudQty.Value,
                    Status = "Pending",
                    Priority = cboPriority.Text,
                    Reporter = txtReporter.Text.Trim()
                };
                _woDal.Insert(wo);
                _orderNo = wo.OrderNo;
            }
            else if (forceUpdate)
            {
                WorkOrderEntity exist = _woDal.GetByOrderNo(_orderNo);
                if (exist != null)
                {
                    exist.DeviceNo = txtDeviceNo.Text.Trim();
                    exist.DeviceName = txtDeviceName.Text.Trim();
                    exist.RepairPart = txtRepairPart.Text.Trim();
                    exist.FaultDesc = txtFault.Text.Trim();
                    exist.PhotoPath = _photoPath;
                    exist.RepairQuantity = (int)nudQty.Value;
                    exist.Priority = cboPriority.Text;
                    exist.Reporter = txtReporter.Text.Trim();
                    _woDal.Update(exist);
                }
            }
        }

        private void FormatMatGrid()
        {
            string[] h = { "Id", "工单号", "配件Id", "编码", "配件名称", "规格", "单位", "应领", "库存", "缺料", "状态", "备注", "时间" };
            for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                dgv.Columns[i].HeaderText = h[i];
            if (dgv.Columns.Contains("PartId")) dgv.Columns["PartId"].Visible = false;
        }

        private void ResetForm()
        {
            txtDeviceNo.Text = txtDeviceName.Text = txtFault.Text = txtRepairPart.Text = "";
            txtReporter.Text = SessionContext.DisplayName;
            cboPriority.Text = "Normal";
            nudQty.Value = 1;
            pic.ImageLocation = null;
            _photoPath = null;
            _orderNo = null;
            dgv.DataSource = null;
            lblAi.Text = "识别结果：等待识别...";
        }
    }
}
