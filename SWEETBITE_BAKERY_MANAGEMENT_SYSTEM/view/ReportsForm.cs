using System;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class ReportsForm : Form
    {
        public ReportsForm() { InitializeComponent(); }
        private void ReportsForm_Load(object sender, EventArgs e) { LoadReports(); }
        private void LoadReports()
        {
            try
            {
                lblProducts.Text = ProductController.Count().ToString();
                lblSales.Text = SaleController.Count().ToString();
                lblRevenue.Text = SaleController.TotalRevenue().ToString("N0");
                dgvReports.DataSource = ReportController.SalesByProduct();
                dgvReports.ClearSelection();
            }
            catch (Exception ex) { lblHint.Text = "Report error: " + ex.Message; }
        }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadReports(); }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
