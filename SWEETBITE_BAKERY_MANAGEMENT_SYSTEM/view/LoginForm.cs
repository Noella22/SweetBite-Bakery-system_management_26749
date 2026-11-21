using System;
using System.Windows.Forms;
using SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.controller;

namespace SWEETBITE_BAKERY_MANAGEMENT_SYSTEM.view
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            btnSetup.Text = "CREATE ACCOUNT";
        }

        private void OpenDashboard(string name)
        {
            Hide();
            using (DashboardForm form = new DashboardForm(name)) form.ShowDialog();
            txtPassword.Clear();
            Show();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim() == "" || txtPassword.Text == "")
            { MessageBox.Show("Enter your username and password."); return; }
            try
            {
                if (AuthController.Login(txtUsername.Text.Trim(), txtPassword.Text))
                    OpenDashboard(txtUsername.Text.Trim());
                else MessageBox.Show("Incorrect login or users table not set up yet.");
            }
            catch (Exception ex) { MessageBox.Show("Login error: " + ex.Message); }
        }
        private void btnSetup_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SchemaController.HasTable("users"))
                {
                    MessageBox.Show("Please import sweetbite_db.sql into MySQL first.");
                    return;
                }

                using (AccountSetupForm form = new AccountSetupForm())
                {
                    form.ShowDialog(this);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Account error: " + ex.Message);
            }
        }
        private void btnPreview_Click(object sender, EventArgs e)
        {
           
            OpenDashboard("Preview mode (not signed in)");
        }
        private void btnExit_Click(object sender, EventArgs e) { Close(); }

        private void lblLoginHelp_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
