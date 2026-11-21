using System;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class DashboardForm : Form
    {
        private readonly string username;
        // Visual Studio Designer uses the parameterless constructor.
        public DashboardForm() : this("Designer preview") { }

        public DashboardForm(string username)
        {
            InitializeComponent();
            this.username = username;
        }
        private void DashboardForm_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            lblGreeting.Text = "Welcome, " + username;
            RefreshSummary();
        }
        private void RefreshSummary()
        {
            try
            {
                lblProductCount.Text = ProductController.Count().ToString();
                lblSalesCount.Text = SaleController.Count().ToString();
                lblRevenue.Text = SaleController.TotalRevenue().ToString("N0");
                lblStatus.Text = "Connected to sweetbite_db";
            }
            catch (Exception ex)
            {
                lblProductCount.Text = "--";
                lblSalesCount.Text = "--";
                lblRevenue.Text = "--";
                lblStatus.Text = "Database unavailable: " + ex.Message;
            }
        }
        private void btnProducts_Click(object sender, EventArgs e)
        { using (ProductForm form = new ProductForm()) form.ShowDialog(this); RefreshSummary(); }
        private void btnSales_Click(object sender, EventArgs e)
        { using (SalesForm form = new SalesForm()) form.ShowDialog(this); RefreshSummary(); }
        private void btnCustomers_Click(object sender, EventArgs e)
        { using (CustomerForm form = new CustomerForm()) form.ShowDialog(this); }
        private void btnOrders_Click(object sender, EventArgs e)
        { using (OrderForm form = new OrderForm()) form.ShowDialog(this); RefreshSummary(); }
        private void btnHistory_Click(object sender, EventArgs e)
        { using (OrderHistoryForm form = new OrderHistoryForm()) form.ShowDialog(this); }
        private void btnInventory_Click(object sender, EventArgs e)
        { using (InventoryForm form = new InventoryForm()) form.ShowDialog(this); }
        private void btnReceipts_Click(object sender, EventArgs e)
        { using (ReceiptForm form = new ReceiptForm()) form.ShowDialog(this); }
        private void btnReports_Click(object sender, EventArgs e)
        { using (ReportsForm form = new ReportsForm()) form.ShowDialog(this); }
        private void btnPayments_Click(object sender, EventArgs e)
        { using (PaymentForm form = new PaymentForm()) form.ShowDialog(this); }
        private void btnLogout_Click(object sender, EventArgs e) { Close(); }
    }
}
