using System.Linq;
using System.Windows;
using ImpactManagement.Data;
using ImpactManagement.Models;

namespace ImpactManagement.Views
{
    public partial class ActionDialog : Window
    {
        public ImpactAction Action { get; private set; }
        public bool IsEditMode { get; private set; }

        public ActionDialog(ImpactAction? action = null)
        {
            InitializeComponent();

            // Load impact names
            var db = DatabaseSimulator.Instance;
            var impactNames = db.GetAllAbnormalImpacts().Select(i => i.ImpactName).ToList();
            foreach (var name in impactNames)
            {
                CmbImpactName.Items.Add(name);
            }

            if (action != null)
            {
                // Edit mode
                IsEditMode = true;
                Title = "Edit Action";
                Action = action;
                
                CmbImpactName.SelectedItem = action.ImpactName;
                TxtGroupFieldValue.Text = action.GroupFieldValue;
                TxtActionDesc.Text = action.ActionDesc;
                TxtOpOwner.Text = action.OpOwner;
                TxtActionOwner.Text = action.ActionOwner;
                TxtActionTool.Text = action.ActionTool;
            }
            else
            {
                // Add mode
                IsEditMode = false;
                Title = "Add Action";
                Action = new ImpactAction();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (CmbImpactName.SelectedItem == null)
            {
                MessageBox.Show("Impact Name is required", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update Action object
            Action.ImpactName = CmbImpactName.SelectedItem.ToString() ?? string.Empty;
            Action.GroupFieldValue = TxtGroupFieldValue.Text.Trim();
            Action.ActionDesc = TxtActionDesc.Text.Trim();
            Action.OpOwner = TxtOpOwner.Text.Trim();
            Action.ActionOwner = TxtActionOwner.Text.Trim();
            Action.ActionTool = TxtActionTool.Text.Trim();

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
