using System;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PageHome : UserControl
    {
        private readonly WorkOrderDAL _woDal = new WorkOrderDAL();
        private readonly PartDAL _partDal = new PartDAL();
        private readonly UIDataGridView _dgv = new UIDataGridView();
        private readonly UILabel _lblWelcome = new UILabel();
        private readonly FlowLayoutPanel _cards = new FlowLayoutPanel();

        public PageHome()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;

            Panel top = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(8) };
            _lblWelcome.Font = new Font("微软雅黑", 12F);
            _lblWelcome.ForeColor = Color.White;
            _lblWelcome.Dock = DockStyle.Top;
            _lblWelcome.Height = 36;
            _lblWelcome.Style = UIStyle.Custom;
            _lblWelcome.Text = "欢迎使用工业维修智能化零件分配平台！ 当前用户：" + SessionContext.DisplayName
                + "    " + DateTime.Now.ToString("yyyy年M月d日 dddd");

            UIButton btnRefresh = new UIButton
            {
                Text = "刷新数据",
                Width = 110,
                Height = 32,
                Location = new Point(8, 48)
            };
            btnRefresh.Click += (s, e) => LoadData();
            top.Controls.Add(btnRefresh);
            top.Controls.Add(_lblWelcome);

            _cards.Dock = DockStyle.Top;
            _cards.Height = 100;
            _cards.Padding = new Padding(8);
            _cards.BackColor = Color.FromArgb(40, 44, 52);

            UIGroupBox box = new UIGroupBox
            {
                Text = "最近报修工单",
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(48, 52, 60),
                RectColor = Color.FromArgb(70, 74, 82),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 11F),
                Padding = new Padding(8, 28, 8, 8)
            };
            _dgv.Dock = DockStyle.Fill;
            _dgv.AllowUserToAddRows = false;
            _dgv.ReadOnly = true;
            _dgv.RowHeadersVisible = false;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgv.BackgroundColor = Color.White;
            box.Controls.Add(_dgv);

            Controls.Add(box);
            Controls.Add(_cards);
            Controls.Add(top);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _cards.Controls.Clear();
                AddCard("工单总数", _woDal.CountAll().ToString(), Color.FromArgb(80, 160, 255));
                AddCard("待受理", _woDal.CountByStatus("Pending").ToString(), Color.FromArgb(230, 150, 60));
                AddCard("处理中", _woDal.CountByStatus("Processing").ToString(), Color.FromArgb(80, 160, 255));
                AddCard("已完成", _woDal.CountByStatus("Completed").ToString(), Color.FromArgb(110, 190, 40));
                AddCard("配件总数", _partDal.CountAll().ToString(), Color.FromArgb(80, 160, 255));
                AddCard("低库存预警", _partDal.CountLowStock().ToString(), Color.FromArgb(230, 150, 60));

                _dgv.DataSource = _woDal.GetRecent(30);
                string[] h = { "Id", "工单号", "设备编号", "设备名称", "维修部位", "状态", "优先级", "报修人", "处理人", "报修时间", "备注" };
                for (int i = 0; i < h.Length && i < _dgv.Columns.Count; i++)
                    _dgv.Columns[i].HeaderText = h[i];
            }
            catch (Exception ex)
            {
                UIMessageBox.ShowError(ex.Message);
            }
        }

        private void AddCard(string title, string value, Color accent)
        {
            Panel p = new Panel
            {
                Width = 150,
                Height = 72,
                Margin = new Padding(6),
                BackColor = Color.FromArgb(48, 52, 60)
            };
            Label t = new Label
            {
                Text = title,
                ForeColor = Color.Silver,
                Font = new Font("微软雅黑", 9F),
                Location = new Point(10, 8),
                AutoSize = true
            };
            Label v = new Label
            {
                Text = value,
                ForeColor = accent,
                Font = new Font("微软雅黑", 20F, FontStyle.Bold),
                Location = new Point(10, 30),
                AutoSize = true
            };
            p.Controls.Add(t);
            p.Controls.Add(v);
            _cards.Controls.Add(p);
        }
    }
}
