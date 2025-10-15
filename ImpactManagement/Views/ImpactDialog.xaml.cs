using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ImpactManagement.Data;
using ImpactManagement.Models;

namespace ImpactManagement.Views
{
    public partial class ImpactDialog : Window
    {
        public AbnormalImpact Impact { get; private set; }
        public List<string> ReplaceItems { get; private set; }
        public bool IsEditMode { get; private set; }
        private string? _originalImpactName;

        public ImpactDialog(AbnormalImpact? impact = null, List<string>? replaceItems = null)
        {
            InitializeComponent();

            ReplaceItems = new List<string>();

            // Load connect names
            var db = DatabaseSimulator.Instance;
            var connectNames = db.GetAllDbConnects().Select(d => d.ConnectName).ToList();
            foreach (var name in connectNames)
            {
                CmbConnectName.Items.Add(name);
            }

            if (impact != null)
            {
                // Edit mode
                IsEditMode = true;
                Title = "Edit Impact";
                Impact = impact;
                _originalImpactName = impact.ImpactName;
                
                TxtSceneName.Text = impact.SceneName;
                TxtImpactName.Text = impact.ImpactName;
                CmbConnectName.SelectedItem = impact.ConnectName;
                TxtSql.Text = impact.Sql;
                TxtGroupField.Text = impact.GroupField;

                // Load replace items
                if (replaceItems != null)
                {
                    foreach (var item in replaceItems)
                    {
                        ReplaceItems.Add(item);
                        LstReplaceItems.Items.Add(item);
                    }
                }
            }
            else
            {
                // Add mode
                IsEditMode = false;
                Title = "Add Impact";
                Impact = new AbnormalImpact();
            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNewReplaceItem.Text))
            {
                var item = TxtNewReplaceItem.Text.Trim();
                if (!ReplaceItems.Contains(item))
                {
                    ReplaceItems.Add(item);
                    LstReplaceItems.Items.Add(item);
                    TxtNewReplaceItem.Clear();
                }
            }
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is string item)
            {
                ReplaceItems.Remove(item);
                LstReplaceItems.Items.Remove(item);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(TxtImpactName.Text))
            {
                MessageBox.Show("Impact Name is required", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbConnectName.SelectedItem == null)
            {
                MessageBox.Show("Connect Name is required", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update Impact object
            Impact.SceneName = TxtSceneName.Text.Trim();
            Impact.ImpactName = TxtImpactName.Text.Trim();
            Impact.ConnectName = CmbConnectName.SelectedItem.ToString() ?? string.Empty;
            Impact.Sql = TxtSql.Text.Trim();
            Impact.GroupField = TxtGroupField.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public string? OriginalImpactName => _originalImpactName;
    }
}
