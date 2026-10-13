using System;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PageUser : UserControl
    {
        private readonly UserDAL _dal = new UserDAL();
        private readonly UIDataGridView dgv = new UIDataGridView();

        public PageUser()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;
            if (!SessionContext.IsAdmin)
            {
                Controls.Add(new UILabel
                {
                    Text = "仅管理员可管理用户",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Orange,
                    Font = new Font("微软雅黑", 14F),
                    Style = UIStyle.Custom
                });
                return;
            }
            Panel bar = new Panel { Dock = DockStyle.Top, Height = 48 };
            UIButton btn = new UIButton { Text = "刷新", Location = new Point(8, 10), Size = new Size(80, 29) };
            btn.Click += (s, e) => LoadData();
            bar.Controls.Add(btn);
            dgv.Dock = DockStyle.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            Controls.Add(dgv);
            Controls.Add(bar);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = _dal.GetAll();
                string[] h = { "Id", "用户名", "姓名", "角色", "电话", "状态", "创建时间" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }
    }
}
