using System;
using System.Data;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class SalesForm : Form
    {
        private int selectedSaleId = 0;

        public SalesForm()
        {
            InitializeComponent();
        }

        private void SalesForm_Load(object sender, EventArgs e)
        {
            try
            {
                cmbProduct.DisplayMember = "product_name";
                cmbProduct.ValueMember = "product_id";
                cmbProduct.DataSource = ProductController.GetProducts("");
                btnSave.Enabled = cmbProduct.Items.Count > 0;
                LoadSales();
                UpdateTotal();
            }
            catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
        }

        private void LoadSales()
        {
            try
            {
                dgvSales.DataSource = SaleController.GetSales(txtSearch.Text.Trim());
                dgvSales.Columns["product_id"].Visible = false;
                dgvSales.Columns["sale_id"].HeaderText = "Sale ID";
                dgvSales.Columns["product_name"].HeaderText = "Product";
                dgvSales.Columns["quantity"].HeaderText = "Qty";
                dgvSales.Columns["unit_price"].HeaderText = "Price";
                dgvSales.Columns["total_amount"].HeaderText = "Total";
                dgvSales.Columns["sale_date"].HeaderText = "Date";
                dgvSales.Columns["unit_price"].DefaultCellStyle.Format = "N0";
                dgvSales.Columns["total_amount"].DefaultCellStyle.Format = "N0";
                dgvSales.ClearSelection();
            }
            catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
        }

        private void UpdateTotal()
        {
            DataRowView product = cmbProduct.SelectedItem as DataRowView;
            if (product == null)
            {
                txtUnitPrice.Text = "";
                txtTotal.Text = "";
                return;
            }
            decimal price = Convert.ToDecimal(product["unit_price"]);
            txtUnitPrice.Text = price.ToString("N0");
            txtTotal.Text = (price * numQuantity.Value).ToString("N0");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem == null) { MessageBox.Show("Add a product first."); return; }
            try
            {
                SaleController.Save(Convert.ToInt32(cmbProduct.SelectedValue), (int)numQuantity.Value);
                MessageBox.Show("Sale saved successfully!");
                ClearFields();
                LoadSales();
            }
            catch (Exception ex) { MessageBox.Show("Save failed: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedSaleId == 0) { MessageBox.Show("Select a sale first."); return; }
            if (cmbProduct.SelectedItem == null) { MessageBox.Show("Select a valid product."); return; }
            try
            {
                SaleController.Update(selectedSaleId, Convert.ToInt32(cmbProduct.SelectedValue), (int)numQuantity.Value);
                MessageBox.Show("Sale updated successfully!");
                ClearFields();
                LoadSales();
            }
            catch (Exception ex) { MessageBox.Show("Update failed: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedSaleId == 0) { MessageBox.Show("Select a sale first."); return; }
            if (MessageBox.Show("Delete this sale?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                SaleController.Delete(selectedSaleId);
                MessageBox.Show("Sale deleted successfully!");
                ClearFields();
                LoadSales();
            }
            catch (Exception ex) { MessageBox.Show("Delete failed: " + ex.Message); }
        }

        private void dgvSales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvSales.Rows[e.RowIndex];
            selectedSaleId = Convert.ToInt32(row.Cells["sale_id"].Value);
            if (row.Cells["product_id"].Value != DBNull.Value)
                cmbProduct.SelectedValue = Convert.ToInt32(row.Cells["product_id"].Value);
            numQuantity.Value = Convert.ToDecimal(row.Cells["quantity"].Value);
            lblSelected.Text = "Selected sale ID: " + selectedSaleId;
            UpdateTotal();
        }

        private void ClearFields()
        {
            selectedSaleId = 0;
            lblSelected.Text = "New sale";
            numQuantity.Value = 1;
            if (cmbProduct.Items.Count > 0) cmbProduct.SelectedIndex = 0;
            UpdateTotal();
            dgvSales.ClearSelection();
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e) { UpdateTotal(); }
        private void numQuantity_ValueChanged(object sender, EventArgs e) { UpdateTotal(); }
        private void btnClear_Click(object sender, EventArgs e) { ClearFields(); }
        private void btnSearch_Click(object sender, EventArgs e) { LoadSales(); }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadSales();
        }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
