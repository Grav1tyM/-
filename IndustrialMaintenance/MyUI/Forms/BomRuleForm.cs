using System;
using System.Data;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Forms
{
    public partial class BomRuleForm : UIForm
    {
        private readonly BomRuleDAL _dal = new BomRuleDAL();
        private readonly PartDAL _partDal = new PartDAL();

        public BomRuleForm()
        {
            InitializeComponent();
            LoadParts();
            LoadData();
        }

        private void LoadParts()
        {
            DataTable dt = _partDal.GetAll();
            cboPart.ValueMember = "Id";
            cboPart.DisplayMember = "PartName";
            // Display: PartCode - PartName
            dt.Columns.Add("Display", typeof(string));
            foreach (DataRow r in dt.Rows)
            {
                r["Display"] = r["PartCode"] + " - " + r["PartName"];
            }
            cboPart.DisplayMember = "Display";
            cboPart.DataSource = dt;
        }

        private void LoadData()
        {
            try
            {
                string rp = string.IsNullOrWhiteSpace(txtFilter.Text) ? null : txtFilter.Text.Trim();
                dgv.DataSource = _dal.GetAll(rp);
                FormatGrid();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void FormatGrid()
        {
            string[] h = { "Id", "维修部位", "配件Id", "配件编码", "配件名称", "基准用量", "损耗率", "最小领用", "单位", "备注" };
            for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                dgv.Columns[i].HeaderText = h[i];
            if (dgv.Columns.Contains("PartId")) dgv.Columns["PartId"].Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e) { LoadData(); }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                _dal.Insert(ReadForm(0));
                UIMessageTip.ShowOk("新增成功");
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
                UIMessageTip.ShowOk("修改成功");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0) return;
            if (!UIMessageBox.ShowAsk("确认删除？")) return;
            try { _dal.Delete(id); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private BomRuleEntity ReadForm(int id)
        {
            if (cboPart.SelectedValue == null) throw new Exception("请选择配件");
            return new BomRuleEntity
            {
                Id = id,
                RepairPart = txtRepairPart.Text.Trim(),
                PartId = Convert.ToInt32(cboPart.SelectedValue),
                QuantityPerUnit = Convert.ToDecimal(nudQty.Value),
                LossRate = Convert.ToDecimal(nudLoss.Value),
                MinQuantity = Convert.ToDecimal(nudMin.Value),
                Unit = txtUnit.Text.Trim(),
                Remark = txtRemark.Text.Trim()
            };
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            txtId.Text = row.Cells["Id"].Value.ToString();
            txtRepairPart.Text = row.Cells["RepairPart"].Value.ToString();
            cboPart.SelectedValue = Convert.ToInt32(row.Cells["PartId"].Value);
            nudQty.Value = Convert.ToDouble(row.Cells["QuantityPerUnit"].Value);
            nudLoss.Value = Convert.ToDouble(row.Cells["LossRate"].Value);
            nudMin.Value = Convert.ToDouble(row.Cells["MinQuantity"].Value);
            txtUnit.Text = row.Cells["Unit"].Value?.ToString() ?? "";
            txtRemark.Text = row.Cells["Remark"].Value?.ToString() ?? "";
        }
    }
}
