using System;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using MyUI.Forms;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PageUser : UserControl
    {
        private readonly UserDAL _dal = new UserDAL();
        private readonly UITextBox txtKeyword = new UITextBox();
        private readonly UIDataGridView dgv = new UIDataGridView();

        public PageUser()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;
            if (!SessionContext.IsAdmin)
            {
                Controls.Add(new UILabel
                {
                    Text = "仅管理员可管理用户",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Orange,
                    Font = new Font("微软雅黑", 14F),
                    Style = UIStyle.Custom
                });
                return;
            }

            Panel bar = new Panel { Dock = DockStyle.Top, Height = 90 };
            txtKeyword.Location = new Point(8, 10);
            txtKeyword.Size = new Size(200, 29);
            txtKeyword.Watermark = "用户名/姓名";
            UIButton btnSearch = new UIButton { Text = "查询", Location = new Point(220, 10), Size = new Size(80, 29) };
            UIButton btnRefresh = new UIButton { Text = "刷新", Location = new Point(310, 10), Size = new Size(80, 29) };
            UIButton btnAdd = new UIButton { Text = "新增用户", Location = new Point(8, 50), Size = new Size(100, 32), FillColor = Color.FromArgb(110, 190, 40) };
            UIButton btnEdit = new UIButton { Text = "编辑用户", Location = new Point(118, 50), Size = new Size(100, 32) };
            UIButton btnReset = new UIButton { Text = "重置密码", Location = new Point(228, 50), Size = new Size(100, 32), FillColor = Color.FromArgb(80, 160, 255) };
            UIButton btnDel = new UIButton { Text = "删除用户", Location = new Point(338, 50), Size = new Size(100, 32), FillColor = Color.FromArgb(230, 80, 80) };
            btnSearch.Click += (s, e) => LoadData();
            btnRefresh.Click += (s, e) => { txtKeyword.Text = ""; LoadData(); };
            btnAdd.Click += (s, e) => Edit(null);
            btnEdit.Click += (s, e) => Edit(Selected());
            btnReset.Click += BtnReset_Click;
            btnDel.Click += BtnDel_Click;
            bar.Controls.AddRange(new Control[] { txtKeyword, btnSearch, btnRefresh, btnAdd, btnEdit, btnReset, btnDel });

            dgv.Dock = DockStyle.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) Edit(Selected()); };

            Controls.Add(dgv);
            Controls.Add(bar);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = _dal.GetAll(txtKeyword.Text.Trim());
                string[] h = { "Id", "用户名", "姓名", "角色", "电话", "状态", "创建时间" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    string role = Convert.ToString(row.Cells["Role"].Value);
                    if (role == "Admin") row.Cells["Role"].Value = "管理员";
                    else if (role == "Engineer") row.Cells["Role"].Value = "维修工程师";
                    else if (role == "Storekeeper") row.Cells["Role"].Value = "仓库管理员";
                    else if (role == "User") row.Cells["Role"].Value = "普通用户";
                    string st = Convert.ToString(row.Cells["Status"].Value);
                    if (st == "1") row.Cells["Status"].Value = "启用";
                    else if (st == "0") row.Cells["Status"].Value = "禁用";
                }
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private UsersEntity Selected()
        {
            if (dgv.CurrentRow == null) return null;
            string role = Convert.ToString(dgv.CurrentRow.Cells["Role"].Value);
            if (role == "管理员") role = "Admin";
            else if (role == "维修工程师") role = "Engineer";
            else if (role == "仓库管理员") role = "Storekeeper";
            else if (role == "普通用户") role = "User";
            string st = Convert.ToString(dgv.CurrentRow.Cells["Status"].Value);
            return new UsersEntity
            {
                Id = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value),
                UserName = Convert.ToString(dgv.CurrentRow.Cells["UserName"].Value),
                RealName = Convert.ToString(dgv.CurrentRow.Cells["RealName"].Value),
                Role = role,
                Phone = Convert.ToString(dgv.CurrentRow.Cells["Phone"].Value),
                Status = (st == "禁用" || st == "0") ? 0 : 1
            };
        }

        private void Edit(UsersEntity entity)
        {
            using (FormUserEdit f = new FormUserEdit(entity))
            {
                if (f.ShowDialog(FindForm()) == DialogResult.OK) LoadData();
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            UsersEntity u = Selected();
            if (u == null) { UIMessageTip.ShowWarning("请选择用户"); return; }
            using (FormPassword dlg = new FormPassword())
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
                _dal.ResetPassword(u.Id, dlg.Password);
                UIMessageTip.ShowOk("密码已重置");
            }
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            UsersEntity u = Selected();
            if (u == null) return;
            if (u.UserName == "admin") { UIMessageBox.ShowWarning("不能删除管理员账号"); return; }
            if (!UIMessageBox.ShowAsk("确认删除用户？")) return;
            _dal.Delete(u.Id);
            LoadData();
        }
    }

    public class FormPassword : UIForm
    {
        private readonly UITextBox txt = new UITextBox();
        public string Password { get { return txt.Text; } }

        public FormPassword()
        {
            Text = "重置密码";
            ClientSize = new Size(320, 160);
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            txt.Location = new Point(30, 50);
            txt.Size = new Size(250, 29);
            txt.Watermark = "新密码";
            UIButton ok = new UIButton { Text = "确定", Location = new Point(30, 100), Size = new Size(90, 32) };
            UIButton cancel = new UIButton { Text = "取消", Location = new Point(140, 100), Size = new Size(90, 32) };
            ok.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text)) { UIMessageTip.ShowWarning("请输入密码"); return; }
                DialogResult = DialogResult.OK;
            };
            cancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.AddRange(new Control[] { txt, ok, cancel });
        }
    }
}
