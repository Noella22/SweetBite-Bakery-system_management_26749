using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class ReceiptForm : Form
    {
        public ReceiptForm() { InitializeComponent(); }
        public ReceiptForm(int orderId) : this()
        {
            txtOrderId.Text = orderId.ToString();
            LoadReceipt();
        }
        private void LoadReceipt()
        {
            int id;
            if (!int.TryParse(txtOrderId.Text, out id) || id <= 0)
            { MessageBox.Show("Enter a valid order number."); return; }
            try
            {
                if (!SchemaController.HasTable("orders"))
                { txtReceipt.Text = "Run the optional SQL upgrade to enable order receipts."; return; }
                DataRow found = null;
                foreach (DataRow row in OrderController.GetHistory("").Rows)
                    if (Convert.ToInt32(row["order_id"]) == id) { found = row; break; }
                if (found == null) { MessageBox.Show("Order not found."); return; }
                StringBuilder receipt = new StringBuilder();
                receipt.AppendLine("                  SWEETBITE BAKERY");
                receipt.AppendLine("                  CUSTOMER RECEIPT");
                receipt.AppendLine(new string('-', 66));
                receipt.AppendLine("Order #: " + id);
                receipt.AppendLine("Customer: " + found["customer_name"]);
                receipt.AppendLine("Date: " + Convert.ToDateTime(found["order_date"]).ToString("dd MMM yyyy HH:mm"));
                receipt.AppendLine(new string('-', 66));
                foreach (DataRow row in OrderController.GetItems(id).Rows)
                {
                    receipt.AppendLine(Convert.ToString(row["product_name"]));
                    receipt.AppendLine("  " + row["quantity"] + " x " +
                        Convert.ToDecimal(row["unit_price"]).ToString("N0") + " = " +
                        Convert.ToDecimal(row["line_total"]).ToString("N0") + " RWF");
                }
                receipt.AppendLine(new string('-', 66));
                receipt.AppendLine("TOTAL: " + Convert.ToDecimal(found["total_amount"]).ToString("N0") + " RWF");
                receipt.AppendLine();
                receipt.AppendLine("Thank you for choosing SweetBite!");
                txtReceipt.Font = new System.Drawing.Font("Consolas", 11F);
                txtReceipt.Text = receipt.ToString();
            }
            catch (Exception ex) { MessageBox.Show("Receipt error: " + ex.Message); }
        }
        private void btnLoad_Click(object sender, EventArgs e) { LoadReceipt(); }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtReceipt.Text.Trim() == "") { MessageBox.Show("Load a receipt first."); return; }
            Clipboard.SetText(txtReceipt.Text);
            MessageBox.Show("Receipt copied to clipboard.");
        }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
