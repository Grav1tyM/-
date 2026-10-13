using System;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PageWorkOrder : UserControl
    {
        private readonly WorkOrderDAL _dal = new WorkOrderDAL();
        private readonly RepairRecordDAL _repairDal = new RepairRecordDAL();
        private readonly UIComboBox cboStatus = new UIComboBox();
        private readonly UITextBox txtKeyword = new UITextBox();
        private readonly DateTimePicker dtStart = new DateTimePicker();
        private readonly DateTimePicker dtEnd = new DateTimePicker();
        private readonly UIDataGridView dgv = new UIDataGridView();

        public PageWorkOrder()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;

            Panel bar = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(8) };
            Label l1 = new Label { Text = "状态", ForeColor = Color.White, Location = new Point(8, 14), AutoSize = true };
            cboStatus.Location = new Point(50, 10); cboStatus.Size = new Size(110, 29);
            cboStatus.Items.AddRange(new object[] { "全部", "Pending", "Processing", "Completed", "Cancel" });
            cboStatus.Text = "全部";
            Label l2 = new Label { Text = "关键字", ForeColor = Color.White, Location = new Point(180, 14), AutoSize = true };
            txtKeyword.Location = new Point(240, 10); txtKeyword.Size = new Size(200, 29);
            txtKeyword.Watermark = "工单号/设备编号/设备";
            Label l3 = new Label { Text = "起始", ForeColor = Color.White, Location = new Point(460, 14), AutoSize = true };
            dtStart.Location = new Point(500, 10); dtStart.Size = new Size(120, 29); dtStart.Format = DateTimePickerFormat.Short;
            Label l4 = new Label { Text = "结束", ForeColor = Color.White, Location = new Point(640, 14), AutoSize = true };
            dtEnd.Location = new Point(680, 10); dtEnd.Size = new Size(120, 29); dtEnd.Format = DateTimePickerFormat.Short;
            UIButton btnSearch = new UIButton { Text = "查询", Location = new Point(820, 10), Size = new Size(80, 29) };
            UIButton btnRefresh = new UIButton { Text = "刷新", Location = new Point(910, 10), Size = new Size(80, 29) };
            btnSearch.Click += (s, e) => LoadData();
            btnRefresh.Click += (s, e) => { cboStatus.Text = "全部"; txtKeyword.Text = ""; LoadData(); };

            UIButton btnAccept = new UIButton { Text = "受理/派工", Location = new Point(8, 50), Size = new Size(110, 32) };
            UIButton btnDone = new UIButton { Text = "完工确认", Location = new Point(130, 50), Size = new Size(110, 32), FillColor = Color.FromArgb(110, 190, 40) };
            UIButton btnCancel = new UIButton { Text = "取消工单", Location = new Point(250, 50), Size = new Size(110, 32), FillColor = Color.FromArgb(230, 150, 60) };
            UIButton btnDelete = new UIButton { Text = "删除工单", Location = new Point(370, 50), Size = new Size(110, 32), FillColor = Color.FromArgb(230, 80, 80) };
            btnAccept.Click += BtnAccept_Click;
            btnDone.Click += BtnDone_Click;
            btnCancel.Click += BtnCancel_Click;
            btnDelete.Click += BtnDelete_Click;

            bar.Controls.AddRange(new Control[] {
                l1, cboStatus, l2, txtKeyword, l3, dtStart, l4, dtEnd, btnSearch, btnRefresh,
                btnAccept, btnDone, btnCancel, btnDelete
            });

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
                string st = cboStatus.Text == "全部" ? null : cboStatus.Text;
                dgv.DataSource = _dal.GetFiltered(st, txtKeyword.Text, dtStart.Value.Date.AddYears(-5), dtEnd.Value.Date);
                string[] h = { "Id", "工单号", "设备编号", "设备名称", "维修部位", "状态", "优先级", "报修人", "处理人", "报修时间", "备注" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private WorkOrderEntity Selected()
        {
            if (dgv.CurrentRow == null) return null;
            string no = dgv.CurrentRow.Cells["OrderNo"].Value.ToString();
            return _dal.GetByOrderNo(no);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            WorkOrderEntity wo = Selected();
            if (wo == null) { UIMessageTip.ShowWarning("请选择工单"); return; }
            wo.Status = "Processing";
            wo.Assignee = SessionContext.DisplayName;
            wo.AssignTime = DateTime.Now;
            _dal.Update(wo);
            UIMessageTip.ShowOk("已受理派工");
            LoadData();
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            WorkOrderEntity wo = Selected();
            if (wo == null) return;
            try
            {
                _repairDal.CompleteWorkOrder(wo, SessionContext.DisplayName);
                UIMessageTip.ShowOk("完工确认成功");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            WorkOrderEntity wo = Selected();
            if (wo == null) return;
            wo.Status = "Cancel";
            _dal.Update(wo);
            LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            WorkOrderEntity wo = Selected();
            if (wo == null) return;
            if (!UIMessageBox.ShowAsk("确认删除工单？")) return;
            _dal.Delete(wo.Id);
            LoadData();
        }
    }
}
