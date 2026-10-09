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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuBiz = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWorkOrder = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRepair = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSys = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUser = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.lblWelcome = new Sunny.UI.UILabel();
            this.lblHint = new Sunny.UI.UILabel();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuBiz, this.menuBase, this.menuSys});
            this.menuStrip.Location = new System.Drawing.Point(0, 35);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1000, 25);
            this.menuStrip.TabIndex = 0;
            //
            // menuBiz
            //
            this.menuBiz.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuWorkOrder, this.menuMaterial, this.menuRepair});
            this.menuBiz.Name = "menuBiz";
            this.menuBiz.Text = "业务管理";
            //
            // menuWorkOrder
            //
            this.menuWorkOrder.Name = "menuWorkOrder";
            this.menuWorkOrder.Text = "维修工单";
            this.menuWorkOrder.Click += new System.EventHandler(this.menuWorkOrder_Click);
            //
            // menuMaterial
            //
            this.menuMaterial.Name = "menuMaterial";
            this.menuMaterial.Text = "领料分配";
            this.menuMaterial.Click += new System.EventHandler(this.menuMaterial_Click);
            //
            // menuRepair
            //
            this.menuRepair.Name = "menuRepair";
            this.menuRepair.Text = "维修历史";
            this.menuRepair.Click += new System.EventHandler(this.menuRepair_Click);
            //
            // menuBase
            //
            this.menuBase.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuPart, this.menuCategory, this.menuBom});
            this.menuBase.Name = "menuBase";
            this.menuBase.Text = "基础数据";
            //
            // menuPart
            //
            this.menuPart.Name = "menuPart";
            this.menuPart.Text = "配件库存";
            this.menuPart.Click += new System.EventHandler(this.menuPart_Click);
            //
            // menuCategory
            //
            this.menuCategory.Name = "menuCategory";
            this.menuCategory.Text = "配件分类";
            this.menuCategory.Click += new System.EventHandler(this.menuCategory_Click);
            //
            // menuBom
            //
            this.menuBom.Name = "menuBom";
            this.menuBom.Text = "BOM算料规则";
            this.menuBom.Click += new System.EventHandler(this.menuBom_Click);
            //
            // menuSys
            //
            this.menuSys.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuUser, this.menuLogout, this.menuExit});
            this.menuSys.Name = "menuSys";
            this.menuSys.Text = "系统";
            //
            // menuUser
            //
            this.menuUser.Name = "menuUser";
            this.menuUser.Text = "用户管理";
            this.menuUser.Click += new System.EventHandler(this.menuUser_Click);
            //
            // menuLogout
            //
            this.menuLogout.Name = "menuLogout";
            this.menuLogout.Text = "注销登录";
            this.menuLogout.Click += new System.EventHandler(this.menuLogout_Click);
            //
            // menuExit
            //
            this.menuExit.Name = "menuExit";
            this.menuExit.Text = "退出系统";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            //
            // lblWelcome
            //
            this.lblWelcome.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Location = new System.Drawing.Point(40, 120);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(900, 40);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "欢迎";
            //
            // lblHint
            //
            this.lblHint.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(80, 160, 255);
            this.lblHint.Location = new System.Drawing.Point(40, 180);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(900, 80);
            this.lblHint.TabIndex = 2;
            this.lblHint.Text = "请通过顶部菜单进入：维修工单 → 智能算料 → 领料分配 → 完工归档。\r\n基础数据中可维护配件、分类与 BOM 规则。";
            //
            // MainForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1000, 620);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "工业维修智能化零件分配平台";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 1000, 620);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuBiz;
        private System.Windows.Forms.ToolStripMenuItem menuWorkOrder;
        private System.Windows.Forms.ToolStripMenuItem menuMaterial;
        private System.Windows.Forms.ToolStripMenuItem menuRepair;
        private System.Windows.Forms.ToolStripMenuItem menuBase;
        private System.Windows.Forms.ToolStripMenuItem menuPart;
        private System.Windows.Forms.ToolStripMenuItem menuCategory;
        private System.Windows.Forms.ToolStripMenuItem menuBom;
        private System.Windows.Forms.ToolStripMenuItem menuSys;
        private System.Windows.Forms.ToolStripMenuItem menuUser;
        private System.Windows.Forms.ToolStripMenuItem menuLogout;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private Sunny.UI.UILabel lblWelcome;
        private Sunny.UI.UILabel lblHint;
    }
}
