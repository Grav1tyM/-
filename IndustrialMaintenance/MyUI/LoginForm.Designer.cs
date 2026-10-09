namespace MyUI
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new Sunny.UI.UILabel();
            this.lblSubTitle = new Sunny.UI.UILabel();
            this.txtusername = new Sunny.UI.UITextBox();
            this.txtPassword = new Sunny.UI.UITextBox();
            this.btnLogin = new Sunny.UI.UISymbolButton();
            this.btnExit = new Sunny.UI.UISymbolButton();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(40, 70);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(420, 40);
            this.lblTitle.Style = Sunny.UI.UIStyle.Custom;
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "工业维修智能化零件分配平台";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblSubTitle
            //
            this.lblSubTitle.Font = new System.Drawing.Font("Consolas", 10F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.FromArgb(255, 180, 60);
            this.lblSubTitle.Location = new System.Drawing.Point(40, 115);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(420, 24);
            this.lblSubTitle.Style = Sunny.UI.UIStyle.Custom;
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "INDUSTRIAL MAINTENANCE PLATFORM";
            this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // txtusername
            //
            this.txtusername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtusername.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.txtusername.Location = new System.Drawing.Point(137, 170);
            this.txtusername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtusername.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtusername.Name = "txtusername";
            this.txtusername.Padding = new System.Windows.Forms.Padding(5);
            this.txtusername.Radius = 5;
            this.txtusername.RectColor = System.Drawing.Color.FromArgb(90, 97, 108);
            this.txtusername.ShowText = false;
            this.txtusername.Size = new System.Drawing.Size(225, 29);
            this.txtusername.Style = Sunny.UI.UIStyle.Custom;
            this.txtusername.TabIndex = 2;
            this.txtusername.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtusername.Watermark = "请输入用户名";
            this.txtusername.WatermarkColor = System.Drawing.Color.Gray;
            //
            // txtPassword
            //
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.txtPassword.Location = new System.Drawing.Point(137, 220);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Radius = 5;
            this.txtPassword.RectColor = System.Drawing.Color.FromArgb(90, 97, 108);
            this.txtPassword.ShowText = false;
            this.txtPassword.Size = new System.Drawing.Size(225, 29);
            this.txtPassword.Style = Sunny.UI.UIStyle.Custom;
            this.txtPassword.TabIndex = 3;
            this.txtPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPassword.Watermark = "请输入密码";
            this.txtPassword.WatermarkColor = System.Drawing.Color.Gray;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyDown);
            //
            // btnLogin
            //
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.FillColor = System.Drawing.Color.FromArgb(80, 160, 255);
            this.btnLogin.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.btnLogin.Location = new System.Drawing.Point(110, 280);
            this.btnLogin.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(120, 40);
            this.btnLogin.Style = Sunny.UI.UIStyle.Custom;
            this.btnLogin.Symbol = 61473;
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "登录";
            this.btnLogin.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            //
            // btnExit
            //
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FillColor = System.Drawing.Color.FromArgb(230, 80, 80);
            this.btnExit.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.btnExit.Location = new System.Drawing.Point(260, 280);
            this.btnExit.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(120, 40);
            this.btnExit.Style = Sunny.UI.UIStyle.Custom;
            this.btnExit.Symbol = 61453;
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "退出";
            this.btnExit.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            //
            // LoginForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(40, 44, 52);
            this.ClientSize = new System.Drawing.Size(500, 380);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtusername);
            this.Controls.Add(this.lblSubTitle);
            this.Controls.Add(this.lblTitle);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.RectColor = System.Drawing.Color.FromArgb(40, 44, 52);
            this.ShowIcon = false;
            this.ShowInTaskbar = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.Text = "工业维修智能化零件分配平台 - 登录";
            this.TitleColor = System.Drawing.Color.FromArgb(30, 34, 40);
            this.TitleFont = new System.Drawing.Font("微软雅黑", 10F);
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UILabel lblTitle;
        private Sunny.UI.UILabel lblSubTitle;
        private Sunny.UI.UITextBox txtusername;
        private Sunny.UI.UITextBox txtPassword;
        private Sunny.UI.UISymbolButton btnLogin;
        private Sunny.UI.UISymbolButton btnExit;
    }
}
