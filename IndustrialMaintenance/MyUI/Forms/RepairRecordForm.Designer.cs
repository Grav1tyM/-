namespace MyUI.Forms
{
    partial class RepairRecordForm
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
            this.btnDelete = new Sunny.UI.UIButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.Text = "维修历史";
            this.ClientSize = new System.Drawing.Size(960, 540);
            this.Name = "RepairRecordForm";

            this.txtKeyword.Location = new System.Drawing.Point(20, 45); this.txtKeyword.Size = new System.Drawing.Size(220, 29);
            this.txtKeyword.Watermark = "设备/部位/工程师";
            this.btnSearch.Text = "查询"; this.btnSearch.Location = new System.Drawing.Point(250, 45); this.btnSearch.Size = new System.Drawing.Size(80, 29);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnDelete.Text = "删除"; this.btnDelete.Location = new System.Drawing.Point(340, 45); this.btnDelete.Size = new System.Drawing.Size(80, 29);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.dgv.Location = new System.Drawing.Point(20, 90);
            this.dgv.Size = new System.Drawing.Size(920, 410);
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.RowHeadersVisible = false;

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.txtKeyword, this.btnSearch, this.btnDelete, this.dgv});
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private Sunny.UI.UIDataGridView dgv;
        private Sunny.UI.UITextBox txtKeyword;
        private Sunny.UI.UIButton btnSearch, btnDelete;
    }
}
