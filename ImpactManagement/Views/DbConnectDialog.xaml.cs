using System.Windows;
using ImpactManagement.Models;

namespace ImpactManagement.Views
{
    public partial class DbConnectDialog : Window
    {
        public DbConnect DbConnect { get; private set; }
        public bool IsEditMode { get; private set; }

        public DbConnectDialog(DbConnect? dbConnect = null)
        {
            InitializeComponent();

            if (dbConnect != null)
            {
                // Edit mode
                IsEditMode = true;
                Title = "Edit DB Connect";
                DbConnect = dbConnect;
                
                TxtConnectName.Text = dbConnect.ConnectName;
                TxtIp.Text = dbConnect.Ip;
                TxtPort.Text = dbConnect.Port;
                TxtSchema.Text = dbConnect.Schema;
            }
            else
            {
                // Add mode
                IsEditMode = false;
                Title = "Add DB Connect";
                DbConnect = new DbConnect();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(TxtConnectName.Text))
            {
                MessageBox.Show("Connect Name is required", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update DbConnect object
            DbConnect.ConnectName = TxtConnectName.Text.Trim();
            DbConnect.Ip = TxtIp.Text.Trim();
            DbConnect.Port = TxtPort.Text.Trim();
            DbConnect.Schema = TxtSchema.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
