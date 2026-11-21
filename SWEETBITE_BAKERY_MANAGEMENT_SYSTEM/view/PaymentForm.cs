using System;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class PaymentForm : Form
    {
        public PaymentForm() { InitializeComponent(); }
        private void PaymentForm_Load(object sender, EventArgs e)
        {
            cmbMethod.Items.AddRange(new object[] { "Cash", "Mobile Money", "Bank transfer", "Card (offline record)" });
            cmbMethod.SelectedIndex = 0;
            try
            {
                if (!SchemaController.HasTable("payments"))
                { lblHint.Text = "Payment records require the optional SQL upgrade. No online payments."; btnSave.Enabled = btnRefresh.Enabled = false; return; }
                LoadPayments();
                lblHint.Text = "Optional offline payments only. No online payment gateway.";
            }
            catch (Exception ex) { lblHint.Text = "Database unavailable: " + ex.Message; btnSave.Enabled = false; }
        }
        private void LoadPayments()
        {
            try { dgvPayments.DataSource = PaymentController.GetPayments(); dgvPayments.ClearSelection(); }
            catch (Exception ex) { MessageBox.Show("Load failed: " + ex.Message); }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            int id; decimal amount;
            if (!int.TryParse(txtOrderId.Text, out id) || id <= 0 ||
                !decimal.TryParse(txtAmount.Text, out amount) || amount <= 0)
            { MessageBox.Show("Enter a valid order number and positive amount."); return; }
            try
            {
                PaymentController.Save(id, Convert.ToString(cmbMethod.SelectedItem), amount);
                MessageBox.Show("Payment recorded successfully!");
                txtOrderId.Clear(); txtAmount.Clear(); LoadPayments();
            }
            catch (Exception ex) { MessageBox.Show("Payment failed: " + ex.Message); }
        }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadPayments(); }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
