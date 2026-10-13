using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MyDAL;
using MyEntity;
using Sunny.UI;

namespace MyUI.Forms
{
    /// <summary>
    /// 配件编辑弹窗（对齐截图「配件编辑」）
    /// </summary>
    public class FormPartEdit : UIForm
    {
        private readonly PartDAL _dal = new PartDAL();
        private readonly PartCategoryDAL _catDal = new PartCategoryDAL();
        private readonly PartEntity _entity;
        private readonly UITextBox txtCode = new UITextBox();
        private readonly UITextBox txtName = new UITextBox();
        private readonly UITextBox txtSpec = new UITextBox();
        private readonly UIComboBox cboCat = new UIComboBox();
        private readonly UITextBox txtUnit = new UITextBox();
        private readonly NumericUpDown nudStock = new NumericUpDown();
        private readonly NumericUpDown nudMin = new NumericUpDown();
        private readonly NumericUpDown nudPrice = new NumericUpDown();
        private readonly UITextBox txtRemark = new UITextBox();

        public FormPartEdit(PartEntity entity = null)
        {
            _entity = entity ?? new PartEntity { Unit = "个", StockQuantity = 0, MinStock = 0, Price = 0 };
            Text = entity == null ? "配件新增" : "配件编辑";
            ClientSize = new Size(460, 520);
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Style = UIStyle.Custom;

            int y = 50;
            AddRow("配件编码", txtCode, ref y, "扫码枪扫描的条码值");
            AddRow("配件名称", txtName, ref y, "如 深沟球轴承");
            AddRow("规格型号", txtSpec, ref y, "如 6204-2RS");
            Label lc = new Label { Text = "所属分类", Location = new Point(30, y), AutoSize = true };
            cboCat.Location = new Point(120, y - 4); cboCat.Size = new Size(280, 29);
            cboCat.DropDownStyle = UIDropDownStyle.DropDownList;
            Controls.Add(lc); Controls.Add(cboCat); y += 40;
            AddRow("单位", txtUnit, ref y, "个/套/件/桶");
            AddNum("库存数量", nudStock, ref y);
            AddNum("最低库存", nudMin, ref y);
            AddNum("参考单价", nudPrice, ref y);
            Label lr = new Label { Text = "备注", Location = new Point(30, y), AutoSize = true };
            txtRemark.Location = new Point(120, y - 4); txtRemark.Size = new Size(280, 60); txtRemark.Multiline = true;
            txtRemark.Watermark = "选填";
            Controls.Add(lr); Controls.Add(txtRemark); y += 80;

            UIButton ok = new UIButton { Text = "确定", Location = new Point(120, y), Size = new Size(100, 36) };
            UIButton cancel = new UIButton { Text = "取消", Location = new Point(240, y), Size = new Size(100, 36), FillColor = Color.FromArgb(90, 97, 108) };
            ok.Click += Ok_Click;
            cancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.Add(ok); Controls.Add(cancel);

            DataTable cats = _catDal.GetAll();
            cboCat.ValueMember = "Id";
            cboCat.DisplayMember = "CategoryName";
            cboCat.DataSource = cats;

            txtCode.Text = _entity.PartCode ?? "";
            txtName.Text = _entity.PartName ?? "";
            txtSpec.Text = _entity.Specification ?? "";
            if (_entity.CategoryId > 0) cboCat.SelectedValue = _entity.CategoryId;
            txtUnit.Text = _entity.Unit ?? "个";
            nudStock.Maximum = 999999; nudMin.Maximum = 999999; nudPrice.Maximum = 999999;
            nudStock.DecimalPlaces = 2; nudMin.DecimalPlaces = 2; nudPrice.DecimalPlaces = 2;
            nudStock.Value = _entity.StockQuantity;
            nudMin.Value = _entity.MinStock;
            nudPrice.Value = _entity.Price ?? 0;
            txtRemark.Text = _entity.Remark ?? "";
        }

        private void AddRow(string label, UITextBox box, ref int y, string tip)
        {
            Label l = new Label { Text = label, Location = new Point(30, y), AutoSize = true };
            box.Location = new Point(120, y - 4); box.Size = new Size(280, 29); box.Watermark = tip;
            Controls.Add(l); Controls.Add(box); y += 40;
        }

        private void AddNum(string label, NumericUpDown nud, ref int y)
        {
            Label l = new Label { Text = label, Location = new Point(30, y), AutoSize = true };
            nud.Location = new Point(120, y - 4); nud.Size = new Size(160, 29);
            Controls.Add(l); Controls.Add(nud); y += 40;
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboCat.SelectedValue == null) throw new Exception("请选择分类");
                _entity.PartCode = txtCode.Text.Trim();
                _entity.PartName = txtName.Text.Trim();
                _entity.Specification = txtSpec.Text.Trim();
                _entity.CategoryId = Convert.ToInt32(cboCat.SelectedValue);
                _entity.Unit = txtUnit.Text.Trim();
                _entity.StockQuantity = nudStock.Value;
                _entity.MinStock = nudMin.Value;
                _entity.Price = nudPrice.Value;
                _entity.Remark = txtRemark.Text.Trim();
                if (string.IsNullOrEmpty(_entity.PartCode) || string.IsNullOrEmpty(_entity.PartName))
                    throw new Exception("编码和名称必填");
                if (_entity.Id > 0) _dal.Update(_entity);
                else _dal.Insert(_entity);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { UIMessageBox.ShowError(ex.Message); }
        }
    }
}
