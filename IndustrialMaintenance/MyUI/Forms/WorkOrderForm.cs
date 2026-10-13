using System;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Forms
{
    public partial class WorkOrderForm : UIForm
    {
        private readonly WorkOrderDAL _dal = new WorkOrderDAL();
        private readonly MaterialRequirementDAL _matDal = new MaterialRequirementDAL();
        private readonly RepairRecordDAL _repairDal = new RepairRecordDAL();

        public WorkOrderForm()
        {
            InitializeComponent();
            cboStatus.Items.AddRange(new object[] { "Pending", "Processing", "Completed", "Cancel" });
            cboPriority.Items.AddRange(new object[] { "Normal", "Urgent" });
            cboStatus.SelectedIndex = 0;
            cboPriority.SelectedIndex = 0;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = _dal.GetAll(txtKeyword.Text.Trim());
                FormatGrid();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void FormatGrid()
        {
            string[] h = {
                "Id","工单号","设备编号","设备名称","维修部位","故障描述","照片","识别JSON",
                "维修数量","状态","优先级","报修人","处理人","派工时间","创建时间","完工时间","备注"
            };
            for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                dgv.Columns[i].HeaderText = h[i];
            if (dgv.Columns.Contains("PhotoPath")) dgv.Columns["PhotoPath"].Visible = false;
            if (dgv.Columns.Contains("RecognitionJson")) dgv.Columns["RecognitionJson"].Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e) { LoadData(); }

        private void btnNewNo_Click(object sender, EventArgs e)
        {
            txtOrderNo.Text = _dal.GenerateOrderNo();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                WorkOrderEntity entity = ReadForm(0);
                if (string.IsNullOrWhiteSpace(entity.OrderNo))
                    entity.OrderNo = _dal.GenerateOrderNo();
                entity.Reporter = string.IsNullOrEmpty(entity.Reporter)
                    ? SessionContext.DisplayName : entity.Reporter;
                _dal.Insert(entity);
                UIMessageTip.ShowOk("报修成功：" + entity.OrderNo);
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0) return;
            try
            {
                _dal.Update(ReadForm(id));
                UIMessageTip.ShowOk("已保存");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0) return;
            if (!UIMessageBox.ShowAsk("确认删除工单？")) return;
            try { _dal.Delete(id); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            string orderNo = txtOrderNo.Text.Trim();
            string repairPart = txtRepairPart.Text.Trim();
            if (string.IsNullOrEmpty(orderNo) || string.IsNullOrEmpty(repairPart))
            {
                UIMessageBox.ShowWarning("请先选择工单并填写维修部位");
                return;
            }
            try
            {
                int n = _matDal.CalculateMaterials(orderNo, repairPart, Convert.ToInt32(nudQty.Value));
                // 算料后进入处理中
                if (int.TryParse(txtId.Text, out int id) && id > 0)
                {
                    WorkOrderEntity wo = ReadForm(id);
                    wo.Status = "Processing";
                    if (string.IsNullOrEmpty(wo.Assignee))
                    {
                        wo.Assignee = SessionContext.DisplayName;
                        wo.AssignTime = DateTime.Now;
                    }
                    _dal.Update(wo);
                }
                UIMessageBox.ShowSuccess("智能算料完成，生成 " + n + " 条领料记录");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0) return;
            try
            {
                WorkOrderEntity wo = ReadForm(id);
                _repairDal.CompleteWorkOrder(wo, SessionContext.DisplayName);
                UIMessageTip.ShowOk("工单已完工并写入维修历史");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private WorkOrderEntity ReadForm(int id)
        {
            return new WorkOrderEntity
            {
                Id = id,
                OrderNo = txtOrderNo.Text.Trim(),
                DeviceNo = txtDeviceNo.Text.Trim(),
                DeviceName = txtDeviceName.Text.Trim(),
                RepairPart = txtRepairPart.Text.Trim(),
                FaultDesc = txtFault.Text.Trim(),
                RepairQuantity = Convert.ToInt32(nudQty.Value),
                Status = cboStatus.Text,
                Priority = cboPriority.Text,
                Reporter = txtReporter.Text.Trim(),
                Assignee = txtAssignee.Text.Trim(),
                Remark = txtRemark.Text.Trim()
            };
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            txtId.Text = row.Cells["Id"].Value.ToString();
            txtOrderNo.Text = row.Cells["OrderNo"].Value.ToString();
            txtDeviceNo.Text = row.Cells["DeviceNo"].Value.ToString();
            txtDeviceName.Text = row.Cells["DeviceName"].Value?.ToString() ?? "";
            txtRepairPart.Text = row.Cells["RepairPart"].Value?.ToString() ?? "";
            txtFault.Text = row.Cells["FaultDesc"].Value?.ToString() ?? "";
            nudQty.Value = Convert.ToDecimal(row.Cells["RepairQuantity"].Value);
            cboStatus.Text = row.Cells["Status"].Value.ToString();
            cboPriority.Text = row.Cells["Priority"].Value.ToString();
            txtReporter.Text = row.Cells["Reporter"].Value?.ToString() ?? "";
            txtAssignee.Text = row.Cells["Assignee"].Value?.ToString() ?? "";
            txtRemark.Text = row.Cells["Remark"].Value?.ToString() ?? "";
        }
    }
}
