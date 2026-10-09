namespace MyUI.Forms
{
    partial class BomRuleForm
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
            this.txtFilter = new Sunny.UI.UITextBox();
            this.btnSearch = new Sunny.UI.UIButton();
            this.txtId = new Sunny.UI.UITextBox();
            this.txtRepairPart = new Sunny.UI.UITextBox();
            this.cboPart = new Sunny.UI.UIComboBox();
            this.nudQty = new Sunny.UI.UIDoubleUpDown();
            this.nudLoss = new Sunny.UI.UIDoubleUpDown();
            this.nudMin = new Sunny.UI.UIDoubleUpDown();
            this.txtUnit = new Sunny.UI.UITextBox();
            this.txtRemark = new Sunny.UI.UITextBox();
            this.btnAdd = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.lbl1 = new Sunny.UI.UILabel();
            this.lbl2 = new Sunny.UI.UILabel();
            this.lbl3 = new Sunny.UI.UILabel();
            this.lbl4 = new Sunny.UI.UILabel();
            this.lbl5 = new Sunny.UI.UILabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.Text = "BOM 算料规则";
            this.ClientSize = new System.Drawing.Size(960, 600);
            this.Name = "BomRuleForm";

            this.txtFilter.Location = new System.Drawing.Point(20, 45);
            this.txtFilter.Size = new System.Drawing.Size(160, 29);
            this.txtFilter.Watermark = "按维修部位筛选";
            this.btnSearch.Text = "查询"; this.btnSearch.Location = new System.Drawing.Point(190, 45); this.btnSearch.Size = new System.Drawing.Size(80, 29);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            this.dgv.Location = new System.Drawing.Point(20, 85);
            this.dgv.Size = new System.Drawing.Size(920, 320);
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.RowHeadersVisible = false;
            this.dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellClick);

            this.txtId.Visible = false;
            int y = 420;
            this.lbl1.Text = "维修部位"; this.lbl1.Location = new System.Drawing.Point(20, y);
            this.txtRepairPart.Location = new System.Drawing.Point(100, y); this.txtRepairPart.Size = new System.Drawing.Size(120, 29);
            this.lbl2.Text = "配件"; this.lbl2.Location = new System.Drawing.Point(240, y);
            this.cboPart.Location = new System.Drawing.Point(290, y); this.cboPart.Size = new System.Drawing.Size(220, 29);
            this.cboPart.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.lbl3.Text = "基准"; this.lbl3.Location = new System.Drawing.Point(530, y);
            this.nudQty.Location = new System.Drawing.Point(580, y); this.nudQty.Size = new System.Drawing.Size(90, 29); this.nudQty.Value = 1; this.nudQty.Maximum = 9999;
            this.lbl4.Text = "损耗"; this.lbl4.Location = new System.Drawing.Point(680, y);
            this.nudLoss.Location = new System.Drawing.Point(730, y); this.nudLoss.Size = new System.Drawing.Size(90, 29); this.nudLoss.DecimalPlaces = 4; this.nudLoss.Maximum = 1;
            this.lbl5.Text = "最小"; this.lbl5.Location = new System.Drawing.Point(830, y);
            this.nudMin.Location = new System.Drawing.Point(870, y); this.nudMin.Size = new System.Drawing.Size(70, 29); this.nudMin.Maximum = 9999;

            y = 460;
            this.txtUnit.Location = new System.Drawing.Point(20, y); this.txtUnit.Size = new System.Drawing.Size(80, 29); this.txtUnit.Watermark = "单位";
            this.txtRemark.Location = new System.Drawing.Point(110, y); this.txtRemark.Size = new System.Drawing.Size(300, 29); this.txtRemark.Watermark = "备注";

            this.btnAdd.Text = "新增"; this.btnAdd.Location = new System.Drawing.Point(20, 510); this.btnAdd.Size = new System.Drawing.Size(90, 35);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnUpdate.Text = "修改"; this.btnUpdate.Location = new System.Drawing.Point(120, 510); this.btnUpdate.Size = new System.Drawing.Size(90, 35);
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            this.btnDelete.Text = "删除"; this.btnDelete.Location = new System.Drawing.Point(220, 510); this.btnDelete.Size = new System.Drawing.Size(90, 35);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.txtFilter, this.btnSearch, this.dgv, this.txtId, this.lbl1, this.txtRepairPart,
                this.lbl2, this.cboPart, this.lbl3, this.nudQty, this.lbl4, this.nudLoss, this.lbl5, this.nudMin,
                this.txtUnit, this.txtRemark, this.btnAdd, this.btnUpdate, this.btnDelete});
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private Sunny.UI.UIDataGridView dgv;
        private Sunny.UI.UITextBox txtFilter, txtId, txtRepairPart, txtUnit, txtRemark;
        private Sunny.UI.UIComboBox cboPart;
        private Sunny.UI.UIDoubleUpDown nudQty, nudLoss, nudMin;
        private Sunny.UI.UIButton btnSearch, btnAdd, btnUpdate, btnDelete;
        private Sunny.UI.UILabel lbl1, lbl2, lbl3, lbl4, lbl5;
    }
}
