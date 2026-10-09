using System;
using System.Windows.Forms;
using MyDAL;
using Sunny.UI;

namespace MyUI.Forms
{
    public partial class MaterialForm : UIForm
    {
        private readonly MaterialRequirementDAL _dal = new MaterialRequirementDAL();

        public MaterialForm()
        {
            InitializeComponent();
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
                "Id","工单号","配件Id","配件编码","配件名称","规格","单位",
                "应领数量","库存快照","缺料","状态","备注","生成时间"
            };
            for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                dgv.Columns[i].HeaderText = h[i];
            if (dgv.Columns.Contains("PartId")) dgv.Columns["PartId"].Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e) { LoadData(); }

        private int? SelectedId()
        {
            if (dgv.CurrentRow == null) return null;
            return Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
        }

        private void btnIssue_Click(object sender, EventArgs e)
        {
            int? id = SelectedId();
            if (id == null)
            {
                UIMessageBox.ShowWarning("请先选择领料记录");
                return;
            }
            if (!UIMessageBox.ShowAsk("确认对该记录执行领料出库？")) return;
            try
            {
                _dal.Issue(id.Value);
                UIMessageTip.ShowOk("领料成功，库存已扣减");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            int? id = SelectedId();
            if (id == null) return;
            try
            {
                int n = _dal.Cancel(id.Value);
                if (n == 0) UIMessageTip.ShowWarning("仅待领状态可取消");
                else UIMessageTip.ShowOk("已取消");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }
    }
}
