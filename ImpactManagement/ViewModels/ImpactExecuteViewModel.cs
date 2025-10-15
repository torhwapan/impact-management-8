using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ImpactManagement.Commands;
using ImpactManagement.Data;
using ImpactManagement.Models;
using Microsoft.Win32;

namespace ImpactManagement.ViewModels
{
    public class ReplaceItemValue : ViewModelBase
    {
        private string _item = string.Empty;
        private string _value = string.Empty;

        public string Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }

    public class ImpactExecuteViewModel : ViewModelBase
    {
        private readonly DatabaseSimulator _db;
        private ObservableCollection<string> _sceneNames;
        private ObservableCollection<string> _impactNames;
        private ObservableCollection<string> _groupFieldValues;
        private ObservableCollection<ReplaceItemValue> _replaceItemValues;
        private ObservableCollection<ImpactAction> _actions;
        private string _selectedSceneName = string.Empty;
        private string _selectedImpactName = string.Empty;
        private string _selectedGroupFieldValue = string.Empty;
        private string _sql = string.Empty;
        private string _connectionInfo = string.Empty;
        private bool _hasReplaceItems;
        private bool _hasGroupField;
        private DataTable? _queryResults;
        private DataTable? _fullQueryResults;

        public ObservableCollection<string> SceneNames
        {
            get => _sceneNames;
            set => SetProperty(ref _sceneNames, value);
        }

        public ObservableCollection<string> ImpactNames
        {
            get => _impactNames;
            set => SetProperty(ref _impactNames, value);
        }

        public ObservableCollection<string> GroupFieldValues
        {
            get => _groupFieldValues;
            set => SetProperty(ref _groupFieldValues, value);
        }

        public ObservableCollection<ReplaceItemValue> ReplaceItemValues
        {
            get => _replaceItemValues;
            set => SetProperty(ref _replaceItemValues, value);
        }

        public ObservableCollection<ImpactAction> Actions
        {
            get => _actions;
            set => SetProperty(ref _actions, value);
        }

        public string SelectedSceneName
        {
            get => _selectedSceneName;
            set
            {
                SetProperty(ref _selectedSceneName, value);
                LoadImpactNamesByScene();
            }
        }

        public string SelectedImpactName
        {
            get => _selectedImpactName;
            set
            {
                SetProperty(ref _selectedImpactName, value);
                LoadImpactDetails();
            }
        }

        public string SelectedGroupFieldValue
        {
            get => _selectedGroupFieldValue;
            set
            {
                SetProperty(ref _selectedGroupFieldValue, value);
                FilterResultsByGroupValue();
                LoadActions();
            }
        }

        public string Sql
        {
            get => _sql;
            set => SetProperty(ref _sql, value);
        }

        public string ConnectionInfo
        {
            get => _connectionInfo;
            set => SetProperty(ref _connectionInfo, value);
        }

        public bool HasReplaceItems
        {
            get => _hasReplaceItems;
            set => SetProperty(ref _hasReplaceItems, value);
        }

        public bool HasGroupField
        {
            get => _hasGroupField;
            set => SetProperty(ref _hasGroupField, value);
        }

        public DataTable? QueryResults
        {
            get => _queryResults;
            set => SetProperty(ref _queryResults, value);
        }

        public ICommand QueryCommand { get; }
        public ICommand ExportCommand { get; }

        public ImpactExecuteViewModel()
        {
            _db = DatabaseSimulator.Instance;
            _sceneNames = new ObservableCollection<string>();
            _impactNames = new ObservableCollection<string>();
            _groupFieldValues = new ObservableCollection<string>();
            _replaceItemValues = new ObservableCollection<ReplaceItemValue>();
            _actions = new ObservableCollection<ImpactAction>();

            QueryCommand = new RelayCommand(ExecuteQuery, CanExecuteQuery);
            ExportCommand = new RelayCommand(Export, CanExport);

            LoadSceneNames();
        }

        private void LoadSceneNames()
        {
            SceneNames.Clear();
            var scenes = _db.GetAllAbnormalImpacts()
                .Select(i => i.SceneName)
                .Distinct()
                .OrderBy(s => s);

            foreach (var scene in scenes)
            {
                SceneNames.Add(scene);
            }
        }

        private void LoadImpactNamesByScene()
        {
            ImpactNames.Clear();
            if (!string.IsNullOrWhiteSpace(SelectedSceneName))
            {
                var impacts = _db.GetAllAbnormalImpacts()
                    .Where(i => i.SceneName == SelectedSceneName)
                    .Select(i => i.ImpactName);

                foreach (var impact in impacts)
                {
                    ImpactNames.Add(impact);
                }
            }
        }

        private void LoadImpactDetails()
        {
            if (string.IsNullOrWhiteSpace(SelectedImpactName))
            {
                Sql = string.Empty;
                ConnectionInfo = string.Empty;
                HasReplaceItems = false;
                HasGroupField = false;
                ReplaceItemValues.Clear();
                return;
            }

            var impact = _db.GetAllAbnormalImpacts()
                .FirstOrDefault(i => i.ImpactName == SelectedImpactName);

            if (impact != null)
            {
                Sql = impact.Sql;

                // Load connection info
                var dbConnect = _db.GetAllDbConnects()
                    .FirstOrDefault(d => d.ConnectName == impact.ConnectName);

                if (dbConnect != null)
                {
                    ConnectionInfo = $"{dbConnect.ConnectName} - {dbConnect.Ip}:{dbConnect.Port}/{dbConnect.Schema}";
                }

                // Load replace items
                var replaceItems = _db.GetReplaceItemsByImpactId(impact.ImpactName);
                ReplaceItemValues.Clear();
                foreach (var item in replaceItems)
                {
                    ReplaceItemValues.Add(new ReplaceItemValue { Item = item.ReplaceItem });
                }
                HasReplaceItems = ReplaceItemValues.Count > 0;

                // Check if has group field
                HasGroupField = impact.GroupFlag;
            }

            // Clear previous results
            QueryResults = null;
            _fullQueryResults = null;
            GroupFieldValues.Clear();
            Actions.Clear();
        }

        private bool CanExecuteQuery(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(SelectedImpactName) && !string.IsNullOrWhiteSpace(Sql);
        }

        private void ExecuteQuery(object? parameter)
        {
            try
            {
                // Prepare replacements
                Dictionary<string, string> replacements = new Dictionary<string, string>();
                foreach (var item in ReplaceItemValues)
                {
                    if (string.IsNullOrWhiteSpace(item.Value))
                    {
                        MessageBox.Show($"Please provide value for {item.Item}", "Validation Error", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    replacements[item.Item] = item.Value;
                }

                // Execute SQL
                var results = _db.ExecuteSql(Sql, replacements);
                _fullQueryResults = results;
                QueryResults = results;

                // Load group field values if applicable
                if (HasGroupField)
                {
                    var impact = _db.GetAllAbnormalImpacts()
                        .FirstOrDefault(i => i.ImpactName == SelectedImpactName);

                    if (impact != null && !string.IsNullOrWhiteSpace(impact.GroupField))
                    {
                        GroupFieldValues.Clear();
                        if (results.Columns.Contains(impact.GroupField))
                        {
                            var values = results.AsEnumerable()
                                .Select(r => r[impact.GroupField]?.ToString() ?? string.Empty)
                                .Where(v => !string.IsNullOrWhiteSpace(v))
                                .Distinct()
                                .OrderBy(v => v);

                            foreach (var value in values)
                            {
                                GroupFieldValues.Add(value);
                            }
                        }
                    }
                }

                MessageBox.Show($"Query executed successfully. {results.Rows.Count} rows returned.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing query: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterResultsByGroupValue()
        {
            if (_fullQueryResults == null || string.IsNullOrWhiteSpace(SelectedGroupFieldValue))
            {
                QueryResults = _fullQueryResults;
                return;
            }

            var impact = _db.GetAllAbnormalImpacts()
                .FirstOrDefault(i => i.ImpactName == SelectedImpactName);

            if (impact != null && !string.IsNullOrWhiteSpace(impact.GroupField))
            {
                if (_fullQueryResults.Columns.Contains(impact.GroupField))
                {
                    var filteredView = _fullQueryResults.AsEnumerable()
                        .Where(r => r[impact.GroupField]?.ToString() == SelectedGroupFieldValue);

                    if (filteredView.Any())
                    {
                        QueryResults = filteredView.CopyToDataTable();
                    }
                    else
                    {
                        QueryResults = _fullQueryResults.Clone(); // Empty table with same structure
                    }
                }
            }
        }

        private void LoadActions()
        {
            Actions.Clear();
            if (!string.IsNullOrWhiteSpace(SelectedImpactName) && !string.IsNullOrWhiteSpace(SelectedGroupFieldValue))
            {
                var actions = _db.GetActionsByImpactNameAndGroupValue(SelectedImpactName, SelectedGroupFieldValue);
                foreach (var action in actions)
                {
                    Actions.Add(action);
                }
            }
        }

        private bool CanExport(object? parameter)
        {
            return QueryResults != null && QueryResults.Rows.Count > 0;
        }

        private void Export(object? parameter)
        {
            try
            {
                if (QueryResults == null)
                    return;

                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    FileName = $"QueryResults_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToCsv(QueryResults, saveDialog.FileName);
                    MessageBox.Show($"Data exported successfully to {saveDialog.FileName}", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToCsv(DataTable dataTable, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // Write headers
            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>()
                .Select(column => EscapeCsvField(column.ColumnName));
            sb.AppendLine(string.Join(",", columnNames));

            // Write rows
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => 
                    EscapeCsvField(field?.ToString() ?? string.Empty));
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}
