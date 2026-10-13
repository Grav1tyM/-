namespace MyUI.Forms
{
    partial class UserForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgv = new Sunny.UI.UIDataGridView();
            this.txtId = new Sunny.UI.UITextBox();
            this.txtUserName = new Sunny.UI.UITextBox();
            this.txtPassword = new Sunny.UI.UITextBox();
            this.txtRealName = new Sunny.UI.UITextBox();
            this.txtPhone = new Sunny.UI.UITextBox();
            this.cboRole = new Sunny.UI.UIComboBox();
            this.cboStatus = new Sunny.UI.UIComboBox();
            this.btnAdd = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.Text = "用户管理";
            this.ClientSize = new System.Drawing.Size(860, 540);
            this.Name = "UserForm";

            this.dgv.Location = new System.Drawing.Point(20, 45);
            this.dgv.Size = new System.Drawing.Size(820, 320);
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.RowHeadersVisible = false;
            this.dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellClick);

            this.txtId.Visible = false;
            int y = 385;
            this.txtUserName.Location = new System.Drawing.Point(20, y); this.txtUserName.Size = new System.Drawing.Size(120, 29); this.txtUserName.Watermark = "用户名";
            this.txtPassword.Location = new System.Drawing.Point(150, y); this.txtPassword.Size = new System.Drawing.Size(120, 29); this.txtPassword.Watermark = "密码";
            this.txtRealName.Location = new System.Drawing.Point(280, y); this.txtRealName.Size = new System.Drawing.Size(100, 29); this.txtRealName.Watermark = "姓名";
            this.cboRole.Location = new System.Drawing.Point(390, y); this.cboRole.Size = new System.Drawing.Size(120, 29);
            this.txtPhone.Location = new System.Drawing.Point(520, y); this.txtPhone.Size = new System.Drawing.Size(120, 29); this.txtPhone.Watermark = "电话";
            this.cboStatus.Location = new System.Drawing.Point(650, y); this.cboStatus.Size = new System.Drawing.Size(80, 29);

            this.btnAdd.Text = "新增"; this.btnAdd.Location = new System.Drawing.Point(20, 440); this.btnAdd.Size = new System.Drawing.Size(90, 35);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnUpdate.Text = "修改"; this.btnUpdate.Location = new System.Drawing.Point(120, 440); this.btnUpdate.Size = new System.Drawing.Size(90, 35);
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            this.btnDelete.Text = "删除"; this.btnDelete.Location = new System.Drawing.Point(220, 440); this.btnDelete.Size = new System.Drawing.Size(90, 35);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.dgv, this.txtId, this.txtUserName, this.txtPassword, this.txtRealName,
                this.cboRole, this.txtPhone, this.cboStatus, this.btnAdd, this.btnUpdate, this.btnDelete});
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private Sunny.UI.UIDataGridView dgv;
        private Sunny.UI.UITextBox txtId, txtUserName, txtPassword, txtRealName, txtPhone;
        private Sunny.UI.UIComboBox cboRole, cboStatus;
        private Sunny.UI.UIButton btnAdd, btnUpdate, btnDelete;
    }
}
