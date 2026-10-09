using System;
using System.Windows.Forms;
using MyUI.Forms;
using Sunny.UI;

namespace MyUI
{
    public partial class MainForm : UIForm
    {
        public MainForm()
        {
            InitializeComponent();
            Text = "工业维修智能化零件分配平台";
            lblWelcome.Text = "欢迎，" + SessionContext.DisplayName
                + "（" + RoleText(SessionContext.CurrentUser?.Role) + "）";
        }

        private static string RoleText(string role)
        {
            switch (role)
            {
                case "Admin": return "系统管理员";
                case "Engineer": return "维修工程师";
                case "Storekeeper": return "仓管员";
                default: return role ?? "用户";
            }
        }

        private void OpenChild(Form form)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }

        private void menuWorkOrder_Click(object sender, EventArgs e)
        {
            OpenChild(new WorkOrderForm());
        }

        private void menuMaterial_Click(object sender, EventArgs e)
        {
            OpenChild(new MaterialForm());
        }

        private void menuPart_Click(object sender, EventArgs e)
        {
            OpenChild(new PartForm());
        }

        private void menuCategory_Click(object sender, EventArgs e)
        {
            OpenChild(new CategoryForm());
        }

        private void menuBom_Click(object sender, EventArgs e)
        {
            OpenChild(new BomRuleForm());
        }

        private void menuRepair_Click(object sender, EventArgs e)
        {
            OpenChild(new RepairRecordForm());
        }

        private void menuUser_Click(object sender, EventArgs e)
        {
            if (!SessionContext.IsAdmin)
            {
                UIMessageBox.ShowWarning("仅管理员可管理用户");
                return;
            }
            OpenChild(new UserForm());
        }

        private void menuLogout_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
