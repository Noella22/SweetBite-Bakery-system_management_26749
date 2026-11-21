using System;
using System.Data;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class OrderForm : Form
    {
        private readonly DataTable cart = new DataTable();
        public OrderForm()
        {
            InitializeComponent();
            cart.Columns.Add("product_id", typeof(int));
            cart.Columns.Add("product_name", typeof(string));
            cart.Columns.Add("quantity", typeof(int));
            cart.Columns.Add("unit_price", typeof(decimal));
            cart.Columns.Add("line_total", typeof(decimal));
            dgvItems.DataSource = cart;
            dgvItems.Columns["product_id"].Visible = false;
        }
        private void OrderForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!SchemaController.HasTable("orders") || !SchemaController.HasTable("customers") || !SchemaController.HasTable("inventory"))


                {
                    lblHint.Text = "Orders are not enabled. Run the optional SQL upgrade first.";
                    btnAdd.Enabled = btnSave.Enabled = false;
                    return;
                }
                LoadChoices();
                lblHint.Text = "Choose a customer, add products, then save the order.";
            }
            catch (Exception ex) { lblHint.Text = "Database unavailable: " + ex.Message; btnAdd.Enabled = btnSave.Enabled = false; }
        }
        private void LoadChoices()
        {
            cmbCustomer.DisplayMember = "customer_name";
            cmbCustomer.ValueMember = "customer_id";
            cmbCustomer.DataSource = CustomerController.GetCustomers("");
            cmbProduct.DisplayMember = "product_name";
            cmbProduct.ValueMember = "product_id";
            cmbProduct.DataSource = OrderController.GetAvailableProducts();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRowView product = cmbProduct.SelectedItem as DataRowView;
            if (product == null) { MessageBox.Show("Add stock to a product first."); return; }
            int id = Convert.ToInt32(product["product_id"]);
            int quantity = (int)numQuantity.Value;
            int stock = Convert.ToInt32(product["stock_qty"]);
            decimal price = Convert.ToDecimal(product["unit_price"]);
            DataRow existing = null;
            foreach (DataRow row in cart.Rows)
                if (Convert.ToInt32(row["product_id"]) == id) { existing = row; break; }
            int already = existing == null ? 0 : Convert.ToInt32(existing["quantity"]);
            if (quantity + already > stock) { MessageBox.Show("Not enough stock. Available: " + stock); return; }
            if (existing == null) cart.Rows.Add(id, product["product_name"], quantity, price, price * quantity);
            else
            {
                existing["quantity"] = quantity + already;
                existing["line_total"] = price * (quantity + already);
            }
            UpdateTotal();
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvItems.CurrentRow == null) return;
            DataRowView selected = dgvItems.CurrentRow.DataBoundItem as DataRowView;
            if (selected != null) { selected.Row.Delete(); UpdateTotal(); }
        }
        private void UpdateTotal()
        {
            decimal total = 0;
            foreach (DataRow row in cart.Rows)
                if (row.RowState != DataRowState.Deleted) total += Convert.ToDecimal(row["line_total"]);
            lblTotal.Text = "TOTAL: " + total.ToString("N0") + " RWF";
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedItem == null) { MessageBox.Show("Add a customer first."); return; }
            if (cart.Rows.Count == 0) { MessageBox.Show("Add products to the order first."); return; }
            try
            {
                int id = OrderController.Save(Convert.ToInt32(cmbCustomer.SelectedValue), cart);
                MessageBox.Show("Order #" + id + " saved successfully!");
                cart.Clear(); UpdateTotal(); LoadChoices();
                using (ReceiptForm form = new ReceiptForm(id)) form.ShowDialog(this);
            }
            catch (Exception ex) { MessageBox.Show("Order failed: " + ex.Message); }
        }
        private void btnBack_Click(object sender, EventArgs e) { Close(); }
    }
}
