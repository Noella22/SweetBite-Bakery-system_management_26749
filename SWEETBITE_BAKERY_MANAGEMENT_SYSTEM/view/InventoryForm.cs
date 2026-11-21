using System;
using System.Data;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class InventoryForm : Form
    {
        public InventoryForm() { InitializeComponent(); }
        private void InventoryForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!SchemaController.HasTable("inventory"))
                {
                    lblHint.Text = "Inventory requires optional SQL upgrade (stock_qty column).";
                    btnSave.Enabled = false; btnRefresh.Enabled = false;
                    return;
                }

                txtCategory.Text = "Not stored";
                txtCategory.Enabled = false;
                chkAvailable.Enabled = false;

                LoadStock();
                lblHint.Text = "Update product stock quantities.";
            }
            catch (Exception ex) { lblHint.Text = "Database unavailable: " + ex.Message; btnSave.Enabled = false; }
        }
        private void LoadStock()
        {
            try
            {
                DataTable stock = InventoryController.GetStock();
                dgvStock.DataSource = stock;
                cmbProduct.DisplayMember = "product_name";
                cmbProduct.ValueMember = "product_id";
                cmbProduct.DataSource = stock.Copy();
                if (dgvStock.Rows.Count > 0) dgvStock.ClearSelection();
            }
            catch (Exception ex) { MessageBox.Show("Inventory error: " + ex.Message); }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem == null) { MessageBox.Show("Select a product."); return; }
            if (txtCategory.Text.Trim() == "") { MessageBox.Show("Enter a category."); return; }
            try
            {
                InventoryController.Save(Convert.ToInt32(cmbProduct.SelectedValue),
                    txtCategory.Text.Trim(), (int)numStock.Value, chkAvailable.Checked);
                MessageBox.Show("Stock updated successfully!"); LoadStock();
            }
            catch (Exception ex) { MessageBox.Show("Save failed: " + ex.Message); }
        }
        private void dgvStock_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvStock.Rows[e.RowIndex];
            cmbProduct.SelectedValue = Convert.ToInt32(row.Cells["product_id"].Value);
            txtCategory.Text = Convert.ToString(row.Cells["category"].Value);
            numStock.Value = Convert.ToDecimal(row.Cells["stock_qty"].Value);
            chkAvailable.Checked = Convert.ToInt32(row.Cells["is_available"].Value) == 1;
        }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadStock(); }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
