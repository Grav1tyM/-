using System;
using System.Data;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Forms
{
    public partial class PartForm : UIForm
    {
        private readonly PartDAL _dal = new PartDAL();
        private readonly PartCategoryDAL _catDal = new PartCategoryDAL();

        public PartForm()
        {
            InitializeComponent();
            LoadCategories();
            LoadData();
        }

        private void LoadCategories()
        {
            DataTable dt = _catDal.GetAll();
            cboCategory.ValueMember = "Id";
            cboCategory.DisplayMember = "CategoryName";
            cboCategory.DataSource = dt;
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = _dal.GetAll(txtKeyword.Text.Trim());
                FormatGrid();
            }
            catch (Exception ex)
            {
                UIMessageBox.ShowError(ex.Message);
            }
        }

        private void FormatGrid()
        {
            if (dgv.Columns.Count == 0) return;
            string[] headers = { "Id", "配件编码", "配件名称", "规格", "分类Id", "分类", "单位", "库存", "最低库存", "单价", "状态", "备注", "创建时间" };
            for (int i = 0; i < headers.Length && i < dgv.Columns.Count; i++)
            {
                dgv.Columns[i].HeaderText = headers[i];
            }
            if (dgv.Columns.Contains("CategoryId")) dgv.Columns["CategoryId"].Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e) { LoadData(); }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                PartEntity entity = ReadForm(0);
                _dal.Insert(entity);
                UIMessageTip.ShowOk("新增成功");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0)
            {
                UIMessageBox.ShowWarning("请先选中一行");
                return;
            }
            try
            {
                PartEntity entity = ReadForm(id);
                _dal.Update(entity);
                UIMessageTip.ShowOk("修改成功");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0) return;
            if (!UIMessageBox.ShowAsk("确认删除该配件？")) return;
            try
            {
                _dal.Delete(id);
                UIMessageTip.ShowOk("已删除");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private PartEntity ReadForm(int id)
        {
            if (cboCategory.SelectedValue == null) throw new Exception("请选择分类");
            return new PartEntity
            {
                Id = id,
                PartCode = txtCode.Text.Trim(),
                PartName = txtName.Text.Trim(),
                Specification = txtSpec.Text.Trim(),
                CategoryId = Convert.ToInt32(cboCategory.SelectedValue),
                Unit = txtUnit.Text.Trim(),
                StockQuantity = Convert.ToDecimal(nudStock.Value),
                MinStock = Convert.ToDecimal(nudMin.Value),
                Price = Convert.ToDecimal(nudPrice.Value),
                Remark = txtRemark.Text.Trim()
            };
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgv.Rows[e.RowIndex];
            txtId.Text = row.Cells["Id"].Value.ToString();
            txtCode.Text = row.Cells["PartCode"].Value.ToString();
            txtName.Text = row.Cells["PartName"].Value.ToString();
            txtSpec.Text = row.Cells["Specification"].Value?.ToString() ?? "";
            cboCategory.SelectedValue = Convert.ToInt32(row.Cells["CategoryId"].Value);
            txtUnit.Text = row.Cells["Unit"].Value.ToString();
            nudStock.Value = Convert.ToDouble(row.Cells["StockQuantity"].Value);
            nudMin.Value = Convert.ToDouble(row.Cells["MinStock"].Value);
            nudPrice.Value = row.Cells["Price"].Value == DBNull.Value ? 0 : Convert.ToDouble(row.Cells["Price"].Value);
            txtRemark.Text = row.Cells["Remark"].Value?.ToString() ?? "";
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            if (string.IsNullOrEmpty(code)) return;
            PartEntity part = _dal.GetByCode(code);
            if (part == null)
            {
                UIMessageTip.ShowWarning("未找到配件编码：" + code);
                return;
            }
            txtId.Text = part.Id.ToString();
            txtName.Text = part.PartName;
            txtSpec.Text = part.Specification ?? "";
            cboCategory.SelectedValue = part.CategoryId;
            txtUnit.Text = part.Unit;
            nudStock.Value = Convert.ToDouble(part.StockQuantity);
            nudMin.Value = Convert.ToDouble(part.MinStock);
            nudPrice.Value = Convert.ToDouble(part.Price ?? 0);
            txtRemark.Text = part.Remark ?? "";
            UIMessageTip.ShowOk("已按编码带出配件");
        }
    }
}
