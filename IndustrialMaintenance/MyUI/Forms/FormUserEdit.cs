using System;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Forms
{
    public class FormUserEdit : UIForm
    {
        private readonly UserDAL _dal = new UserDAL();
        private readonly UsersEntity _entity;
        private readonly UITextBox txtUser = new UITextBox();
        private readonly UITextBox txtPwd = new UITextBox();
        private readonly UITextBox txtName = new UITextBox();
        private readonly UIComboBox cboRole = new UIComboBox();
        private readonly UITextBox txtPhone = new UITextBox();
        private readonly UIComboBox cboStatus = new UIComboBox();

        public FormUserEdit(UsersEntity entity = null)
        {
            _entity = entity ?? new UsersEntity { Status = 1, Role = "Engineer" };
            Text = entity == null ? "新增用户" : "编辑用户";
            ClientSize = new Size(420, 400);
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            int y = 50;
            AddBox("用户名", txtUser, ref y);
            AddBox("密码", txtPwd, ref y);
            txtPwd.Watermark = _entity.Id > 0 ? "留空则不修改" : "必填";
            AddBox("姓名", txtName, ref y);

            Label lr = new Label { Text = "角色", Location = new Point(30, y), AutoSize = true };
            cboRole.Location = new Point(110, y - 4);
            cboRole.Size = new Size(250, 29);
            cboRole.Items.AddRange(new object[] { "管理员", "维修工程师", "仓库管理员", "普通用户" });
            Controls.Add(lr);
            Controls.Add(cboRole);
            y += 40;

            AddBox("电话", txtPhone, ref y);

            Label ls = new Label { Text = "状态", Location = new Point(30, y), AutoSize = true };
            cboStatus.Location = new Point(110, y - 4);
            cboStatus.Size = new Size(250, 29);
            cboStatus.Items.AddRange(new object[] { "启用", "禁用" });
            Controls.Add(ls);
            Controls.Add(cboStatus);
            y += 50;

            UIButton ok = new UIButton { Text = "确定", Location = new Point(110, y), Size = new Size(100, 34) };
            UIButton cancel = new UIButton { Text = "取消", Location = new Point(230, y), Size = new Size(100, 34) };
            ok.Click += Ok_Click;
            cancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.Add(ok);
            Controls.Add(cancel);

            txtUser.Text = _entity.UserName ?? "";
            txtUser.ReadOnly = _entity.Id > 0;
            txtName.Text = _entity.RealName ?? "";
            txtPhone.Text = _entity.Phone ?? "";
            cboRole.Text = ToCn(_entity.Role);
            cboStatus.Text = _entity.Status == 0 ? "禁用" : "启用";
        }

        private void AddBox(string label, UITextBox box, ref int y)
        {
            Label l = new Label { Text = label, Location = new Point(30, y), AutoSize = true };
            box.Location = new Point(110, y - 4);
            box.Size = new Size(250, 29);
            Controls.Add(l);
            Controls.Add(box);
            y += 40;
        }

        private static string ToCn(string role)
        {
            if (role == "Admin") return "管理员";
            if (role == "Engineer") return "维修工程师";
            if (role == "Storekeeper") return "仓库管理员";
            if (role == "User") return "普通用户";
            return string.IsNullOrEmpty(role) ? "维修工程师" : role;
        }

        private static string ToEn(string role)
        {
            if (role == "管理员") return "Admin";
            if (role == "维修工程师") return "Engineer";
            if (role == "仓库管理员") return "Storekeeper";
            if (role == "普通用户") return "User";
            return role;
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            try
            {
                _entity.RealName = txtName.Text.Trim();
                _entity.Role = ToEn(cboRole.Text);
                _entity.Phone = txtPhone.Text.Trim();
                _entity.Status = cboStatus.Text == "禁用" ? 0 : 1;
                if (_entity.Id <= 0)
                {
                    if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPwd.Text))
                        throw new Exception("用户名和密码必填");
                    _entity.UserName = txtUser.Text.Trim();
                    _entity.PasswordHash = txtPwd.Text;
                    _dal.Insert(_entity);
                }
                else
                {
                    _entity.PasswordHash = txtPwd.Text;
                    _dal.Update(_entity);
                }
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                UIMessageBox.ShowError(ex.Message);
            }
        }
    }
}
