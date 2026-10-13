using System;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI
{
    public partial class LoginForm : UIForm
    {
        private readonly UserDAL _userDal = new UserDAL();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = (txtusername.Text ?? string.Empty).Trim();
            string password = txtPassword.Text ?? string.Empty;

            if (string.IsNullOrEmpty(userName))
            {
                UIMessageTip.ShowWarning("请输入用户名");
                txtusername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                UIMessageTip.ShowWarning("请输入密码");
                txtPassword.Focus();
                return;
            }

            try
            {
                UsersEntity user = _userDal.Login(userName, password);
                if (user == null)
                {
                    UIMessageBox.ShowError("用户名或密码错误，或账号已被禁用");
                    return;
                }

                SessionContext.CurrentUser = user;
                MainForm main = new MainForm();
                main.FormClosed += (s, args) =>
                {
                    SessionContext.CurrentUser = null;
                    txtPassword.Clear();
                    Show();
                };
                Hide();
                main.Show();
            }
            catch (Exception ex)
            {
                UIMessageBox.ShowError("登录失败：" + ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}
