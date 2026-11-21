using System;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class OrderHistoryForm : Form
    {
        private int selectedOrderId = 0;
        public OrderHistoryForm() { InitializeComponent(); }
        private void OrderHistoryForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!SchemaController.HasTable("orders"))
                { lblHint.Text = "Order history requires the optional SQL upgrade."; btnReceipt.Enabled = btnSearch.Enabled = btnRefresh.Enabled = false; return; }
                LoadOrders();
                lblHint.Text = "Select an order to view its receipt.";
            }
            catch (Exception ex) { lblHint.Text = "Database unavailable: " + ex.Message; btnReceipt.Enabled = false; }
        }
        private void LoadOrders()
        {
            try
            {
                dgvOrders.DataSource = OrderController.GetHistory(txtSearch.Text.Trim());
                dgvOrders.ClearSelection(); selectedOrderId = 0;
            }
            catch (Exception ex) { MessageBox.Show("Load failed: " + ex.Message); }
        }
        private void dgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            selectedOrderId = Convert.ToInt32(dgvOrders.Rows[e.RowIndex].Cells["order_id"].Value);
        }
        private void btnReceipt_Click(object sender, EventArgs e)
        {
            if (selectedOrderId == 0) { MessageBox.Show("Select an order first."); return; }
            using (ReceiptForm form = new ReceiptForm(selectedOrderId)) form.ShowDialog(this);
        }
        private void btnSearch_Click(object sender, EventArgs e) { LoadOrders(); }
        private void btnRefresh_Click(object sender, EventArgs e) { txtSearch.Clear(); LoadOrders(); }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
