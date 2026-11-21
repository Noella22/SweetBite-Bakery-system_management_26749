using System;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class ProductForm : Form
    {
        private int selectedProductId = 0;

        public ProductForm()
        {
            InitializeComponent();
        }

        private void ProductForm_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                dgvProducts.DataSource = ProductController.GetProducts(txtSearch.Text.Trim());
                dgvProducts.Columns["product_id"].HeaderText = "ID";
                dgvProducts.Columns["product_name"].HeaderText = "Product name";
                dgvProducts.Columns["unit_price"].HeaderText = "Price (RWF)";
                dgvProducts.Columns["unit_price"].DefaultCellStyle.Format = "N0";
                dgvProducts.ClearSelection();
            }
            catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
        }

        private bool ReadFields(out decimal price)
        {
            price = 0;
            if (txtProductName.Text.Trim() == "" ||
                !decimal.TryParse(txtUnitPrice.Text, out price) || price <= 0)
            {
                MessageBox.Show("Enter a product name and a valid positive price.");
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            decimal price;
            if (!ReadFields(out price)) return;
            try
            {
                ProductController.Save(txtProductName.Text.Trim(), price);
                MessageBox.Show("Product saved successfully!");
                ClearFields();
                LoadProducts();
            }
            catch (Exception ex) { MessageBox.Show("Save failed: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedProductId == 0) { MessageBox.Show("Select a product first."); return; }
            decimal price;
            if (!ReadFields(out price)) return;
            try
            {
                ProductController.Update(selectedProductId, txtProductName.Text.Trim(), price);
                MessageBox.Show("Product updated successfully!");
                ClearFields();
                LoadProducts();
            }
            catch (Exception ex) { MessageBox.Show("Update failed: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedProductId == 0) { MessageBox.Show("Select a product first."); return; }
            if (MessageBox.Show("Delete this product?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                ProductController.Delete(selectedProductId);
                MessageBox.Show("Product deleted successfully!");
                ClearFields();
                LoadProducts();
            }
            catch (Exception ex) { MessageBox.Show("Delete failed: " + ex.Message); }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvProducts.Rows[e.RowIndex];
            selectedProductId = Convert.ToInt32(row.Cells["product_id"].Value);
            txtProductName.Text = Convert.ToString(row.Cells["product_name"].Value);
            txtUnitPrice.Text = Convert.ToDecimal(row.Cells["unit_price"].Value).ToString("0.##");
            lblSelected.Text = "Selected product ID: " + selectedProductId;
        }

        private void ClearFields()
        {
            selectedProductId = 0;
            txtProductName.Clear();
            txtUnitPrice.Clear();
            lblSelected.Text = "New product";
            dgvProducts.ClearSelection();
        }

        private void btnClear_Click(object sender, EventArgs e) { ClearFields(); }
        private void btnSearch_Click(object sender, EventArgs e) { LoadProducts(); }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadProducts();
        }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
