using System;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Forms
{
    public partial class UserForm : UIForm
    {
        private readonly UserDAL _dal = new UserDAL();

        public UserForm()
        {
            InitializeComponent();
            cboRole.Items.AddRange(new object[] { "Admin", "Engineer", "Storekeeper", "User" });
            cboRole.SelectedIndex = 1;
            cboStatus.Items.AddRange(new object[] { "1", "0" });
            cboStatus.SelectedIndex = 0;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = _dal.GetAll();
                string[] h = { "Id", "用户名", "姓名", "角色", "电话", "状态", "创建时间" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                _dal.Insert(new UsersEntity
                {
                    UserName = txtUserName.Text.Trim(),
                    PasswordHash = txtPassword.Text,
                    RealName = txtRealName.Text.Trim(),
                    Role = cboRole.Text,
                    Phone = txtPhone.Text.Trim(),
                    Status = Convert.ToInt32(cboStatus.Text)
                });
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
                _dal.Update(new UsersEntity
                {
                    Id = id,
                    PasswordHash = txtPassword.Text,
                    RealName = txtRealName.Text.Trim(),
                    Role = cboRole.Text,
                    Phone = txtPhone.Text.Trim(),
                    Status = Convert.ToInt32(cboStatus.Text)
                });
                UIMessageTip.ShowOk("修改成功（密码留空则不改）");
                LoadData();
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0) return;
            if (!UIMessageBox.ShowAsk("确认删除用户？")) return;
            try { _dal.Delete(id); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            txtId.Text = row.Cells["Id"].Value.ToString();
            txtUserName.Text = row.Cells["UserName"].Value.ToString();
            txtRealName.Text = row.Cells["RealName"].Value?.ToString() ?? "";
            cboRole.Text = row.Cells["Role"].Value.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";
            cboStatus.Text = row.Cells["Status"].Value.ToString();
            txtPassword.Text = "";
        }
    }
}
