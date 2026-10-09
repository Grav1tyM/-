namespace MyUI.Forms
{
    partial class MaterialForm
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
            this.btnIssue = new Sunny.UI.UIButton();
            this.btnCancel = new Sunny.UI.UIButton();
            this.lblTip = new Sunny.UI.UILabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.Text = "领料分配";
            this.ClientSize = new System.Drawing.Size(1000, 560);
            this.Name = "MaterialForm";

            this.txtKeyword.Location = new System.Drawing.Point(20, 45); this.txtKeyword.Size = new System.Drawing.Size(220, 29);
            this.txtKeyword.Watermark = "工单号/配件/状态";
            this.btnSearch.Text = "查询"; this.btnSearch.Location = new System.Drawing.Point(250, 45); this.btnSearch.Size = new System.Drawing.Size(80, 29);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnIssue.Text = "领料出库"; this.btnIssue.Location = new System.Drawing.Point(350, 45); this.btnIssue.Size = new System.Drawing.Size(100, 29);
            this.btnIssue.FillColor = System.Drawing.Color.FromArgb(80, 160, 255);
            this.btnIssue.Click += new System.EventHandler(this.btnIssue_Click);
            this.btnCancel.Text = "取消待领"; this.btnCancel.Location = new System.Drawing.Point(460, 45); this.btnCancel.Size = new System.Drawing.Size(100, 29);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.lblTip.Text = "提示：工单模块中「智能算料」会按 BOM 规则生成待领记录；此处执行出库扣库存。";
            this.lblTip.Location = new System.Drawing.Point(580, 48);
            this.lblTip.Size = new System.Drawing.Size(400, 25);

            this.dgv.Location = new System.Drawing.Point(20, 90);
            this.dgv.Size = new System.Drawing.Size(960, 430);
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.RowHeadersVisible = false;

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.txtKeyword, this.btnSearch, this.btnIssue, this.btnCancel, this.lblTip, this.dgv});
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private Sunny.UI.UIDataGridView dgv;
        private Sunny.UI.UITextBox txtKeyword;
        private Sunny.UI.UIButton btnSearch, btnIssue, btnCancel;
        private Sunny.UI.UILabel lblTip;
    }
}
