using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ImpactManagement.Commands;
using ImpactManagement.Data;
using ImpactManagement.Models;
using ImpactManagement.Views;

namespace ImpactManagement.ViewModels
{
    public class DbConnectManageViewModel : ViewModelBase
    {
        private readonly DatabaseSimulator _db;
        private ObservableCollection<DbConnect> _dbConnects;
        private DbConnect? _selectedDbConnect;

        public ObservableCollection<DbConnect> DbConnects
        {
            get => _dbConnects;
            set => SetProperty(ref _dbConnects, value);
        }

        public DbConnect? SelectedDbConnect
        {
            get => _selectedDbConnect;
            set => SetProperty(ref _selectedDbConnect, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public DbConnectManageViewModel()
        {
            _db = DatabaseSimulator.Instance;
            _dbConnects = new ObservableCollection<DbConnect>();
            
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, CanEdit);
            DeleteCommand = new RelayCommand(Delete, CanDelete);

            LoadData();
        }

        private void LoadData()
        {
            DbConnects.Clear();
            foreach (var dbConnect in _db.GetAllDbConnects())
            {
                DbConnects.Add(dbConnect);
            }
        }

        private void Add(object? parameter)
        {
            var dialog = new DbConnectDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _db.AddDbConnect(dialog.DbConnect);
                    LoadData();
                    MessageBox.Show("DB Connect added successfully", "Success", 
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
            return SelectedDbConnect != null;
        }

        private void Edit(object? parameter)
        {
            if (SelectedDbConnect == null)
            {
                MessageBox.Show("Please select a DB Connect to edit", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a copy for editing
            var editCopy = new DbConnect
            {
                ConnectName = SelectedDbConnect.ConnectName,
                Ip = SelectedDbConnect.Ip,
                Port = SelectedDbConnect.Port,
                Schema = SelectedDbConnect.Schema
            };

            var dialog = new DbConnectDialog(editCopy);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _db.UpdateDbConnect(SelectedDbConnect.ConnectName, dialog.DbConnect);
                    LoadData();
                    SelectedDbConnect = null;
                    MessageBox.Show("DB Connect updated successfully", "Success", 
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
            return SelectedDbConnect != null;
        }

        private void Delete(object? parameter)
        {
            try
            {
                if (SelectedDbConnect == null)
                {
                    MessageBox.Show("Please select a DB Connect to delete", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete '{SelectedDbConnect.ConnectName}'?", 
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _db.DeleteDbConnect(SelectedDbConnect.ConnectName);
                    LoadData();
                    SelectedDbConnect = null;
                    MessageBox.Show("DB Connect deleted successfully", "Success", 
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
