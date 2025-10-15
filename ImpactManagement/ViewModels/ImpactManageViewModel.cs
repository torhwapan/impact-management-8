using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ImpactManagement.Commands;
using ImpactManagement.Data;
using ImpactManagement.Models;
using ImpactManagement.Views;

namespace ImpactManagement.ViewModels
{
    public class ImpactManageViewModel : ViewModelBase
    {
        private readonly DatabaseSimulator _db;
        private ObservableCollection<AbnormalImpact> _impacts;
        private AbnormalImpact? _selectedImpact;

        public ObservableCollection<AbnormalImpact> Impacts
        {
            get => _impacts;
            set => SetProperty(ref _impacts, value);
        }

        public AbnormalImpact? SelectedImpact
        {
            get => _selectedImpact;
            set => SetProperty(ref _selectedImpact, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public ImpactManageViewModel()
        {
            _db = DatabaseSimulator.Instance;
            _impacts = new ObservableCollection<AbnormalImpact>();

            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, CanEdit);
            DeleteCommand = new RelayCommand(Delete, CanDelete);

            LoadData();
        }

        private void LoadData()
        {
            Impacts.Clear();
            foreach (var impact in _db.GetAllAbnormalImpacts())
            {
                Impacts.Add(impact);
            }
        }

        private void Add(object? parameter)
        {
            var replaceItems = new List<string>();
            var dialog = new ImpactDialog(null, replaceItems);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _db.AddAbnormalImpact(dialog.Impact);

                    // Save replace items
                    foreach (var item in dialog.ReplaceItems)
                    {
                        _db.AddImpactSqlReplace(new ImpactSqlReplace
                        {
                            ImpactId = dialog.Impact.ImpactName,
                            ReplaceItem = item
                        });
                    }

                    LoadData();
                    MessageBox.Show("Impact added successfully", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEdit(object? parameter)
        {
            return SelectedImpact != null;
        }

        private void Edit(object? parameter)
        {
            if (SelectedImpact == null)
            {
                MessageBox.Show("Please select an Impact to edit", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a copy for editing
            var editCopy = new AbnormalImpact
            {
                SceneName = SelectedImpact.SceneName,
                ImpactName = SelectedImpact.ImpactName,
                Sql = SelectedImpact.Sql,
                GroupField = SelectedImpact.GroupField,
                ConnectName = SelectedImpact.ConnectName
            };

            // Load existing replace items
            var replaceItems = _db.GetReplaceItemsByImpactId(SelectedImpact.ImpactName)
                .Select(r => r.ReplaceItem).ToList();

            var dialog = new ImpactDialog(editCopy, replaceItems);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _db.UpdateAbnormalImpact(SelectedImpact.ImpactName, dialog.Impact);

                    // Update replace items
                    _db.DeleteImpactSqlReplacesByImpactId(SelectedImpact.ImpactName);
                    foreach (var item in dialog.ReplaceItems)
                    {
                        _db.AddImpactSqlReplace(new ImpactSqlReplace
                        {
                            ImpactId = dialog.Impact.ImpactName,
                            ReplaceItem = item
                        });
                    }

                    LoadData();
                    SelectedImpact = null;
                    MessageBox.Show("Impact updated successfully", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanDelete(object? parameter)
        {
            return SelectedImpact != null;
        }

        private void Delete(object? parameter)
        {
            try
            {
                if (SelectedImpact == null)
                {
                    MessageBox.Show("Please select an Impact to delete", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete '{SelectedImpact.ImpactName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _db.DeleteAbnormalImpact(SelectedImpact.ImpactName);
                    _db.DeleteImpactSqlReplacesByImpactId(SelectedImpact.ImpactName);
                    LoadData();
                    SelectedImpact = null;
                    MessageBox.Show("Impact deleted successfully", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
