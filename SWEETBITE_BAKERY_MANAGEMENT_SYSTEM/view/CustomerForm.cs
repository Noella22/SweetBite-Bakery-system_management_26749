using System;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class CustomerForm : Form
    {
        private int selectedId = 0;
        public CustomerForm() { InitializeComponent(); }
        private void CustomerForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!SchemaController.HasTable("customers"))
                {
                    lblHint.Text = "Customers not enabled. Run the optional SQL upgrade when ready.";
                    btnSave.Enabled = btnUpdate.Enabled = btnDelete.Enabled = false;
                    btnSearch.Enabled = btnRefresh.Enabled = false;
                    return;
                }
                LoadCustomers();
                lblHint.Text = "Add, update, search and manage your customer records.";
            }
            catch (Exception ex) { lblHint.Text = "Database unavailable: " + ex.Message; }
        }
        private void LoadCustomers()
        {
            try
            {
                dgvCustomers.DataSource = CustomerController.GetCustomers(txtSearch.Text.Trim());
                dgvCustomers.ClearSelection();
            }
            catch (Exception ex) { MessageBox.Show("Cannot load customers: " + ex.Message); }
        }
        private bool CheckFields()
        {
            if (txtCustomerName.Text.Trim() == "" || txtPhone.Text.Trim() == "")
            { MessageBox.Show("Enter customer name and phone."); return false; }
            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!CheckFields()) return;
            try
            {
                CustomerController.Save(0, txtCustomerName.Text.Trim(), txtPhone.Text.Trim());
                MessageBox.Show("Customer saved successfully!"); ClearFields(); LoadCustomers();
            }
            catch (Exception ex) { MessageBox.Show("Save failed: " + ex.Message); }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a customer first."); return; }
            if (!CheckFields()) return;
            try
            {
                CustomerController.Save(selectedId, txtCustomerName.Text.Trim(), txtPhone.Text.Trim());
                MessageBox.Show("Customer updated successfully!"); ClearFields(); LoadCustomers();
            }
            catch (Exception ex) { MessageBox.Show("Update failed: " + ex.Message); }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a customer first."); return; }
            if (MessageBox.Show("Delete this customer?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                CustomerController.Delete(selectedId);
                MessageBox.Show("Customer deleted successfully!"); ClearFields(); LoadCustomers();
            }
            catch (Exception ex) { MessageBox.Show("Delete failed (customer may have orders): " + ex.Message); }
        }
        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];
            selectedId = Convert.ToInt32(row.Cells["customer_id"].Value);
            txtCustomerName.Text = Convert.ToString(row.Cells["customer_name"].Value);
            txtPhone.Text = Convert.ToString(row.Cells["phone"].Value);
            lblSelected.Text = "Selected customer ID: " + selectedId;
        }
        private void ClearFields()
        {
            selectedId = 0; txtCustomerName.Clear(); txtPhone.Clear();
            lblSelected.Text = "New customer"; dgvCustomers.ClearSelection();
        }
        private void btnClear_Click(object sender, EventArgs e) { ClearFields(); }
        private void btnSearch_Click(object sender, EventArgs e) { LoadCustomers(); }
        private void btnRefresh_Click(object sender, EventArgs e) { txtSearch.Clear(); LoadCustomers(); }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
