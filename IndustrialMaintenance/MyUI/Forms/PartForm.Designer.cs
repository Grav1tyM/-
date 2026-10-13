namespace MyUI.Forms
{
    partial class PartForm
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
            this.txtKeyword = new Sunny.UI.UITextBox();
            this.btnSearch = new Sunny.UI.UIButton();
            this.txtId = new Sunny.UI.UITextBox();
            this.txtCode = new Sunny.UI.UITextBox();
            this.txtName = new Sunny.UI.UITextBox();
            this.txtSpec = new Sunny.UI.UITextBox();
            this.cboCategory = new Sunny.UI.UIComboBox();
            this.txtUnit = new Sunny.UI.UITextBox();
            this.nudStock = new Sunny.UI.UIDoubleUpDown();
            this.nudMin = new Sunny.UI.UIDoubleUpDown();
            this.nudPrice = new Sunny.UI.UIDoubleUpDown();
            this.txtRemark = new Sunny.UI.UITextBox();
            this.btnAdd = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.btnScan = new Sunny.UI.UIButton();
            this.lbl1 = new Sunny.UI.UILabel();
            this.lbl2 = new Sunny.UI.UILabel();
            this.lbl3 = new Sunny.UI.UILabel();
            this.lbl4 = new Sunny.UI.UILabel();
            this.lbl5 = new Sunny.UI.UILabel();
            this.lbl6 = new Sunny.UI.UILabel();
            this.lbl7 = new Sunny.UI.UILabel();
            this.lbl8 = new Sunny.UI.UILabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.Text = "配件库存管理";
            this.ClientSize = new System.Drawing.Size(960, 620);
            this.Name = "PartForm";

            this.txtKeyword.Location = new System.Drawing.Point(20, 45);
            this.txtKeyword.Size = new System.Drawing.Size(200, 29);
            this.txtKeyword.Watermark = "编码/名称/规格";
            this.btnSearch.Location = new System.Drawing.Point(230, 45);
            this.btnSearch.Size = new System.Drawing.Size(80, 29);
            this.btnSearch.Text = "查询";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnScan.Location = new System.Drawing.Point(320, 45);
            this.btnScan.Size = new System.Drawing.Size(100, 29);
            this.btnScan.Text = "按编码带出";
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);

            this.dgv.Location = new System.Drawing.Point(20, 85);
            this.dgv.Size = new System.Drawing.Size(920, 300);
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.RowHeadersVisible = false;
            this.dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellClick);

            int y = 400;
            this.txtId.Visible = false;
            this.lbl1.Text = "编码"; this.lbl1.Location = new System.Drawing.Point(20, y);
            this.txtCode.Location = new System.Drawing.Point(70, y); this.txtCode.Size = new System.Drawing.Size(120, 29);
            this.lbl2.Text = "名称"; this.lbl2.Location = new System.Drawing.Point(200, y);
            this.txtName.Location = new System.Drawing.Point(250, y); this.txtName.Size = new System.Drawing.Size(140, 29);
            this.lbl3.Text = "规格"; this.lbl3.Location = new System.Drawing.Point(400, y);
            this.txtSpec.Location = new System.Drawing.Point(450, y); this.txtSpec.Size = new System.Drawing.Size(120, 29);
            this.lbl4.Text = "分类"; this.lbl4.Location = new System.Drawing.Point(580, y);
            this.cboCategory.Location = new System.Drawing.Point(630, y); this.cboCategory.Size = new System.Drawing.Size(120, 29);
            this.cboCategory.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;

            y = 440;
            this.lbl5.Text = "单位"; this.lbl5.Location = new System.Drawing.Point(20, y);
            this.txtUnit.Location = new System.Drawing.Point(70, y); this.txtUnit.Size = new System.Drawing.Size(80, 29); this.txtUnit.Text = "个";
            this.lbl6.Text = "库存"; this.lbl6.Location = new System.Drawing.Point(160, y);
            this.nudStock.Location = new System.Drawing.Point(210, y); this.nudStock.Size = new System.Drawing.Size(100, 29); this.nudStock.Maximum = 999999;
            this.lbl7.Text = "最低"; this.lbl7.Location = new System.Drawing.Point(320, y);
            this.nudMin.Location = new System.Drawing.Point(370, y); this.nudMin.Size = new System.Drawing.Size(100, 29); this.nudMin.Maximum = 999999;
            this.lbl8.Text = "单价"; this.lbl8.Location = new System.Drawing.Point(480, y);
            this.nudPrice.Location = new System.Drawing.Point(530, y); this.nudPrice.Size = new System.Drawing.Size(100, 29); this.nudPrice.Maximum = 999999;
            this.txtRemark.Location = new System.Drawing.Point(640, y); this.txtRemark.Size = new System.Drawing.Size(180, 29); this.txtRemark.Watermark = "备注";

            this.btnAdd.Text = "新增"; this.btnAdd.Location = new System.Drawing.Point(20, 500); this.btnAdd.Size = new System.Drawing.Size(90, 35);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnUpdate.Text = "修改"; this.btnUpdate.Location = new System.Drawing.Point(120, 500); this.btnUpdate.Size = new System.Drawing.Size(90, 35);
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            this.btnDelete.Text = "删除"; this.btnDelete.Location = new System.Drawing.Point(220, 500); this.btnDelete.Size = new System.Drawing.Size(90, 35);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.txtKeyword, this.btnSearch, this.btnScan, this.dgv, this.txtId,
                this.lbl1, this.txtCode, this.lbl2, this.txtName, this.lbl3, this.txtSpec,
                this.lbl4, this.cboCategory, this.lbl5, this.txtUnit, this.lbl6, this.nudStock,
                this.lbl7, this.nudMin, this.lbl8, this.nudPrice, this.txtRemark,
                this.btnAdd, this.btnUpdate, this.btnDelete});
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private Sunny.UI.UIDataGridView dgv;
        private Sunny.UI.UITextBox txtKeyword, txtId, txtCode, txtName, txtSpec, txtUnit, txtRemark;
        private Sunny.UI.UIComboBox cboCategory;
        private Sunny.UI.UIDoubleUpDown nudStock, nudMin, nudPrice;
        private Sunny.UI.UIButton btnSearch, btnAdd, btnUpdate, btnDelete, btnScan;
        private Sunny.UI.UILabel lbl1, lbl2, lbl3, lbl4, lbl5, lbl6, lbl7, lbl8;
    }
}
