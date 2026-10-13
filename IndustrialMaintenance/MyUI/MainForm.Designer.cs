namespace MyUI
{
    partial class MainForm
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Name = "MainForm";
            this.Text = "工业维修智能化零件分配平台";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 1280, 800);
            this.ResumeLayout(false);
        }
    }
}
