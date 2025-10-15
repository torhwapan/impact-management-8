using System;
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
    public class ActionManageViewModel : ViewModelBase
    {
        private readonly DatabaseSimulator _db;
        private ObservableCollection<ImpactAction> _actions;
        private ImpactAction? _selectedAction;

        public ObservableCollection<ImpactAction> Actions
        {
            get => _actions;
            set => SetProperty(ref _actions, value);
        }

        public ImpactAction? SelectedAction
        {
            get => _selectedAction;
            set => SetProperty(ref _selectedAction, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public ActionManageViewModel()
        {
            _db = DatabaseSimulator.Instance;
            _actions = new ObservableCollection<ImpactAction>();

            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, CanEdit);
            DeleteCommand = new RelayCommand(Delete, CanDelete);

            LoadData();
        }

        private void LoadData()
        {
            Actions.Clear();
            foreach (var action in _db.GetAllImpactActions())
            {
                Actions.Add(action);
            }
        }

        private void Add(object? parameter)
        {
            var dialog = new ActionDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _db.AddImpactAction(dialog.Action);
                    LoadData();
                    MessageBox.Show("Action added successfully", "Success", 
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
            return SelectedAction != null;
        }

        private void Edit(object? parameter)
        {
            if (SelectedAction == null)
            {
                MessageBox.Show("Please select an Action to edit", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a copy for editing
            var editCopy = new ImpactAction
            {
                ImpactName = SelectedAction.ImpactName,
                GroupFieldValue = SelectedAction.GroupFieldValue,
                ActionDesc = SelectedAction.ActionDesc,
                OpOwner = SelectedAction.OpOwner,
                ActionOwner = SelectedAction.ActionOwner,
                ActionTool = SelectedAction.ActionTool
            };

            var dialog = new ActionDialog(editCopy);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var index = Actions.IndexOf(SelectedAction);
                    _db.UpdateImpactAction(index, dialog.Action);
                    LoadData();
                    SelectedAction = null;
                    MessageBox.Show("Action updated successfully", "Success", 
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
            return SelectedAction != null;
        }

        private void Delete(object? parameter)
        {
            try
            {
                if (SelectedAction == null)
                {
                    MessageBox.Show("Please select an Action to delete", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this action?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _db.DeleteImpactAction(SelectedAction);
                    LoadData();
                    SelectedAction = null;
                    MessageBox.Show("Action deleted successfully", "Success", 
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
