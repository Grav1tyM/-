namespace MyUI.Forms
{
    partial class WorkOrderForm
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
            this.txtOrderNo = new Sunny.UI.UITextBox();
            this.txtDeviceNo = new Sunny.UI.UITextBox();
            this.txtDeviceName = new Sunny.UI.UITextBox();
            this.txtRepairPart = new Sunny.UI.UITextBox();
            this.txtFault = new Sunny.UI.UITextBox();
            this.nudQty = new System.Windows.Forms.NumericUpDown();
            this.cboStatus = new Sunny.UI.UIComboBox();
            this.cboPriority = new Sunny.UI.UIComboBox();
            this.txtReporter = new Sunny.UI.UITextBox();
            this.txtAssignee = new Sunny.UI.UITextBox();
            this.txtRemark = new Sunny.UI.UITextBox();
            this.btnNewNo = new Sunny.UI.UIButton();
            this.btnAdd = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.btnCalc = new Sunny.UI.UIButton();
            this.btnComplete = new Sunny.UI.UIButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.Text = "维修工单管理";
            this.ClientSize = new System.Drawing.Size(1000, 680);
            this.Name = "WorkOrderForm";

            this.txtKeyword.Location = new System.Drawing.Point(20, 45); this.txtKeyword.Size = new System.Drawing.Size(200, 29);
            this.txtKeyword.Watermark = "工单号/设备/部位/状态";
            this.btnSearch.Text = "查询"; this.btnSearch.Location = new System.Drawing.Point(230, 45); this.btnSearch.Size = new System.Drawing.Size(80, 29);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            this.dgv.Location = new System.Drawing.Point(20, 85);
            this.dgv.Size = new System.Drawing.Size(960, 300);
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.RowHeadersVisible = false;
            this.dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellClick);

            this.txtId.Visible = false;
            int y = 400;
            this.txtOrderNo.Location = new System.Drawing.Point(20, y); this.txtOrderNo.Size = new System.Drawing.Size(140, 29); this.txtOrderNo.Watermark = "工单号";
            this.btnNewNo.Text = "生成"; this.btnNewNo.Location = new System.Drawing.Point(170, y); this.btnNewNo.Size = new System.Drawing.Size(60, 29);
            this.btnNewNo.Click += new System.EventHandler(this.btnNewNo_Click);
            this.txtDeviceNo.Location = new System.Drawing.Point(240, y); this.txtDeviceNo.Size = new System.Drawing.Size(120, 29); this.txtDeviceNo.Watermark = "设备编号";
            this.txtDeviceName.Location = new System.Drawing.Point(370, y); this.txtDeviceName.Size = new System.Drawing.Size(160, 29); this.txtDeviceName.Watermark = "设备名称";
            this.txtRepairPart.Location = new System.Drawing.Point(540, y); this.txtRepairPart.Size = new System.Drawing.Size(100, 29); this.txtRepairPart.Watermark = "维修部位";
            this.nudQty.Location = new System.Drawing.Point(650, y); this.nudQty.Size = new System.Drawing.Size(80, 29);
            this.nudQty.Minimum = 1; this.nudQty.Maximum = 9999; this.nudQty.Value = 1; this.nudQty.DecimalPlaces = 0;
            this.cboStatus.Location = new System.Drawing.Point(740, y); this.cboStatus.Size = new System.Drawing.Size(110, 29);
            this.cboPriority.Location = new System.Drawing.Point(860, y); this.cboPriority.Size = new System.Drawing.Size(100, 29);

            y = 440;
            this.txtFault.Location = new System.Drawing.Point(20, y); this.txtFault.Size = new System.Drawing.Size(400, 29); this.txtFault.Watermark = "故障描述";
            this.txtReporter.Location = new System.Drawing.Point(430, y); this.txtReporter.Size = new System.Drawing.Size(100, 29); this.txtReporter.Watermark = "报修人";
            this.txtAssignee.Location = new System.Drawing.Point(540, y); this.txtAssignee.Size = new System.Drawing.Size(100, 29); this.txtAssignee.Watermark = "处理人";
            this.txtRemark.Location = new System.Drawing.Point(650, y); this.txtRemark.Size = new System.Drawing.Size(310, 29); this.txtRemark.Watermark = "备注";

            this.btnAdd.Text = "新增报修"; this.btnAdd.Location = new System.Drawing.Point(20, 500); this.btnAdd.Size = new System.Drawing.Size(100, 35);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnUpdate.Text = "保存"; this.btnUpdate.Location = new System.Drawing.Point(130, 500); this.btnUpdate.Size = new System.Drawing.Size(90, 35);
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            this.btnDelete.Text = "删除"; this.btnDelete.Location = new System.Drawing.Point(230, 500); this.btnDelete.Size = new System.Drawing.Size(90, 35);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnCalc.Text = "智能算料"; this.btnCalc.Location = new System.Drawing.Point(350, 500); this.btnCalc.Size = new System.Drawing.Size(110, 35);
            this.btnCalc.FillColor = System.Drawing.Color.FromArgb(110, 190, 40);
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            this.btnComplete.Text = "完工归档"; this.btnComplete.Location = new System.Drawing.Point(470, 500); this.btnComplete.Size = new System.Drawing.Size(110, 35);
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.txtKeyword, this.btnSearch, this.dgv, this.txtId, this.txtOrderNo, this.btnNewNo,
                this.txtDeviceNo, this.txtDeviceName, this.txtRepairPart, this.nudQty, this.cboStatus, this.cboPriority,
                this.txtFault, this.txtReporter, this.txtAssignee, this.txtRemark,
                this.btnAdd, this.btnUpdate, this.btnDelete, this.btnCalc, this.btnComplete});
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private Sunny.UI.UIDataGridView dgv;
        private Sunny.UI.UITextBox txtKeyword, txtId, txtOrderNo, txtDeviceNo, txtDeviceName, txtRepairPart, txtFault, txtReporter, txtAssignee, txtRemark;
        private System.Windows.Forms.NumericUpDown nudQty;
        private Sunny.UI.UIComboBox cboStatus, cboPriority;
        private Sunny.UI.UIButton btnSearch, btnNewNo, btnAdd, btnUpdate, btnDelete, btnCalc, btnComplete;
    }
}
