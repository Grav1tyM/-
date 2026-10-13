using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;

namespace MyUI.Pages
{
    /// <summary>
    /// 数据大屏：统计卡片 + 配件消耗 Top5 + 工单状态分布（数据来自数据库）
    /// </summary>
    public class PageDataScreen : UserControl
    {
        public PageDataScreen()
        {
            BackColor = Color.FromArgb(16, 24, 40);
            Dock = DockStyle.Fill;
            Padding = new Padding(16);

            Label title = new Label
            {
                Text = "工业维修智能化零件分配平台 · 数据屏",
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 16F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };

            FlowLayoutPanel cards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(16, 24, 40)
            };

            WorkOrderDAL wo = new WorkOrderDAL();
            PartDAL part = new PartDAL();
            try
            {
                AddCard(cards, "工单总数", wo.CountAll().ToString());
                AddCard(cards, "待受理", wo.CountByStatus("Pending").ToString());
                AddCard(cards, "处理中", wo.CountByStatus("Processing").ToString());
                AddCard(cards, "已完成", wo.CountByStatus("Completed").ToString());
                AddCard(cards, "配件总数", part.CountAll().ToString());
            }
            catch (Exception ex)
            {
                AddCard(cards, "数据库", ex.Message);
            }

            Panel charts = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(16, 24, 40) };
            Panel left = new Panel { Dock = DockStyle.Left, Width = 520, BackColor = Color.White, Margin = new Padding(8) };
            Panel right = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            left.Paint += (s, e) => DrawBars(e.Graphics, left.ClientSize, true);
            right.Paint += (s, e) => DrawBars(e.Graphics, right.ClientSize, false);
            charts.Controls.Add(right);
            charts.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 16, BackColor = Color.FromArgb(16, 24, 40) });
            charts.Controls.Add(left);

            Controls.Add(charts);
            Controls.Add(cards);
            Controls.Add(title);
        }

        private static void AddCard(FlowLayoutPanel parent, string name, string value)
        {
            Panel p = new Panel { Width = 150, Height = 70, Margin = new Padding(6), BackColor = Color.FromArgb(28, 40, 64) };
            p.Controls.Add(new Label { Text = name, ForeColor = Color.Silver, Location = new Point(8, 8), AutoSize = true });
            p.Controls.Add(new Label
            {
                Text = value,
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 18F, FontStyle.Bold),
                Location = new Point(8, 30),
                AutoSize = true
            });
            parent.Controls.Add(p);
        }

        private void DrawBars(Graphics g, Size size, bool topParts)
        {
            g.Clear(Color.White);
            string caption = topParts ? "配件消耗 Top5" : "工单状态分布";
            g.DrawString(caption, new Font("微软雅黑", 11F, FontStyle.Bold), Brushes.Black, 12, 8);
            try
            {
                string[] labels;
                decimal[] values;
                if (topParts)
                {
                    DataTable dt = new MaterialRequirementDAL().GetTopConsumed(5);
                    labels = new string[dt.Rows.Count];
                    values = new decimal[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        labels[i] = dt.Rows[i]["PartName"] + " " + dt.Rows[i]["PartCode"];
                        values[i] = Convert.ToDecimal(dt.Rows[i]["Qty"]);
                    }
                }
                else
                {
                    WorkOrderDAL wo = new WorkOrderDAL();
                    labels = new[] { "待受理", "处理中", "已完成" };
                    values = new[]
                    {
                        (decimal)wo.CountByStatus("Pending"),
                        (decimal)wo.CountByStatus("Processing"),
                        (decimal)wo.CountByStatus("Completed")
                    };
                }
                if (labels.Length == 0)
                {
                    g.DrawString("暂无数据", new Font("微软雅黑", 10F), Brushes.Gray, 20, 50);
                    return;
                }
                decimal max = 1;
                foreach (decimal v in values) if (v > max) max = v;
                int barW = Math.Max(40, (size.Width - 40) / Math.Max(labels.Length, 1) - 16);
                for (int i = 0; i < labels.Length; i++)
                {
                    int h = (int)(values[i] / max * (size.Height - 90));
                    int x = 20 + i * (barW + 16);
                    int y = size.Height - 36 - h;
                    g.FillRectangle(Brushes.SteelBlue, x, y, barW, h);
                    g.DrawString(labels[i], new Font("微软雅黑", 8F), Brushes.Black, x, size.Height - 30);
                }
            }
            catch (Exception ex)
            {
                g.DrawString(ex.Message, new Font("微软雅黑", 9F), Brushes.Red, 12, 40);
            }
        }
    }
}
