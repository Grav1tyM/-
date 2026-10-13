namespace MyUI.Forms
{
    partial class CategoryForm
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
            this.txtName = new Sunny.UI.UITextBox();
            this.txtRemark = new Sunny.UI.UITextBox();
            this.btnAdd = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.lbl1 = new Sunny.UI.UILabel();
            this.lbl2 = new Sunny.UI.UILabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.Text = "配件分类管理";
            this.ClientSize = new System.Drawing.Size(700, 480);
            this.Name = "CategoryForm";

            this.dgv.Location = new System.Drawing.Point(20, 45);
            this.dgv.Size = new System.Drawing.Size(660, 300);
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.RowHeadersVisible = false;
            this.dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellClick);

            this.txtId.Visible = false;
            this.lbl1.Text = "名称"; this.lbl1.Location = new System.Drawing.Point(20, 370);
            this.txtName.Location = new System.Drawing.Point(70, 370); this.txtName.Size = new System.Drawing.Size(160, 29);
            this.lbl2.Text = "备注"; this.lbl2.Location = new System.Drawing.Point(250, 370);
            this.txtRemark.Location = new System.Drawing.Point(300, 370); this.txtRemark.Size = new System.Drawing.Size(200, 29);

            this.btnAdd.Text = "新增"; this.btnAdd.Location = new System.Drawing.Point(20, 420); this.btnAdd.Size = new System.Drawing.Size(90, 35);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnUpdate.Text = "修改"; this.btnUpdate.Location = new System.Drawing.Point(120, 420); this.btnUpdate.Size = new System.Drawing.Size(90, 35);
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            this.btnDelete.Text = "删除"; this.btnDelete.Location = new System.Drawing.Point(220, 420); this.btnDelete.Size = new System.Drawing.Size(90, 35);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.dgv, this.txtId, this.lbl1, this.txtName, this.lbl2, this.txtRemark,
                this.btnAdd, this.btnUpdate, this.btnDelete});
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private Sunny.UI.UIDataGridView dgv;
        private Sunny.UI.UITextBox txtId, txtName, txtRemark;
        private Sunny.UI.UIButton btnAdd, btnUpdate, btnDelete;
        private Sunny.UI.UILabel lbl1, lbl2;
    }
}
