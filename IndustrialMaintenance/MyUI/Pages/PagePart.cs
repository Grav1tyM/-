using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using MyUI.Forms;
using Sunny.UI;

namespace MyUI.Pages
{
    public class PagePart : UserControl
    {
        private readonly PartDAL _dal = new PartDAL();
        private readonly PartCategoryDAL _catDal = new PartCategoryDAL();
        private readonly UITextBox txtKeyword = new UITextBox();
        private readonly UIComboBox cboCat = new UIComboBox();
        private readonly UIDataGridView dgv = new UIDataGridView();

        public PagePart()
        {
            BackColor = Color.FromArgb(40, 44, 52);
            Dock = DockStyle.Fill;

            Panel bar = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(8) };
            txtKeyword.Location = new Point(8, 10); txtKeyword.Size = new Size(220, 29);
            txtKeyword.Watermark = "编码/名称/规格, 扫码检索";
            cboCat.Location = new Point(240, 10); cboCat.Size = new Size(140, 29);
            UIButton btnSearch = new UIButton { Text = "查询", Location = new Point(400, 10), Size = new Size(80, 29) };
            UIButton btnRefresh = new UIButton { Text = "刷新", Location = new Point(490, 10), Size = new Size(80, 29) };
            btnSearch.Click += (s, e) => LoadData();
            btnRefresh.Click += (s, e) => { txtKeyword.Text = ""; cboCat.SelectedIndex = 0; LoadData(); };

            UIButton btnAdd = new UIButton { Text = "新增配件", Location = new Point(8, 50), Size = new Size(100, 32) };
            UIButton btnEdit = new UIButton { Text = "编辑配件", Location = new Point(118, 50), Size = new Size(100, 32) };
            UIButton btnDel = new UIButton { Text = "删除配件", Location = new Point(228, 50), Size = new Size(100, 32), FillColor = Color.FromArgb(230, 80, 80) };
            UIButton btnIn = new UIButton { Text = "入库", Location = new Point(348, 50), Size = new Size(90, 32), FillColor = Color.FromArgb(110, 190, 40) };
            UIButton btnOut = new UIButton { Text = "出库", Location = new Point(448, 50), Size = new Size(90, 32), FillColor = Color.FromArgb(230, 150, 60) };
            btnAdd.Click += (s, e) => Edit(null);
            btnEdit.Click += (s, e) => Edit(Selected());
            btnDel.Click += BtnDel_Click;
            btnIn.Click += (s, e) => Adjust(true);
            btnOut.Click += (s, e) => Adjust(false);

            bar.Controls.AddRange(new Control[] {
                txtKeyword, cboCat, btnSearch, btnRefresh, btnAdd, btnEdit, btnDel, btnIn, btnOut
            });

            dgv.Dock = DockStyle.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgv.BackgroundColor = Color.White;
            dgv.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) Edit(Selected()); };

            Controls.Add(dgv);
            Controls.Add(bar);

            DataTable cats = _catDal.GetAll();
            DataRow all = cats.NewRow();
            all["Id"] = 0;
            all["CategoryName"] = "全部分类";
            cats.Rows.InsertAt(all, 0);
            cboCat.ValueMember = "Id";
            cboCat.DisplayMember = "CategoryName";
            cboCat.DataSource = cats;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = _dal.GetAll(txtKeyword.Text.Trim());
                if (cboCat.SelectedValue != null && Convert.ToInt32(cboCat.SelectedValue) > 0)
                {
                    DataView dv = dt.DefaultView;
                    dv.RowFilter = "CategoryId=" + Convert.ToInt32(cboCat.SelectedValue);
                    dgv.DataSource = dv.ToTable();
                }
                else dgv.DataSource = dt;

                string[] h = { "Id", "配件编码", "配件名称", "规格型号", "分类Id", "分类", "单位", "库存", "最低库存", "单价(元)", "库存状态", "备注", "创建时间" };
                for (int i = 0; i < h.Length && i < dgv.Columns.Count; i++)
                    dgv.Columns[i].HeaderText = h[i];
                if (dgv.Columns.Contains("CategoryId")) dgv.Columns["CategoryId"].Visible = false;
                // 状态中文显示
                if (dgv.Columns.Contains("Status"))
                {
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        string st = Convert.ToString(row.Cells["Status"].Value);
                        if (st == "InStock") row.Cells["Status"].Value = "库存充足";
                        else if (st == "LowStock") row.Cells["Status"].Value = "库存不足";
                        else if (st == "OutOfStock") row.Cells["Status"].Value = "缺货";
                    }
                }
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private PartEntity Selected()
        {
            if (dgv.CurrentRow == null) return null;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            return _dal.GetById(id);
        }

        private void Edit(PartEntity entity)
        {
            using (FormPartEdit f = new FormPartEdit(entity))
            {
                if (f.ShowDialog(FindForm()) == DialogResult.OK) LoadData();
            }
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            PartEntity p = Selected();
            if (p == null) return;
            if (!UIMessageBox.ShowAsk("确认删除配件？")) return;
            try { _dal.Delete(p.Id); LoadData(); }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }

        private void Adjust(bool inbound)
        {
            PartEntity p = Selected();
            if (p == null) { UIMessageTip.ShowWarning("请选择配件"); return; }
            using (FormStockQty dlg = new FormStockQty(inbound ? "入库数量" : "出库数量"))
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
                decimal qty = dlg.Quantity;
                if (qty <= 0)
                {
                    UIMessageBox.ShowWarning("请输入正确数量");
                    return;
                }
                try
                {
                    _dal.AdjustStock(p.Id, inbound ? qty : -qty);
                    UIMessageTip.ShowOk(inbound ? "入库成功" : "出库成功");
                    LoadData();
                }
                catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
            }
        }
    }
}
