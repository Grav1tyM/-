using System;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Forms
{
    public partial class CategoryForm : UIForm
    {
        private readonly PartCategoryDAL _dal = new PartCategoryDAL();

        public CategoryForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = _dal.GetAll();
                if (dgv.Columns.Count >= 3)
                {
                    dgv.Columns[0].HeaderText = "Id";
                    dgv.Columns[1].HeaderText = "分类名称";
                    dgv.Columns[2].HeaderText = "备注";
                }
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                _dal.Insert(new PartCategoryEntity { CategoryName = txtName.Text.Trim(), Remark = txtRemark.Text.Trim() });
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
                _dal.Update(new PartCategoryEntity { Id = id, CategoryName = txtName.Text.Trim(), Remark = txtRemark.Text.Trim() });
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

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            txtId.Text = row.Cells["Id"].Value.ToString();
            txtName.Text = row.Cells["CategoryName"].Value.ToString();
            txtRemark.Text = row.Cells["Remark"].Value?.ToString() ?? "";
        }
    }
}
