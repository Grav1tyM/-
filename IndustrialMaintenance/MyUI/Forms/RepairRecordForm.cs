using System;
using MyDAL;
using Sunny.UI;

namespace MyUI.Forms
{
    public partial class RepairRecordForm : UIForm
    {
        private readonly RepairRecordDAL _dal = new RepairRecordDAL();

        public RepairRecordForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = _dal.GetAll(txtKeyword.Text.Trim());
                string[] h = { "Id","工单Id","设备编号","设备名称","维修部位","故障描述","配件摘要","工程师","维修时间","备注" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnSearch_Click(object sender, EventArgs e) { LoadData(); }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            if (!UIMessageBox.ShowAsk("确认删除该历史记录？")) return;
            try { _dal.Delete(id); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }
    }
}
