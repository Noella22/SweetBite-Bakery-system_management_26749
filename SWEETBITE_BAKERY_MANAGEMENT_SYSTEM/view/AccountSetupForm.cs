using System;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class AccountSetupForm : Form
    {
        public AccountSetupForm() { InitializeComponent(); }
        private void btnCreate_Click(object sender, EventArgs e)
        {
            string name = txtUsername.Text.Trim();
            if (name == "" || txtPassword.Text.Length < 8)
            { MessageBox.Show("Enter a username and a password of at least 8 characters."); return; }
            if (txtPassword.Text != txtConfirm.Text)
            { MessageBox.Show("Passwords do not match."); return; }

            try
            {
               AuthController.CreateAccount(name, txtPassword.Text);
                MessageBox.Show("Account created successfully. You can now log in.");
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Account setup failed: " + ex.Message); }
        }
        private void btnCancel_Click(object sender, EventArgs e) { Close(); }

        private void lblNote_Click(object sender, EventArgs e)
        {

        }
    }
}
