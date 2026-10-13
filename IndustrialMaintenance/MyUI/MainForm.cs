using System;
using System.Drawing;
using System.Windows.Forms;
using MyUI.Pages;
using Sunny.UI;

namespace MyUI
{
    /// <summary>
    /// 主界面：左侧导航 + 右侧业务页（对齐截图布局）
    /// </summary>
    public partial class MainForm : UIForm
    {
        private readonly UINavMenu _nav;
        private readonly Panel _content;
        private readonly UILabel _lblTitle;
        private UserControl _current;

        public MainForm()
        {
            InitializeComponent();
            Text = "工业维修智能化零件分配平台";
            WindowState = FormWindowState.Maximized;
            Style = UIStyle.Custom;
            BackColor = Color.FromArgb(30, 34, 40);
            TitleColor = Color.FromArgb(24, 28, 34);
            RectColor = Color.FromArgb(24, 28, 34);

            Panel aside = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(24, 28, 34)
            };
            _nav = new UINavMenu
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(24, 28, 34),
                FillColor = Color.FromArgb(24, 28, 34),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 11F),
                Style = UIStyle.Custom
            };
            _nav.NodeMouseClick += (s, e) =>
            {
                if (e.Node?.Tag != null)
                {
                    ShowPage(e.Node.Tag.ToString());
                    _lblTitle.Text = e.Node.Text;
                    _nav.SelectedNode = e.Node;
                }
            };
            aside.Controls.Add(_nav);

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.FromArgb(36, 40, 48)
            };
            _lblTitle = new UILabel
            {
                Text = "首页概览",
                Font = new Font("微软雅黑", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Style = UIStyle.Custom
            };
            header.Controls.Add(_lblTitle);

            _content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 44, 52),
                Padding = new Padding(12)
            };

            Controls.Add(_content);
            Controls.Add(header);
            Controls.Add(aside);

            BuildMenu();
            ShowPage("home");
        }

        private void BuildMenu()
        {
            _nav.Nodes.Clear();
            AddMenu("首页概览", "home");
            AddMenu("报修识别", "repair");
            AddMenu("工单管理", "workorder");
            AddMenu("配件管理", "part");
            AddMenu("BOM材料规则", "bom");
            AddMenu("领料清单", "material");
            AddMenu("用户管理", "user");
            AddMenu("统计图表", "chart");
            AddMenu("数据大屏", "screen");
            AddMenu("系统设置", "setting");
            if (_nav.Nodes.Count > 0)
            {
                _nav.SelectedNode = _nav.Nodes[0];
            }
        }

        private void AddMenu(string text, string key)
        {
            TreeNode node = _nav.Nodes.Add(text);
            node.Tag = key;
        }

        private void ShowPage(string key)
        {
            UserControl page;
            switch (key)
            {
                case "home": page = new PageHome(); break;
                case "repair": page = new PageRepair(); break;
                case "workorder": page = new PageWorkOrder(); break;
                case "part": page = new PagePart(); break;
                case "bom": page = new PageBom(); break;
                case "material": page = new PageMaterial(); break;
                case "user": page = new PageUser(); break;
                case "chart":
                    page = new PagePlaceholder("统计图表", "图表功能暂缓，后续可接工单/库存统计。");
                    break;
                case "screen": page = new PageDataScreen(); break;
                case "setting":
                    page = new PagePlaceholder("系统设置", "系统设置功能暂缓。连接串请改 MyUI\\App.config。");
                    break;
                default:
                    page = new PagePlaceholder(key, "页面未实现");
                    break;
            }

            _content.Controls.Clear();
            if (_current != null)
            {
                _current.Dispose();
                _current = null;
            }
            page.Dock = DockStyle.Fill;
            _content.Controls.Add(page);
            _current = page;
        }
    }
}
