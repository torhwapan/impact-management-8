using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ImpactManagement.Models;

namespace ImpactManagement.Data
{
    public class DatabaseSimulator
    {
        private static DatabaseSimulator? _instance;
        private static readonly object _lock = new object();

        public static DatabaseSimulator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DatabaseSimulator();
                        }
                    }
                }
                return _instance;
            }
        }

        private List<DbConnect> _dbConnects = new List<DbConnect>();
        private List<AbnormalImpact> _abnormalImpacts = new List<AbnormalImpact>();
        private List<ImpactSqlReplace> _impactSqlReplaces = new List<ImpactSqlReplace>();
        private List<ImpactAction> _impactActions = new List<ImpactAction>();

        private DatabaseSimulator()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // Sample DB Connections
            _dbConnects.Add(new DbConnect 
            { 
                ConnectName = "ProductionDB", 
                Ip = "192.168.1.100", 
                Port = "1521", 
                Schema = "PROD_SCHEMA" 
            });
            _dbConnects.Add(new DbConnect 
            { 
                ConnectName = "TestDB", 
                Ip = "192.168.1.101", 
                Port = "1521", 
                Schema = "TEST_SCHEMA" 
            });

            // Sample Abnormal Impacts
            _abnormalImpacts.Add(new AbnormalImpact
            {
                SceneName = "Performance Issue",
                ImpactName = "Slow Query Impact",
                Sql = "SELECT * FROM USERS WHERE status = '{STATUS}' AND created_date > '{DATE}'",
                GroupField = "department",
                ConnectName = "ProductionDB"
            });
            _abnormalImpacts.Add(new AbnormalImpact
            {
                SceneName = "Data Quality",
                ImpactName = "Missing Data Impact",
                Sql = "SELECT * FROM ORDERS WHERE order_status IS NULL",
                GroupField = "",
                ConnectName = "ProductionDB"
            });

            // Sample Replace Items
            _impactSqlReplaces.Add(new ImpactSqlReplace
            {
                ImpactId = "Slow Query Impact",
                ReplaceItem = "{STATUS}"
            });
            _impactSqlReplaces.Add(new ImpactSqlReplace
            {
                ImpactId = "Slow Query Impact",
                ReplaceItem = "{DATE}"
            });

            // Sample Actions
            _impactActions.Add(new ImpactAction
            {
                ImpactName = "Slow Query Impact",
                GroupFieldValue = "IT",
                ActionDesc = "Optimize query and add indexes",
                OpOwner = "John Doe",
                ActionOwner = "Jane Smith",
                ActionTool = "SQL Optimizer"
            });
            _impactActions.Add(new ImpactAction
            {
                ImpactName = "Slow Query Impact",
                GroupFieldValue = "Sales",
                ActionDesc = "Review data access patterns",
                OpOwner = "Bob Johnson",
                ActionOwner = "Alice Williams",
                ActionTool = "Performance Monitor"
            });
        }

        // DB Connect CRUD operations
        public List<DbConnect> GetAllDbConnects() => new List<DbConnect>(_dbConnects);

        public void AddDbConnect(DbConnect dbConnect)
        {
            if (_dbConnects.Any(d => d.ConnectName == dbConnect.ConnectName))
                throw new Exception("ConnectName must be unique");
            _dbConnects.Add(dbConnect);
        }

        public void UpdateDbConnect(string originalConnectName, DbConnect dbConnect)
        {
            var existing = _dbConnects.FirstOrDefault(d => d.ConnectName == originalConnectName);
            if (existing == null)
                throw new Exception("DbConnect not found");
            
            if (originalConnectName != dbConnect.ConnectName && 
                _dbConnects.Any(d => d.ConnectName == dbConnect.ConnectName))
                throw new Exception("ConnectName must be unique");

            existing.ConnectName = dbConnect.ConnectName;
            existing.Ip = dbConnect.Ip;
            existing.Port = dbConnect.Port;
            existing.Schema = dbConnect.Schema;
        }

        public void DeleteDbConnect(string connectName)
        {
            var dbConnect = _dbConnects.FirstOrDefault(d => d.ConnectName == connectName);
            if (dbConnect != null)
                _dbConnects.Remove(dbConnect);
        }

        // Abnormal Impact CRUD operations
        public List<AbnormalImpact> GetAllAbnormalImpacts() => new List<AbnormalImpact>(_abnormalImpacts);

        public void AddAbnormalImpact(AbnormalImpact impact)
        {
            if (_abnormalImpacts.Any(i => i.ImpactName == impact.ImpactName))
                throw new Exception("ImpactName must be unique");
            _abnormalImpacts.Add(impact);
        }

        public void UpdateAbnormalImpact(string originalImpactName, AbnormalImpact impact)
        {
            var existing = _abnormalImpacts.FirstOrDefault(i => i.ImpactName == originalImpactName);
            if (existing == null)
                throw new Exception("AbnormalImpact not found");

            if (originalImpactName != impact.ImpactName && 
                _abnormalImpacts.Any(i => i.ImpactName == impact.ImpactName))
                throw new Exception("ImpactName must be unique");

            existing.SceneName = impact.SceneName;
            existing.ImpactName = impact.ImpactName;
            existing.Sql = impact.Sql;
            existing.GroupField = impact.GroupField;
            existing.ConnectName = impact.ConnectName;
        }

        public void DeleteAbnormalImpact(string impactName)
        {
            var impact = _abnormalImpacts.FirstOrDefault(i => i.ImpactName == impactName);
            if (impact != null)
                _abnormalImpacts.Remove(impact);
        }

        // Impact SQL Replace CRUD operations
        public List<ImpactSqlReplace> GetAllImpactSqlReplaces() => new List<ImpactSqlReplace>(_impactSqlReplaces);

        public List<ImpactSqlReplace> GetReplaceItemsByImpactId(string impactId)
        {
            return _impactSqlReplaces.Where(r => r.ImpactId == impactId).ToList();
        }

        public void AddImpactSqlReplace(ImpactSqlReplace replace)
        {
            _impactSqlReplaces.Add(replace);
        }

        public void DeleteImpactSqlReplacesByImpactId(string impactId)
        {
            _impactSqlReplaces.RemoveAll(r => r.ImpactId == impactId);
        }

        // Impact Action CRUD operations
        public List<ImpactAction> GetAllImpactActions() => new List<ImpactAction>(_impactActions);

        public List<ImpactAction> GetActionsByImpactName(string impactName)
        {
            return _impactActions.Where(a => a.ImpactName == impactName).ToList();
        }

        public List<ImpactAction> GetActionsByImpactNameAndGroupValue(string impactName, string groupFieldValue)
        {
            return _impactActions.Where(a => a.ImpactName == impactName && a.GroupFieldValue == groupFieldValue).ToList();
        }

        public void AddImpactAction(ImpactAction action)
        {
            _impactActions.Add(action);
        }

        public void UpdateImpactAction(int index, ImpactAction action)
        {
            if (index >= 0 && index < _impactActions.Count)
            {
                _impactActions[index] = action;
            }
        }

        public void DeleteImpactAction(ImpactAction action)
        {
            _impactActions.Remove(action);
        }

        // Execute SQL simulation
        public DataTable ExecuteSql(string sql, Dictionary<string, string>? replacements = null)
        {
            // Replace parameters if provided
            string executedSql = sql;
            if (replacements != null)
            {
                foreach (var replacement in replacements)
                {
                    executedSql = executedSql.Replace(replacement.Key, replacement.Value);
                }
            }

            // Simulate query execution - return sample data
            DataTable dataTable = new DataTable();
            
            // Add sample columns based on SQL content
            if (executedSql.ToUpper().Contains("USERS"))
            {
                dataTable.Columns.Add("user_id", typeof(int));
                dataTable.Columns.Add("username", typeof(string));
                dataTable.Columns.Add("status", typeof(string));
                dataTable.Columns.Add("department", typeof(string));
                dataTable.Columns.Add("created_date", typeof(DateTime));

                // Add sample rows
                dataTable.Rows.Add(1, "john.doe", "active", "IT", DateTime.Now.AddDays(-30));
                dataTable.Rows.Add(2, "jane.smith", "active", "Sales", DateTime.Now.AddDays(-25));
                dataTable.Rows.Add(3, "bob.johnson", "active", "IT", DateTime.Now.AddDays(-20));
                dataTable.Rows.Add(4, "alice.williams", "inactive", "Sales", DateTime.Now.AddDays(-15));
                dataTable.Rows.Add(5, "charlie.brown", "active", "HR", DateTime.Now.AddDays(-10));
            }
            else if (executedSql.ToUpper().Contains("ORDERS"))
            {
                dataTable.Columns.Add("order_id", typeof(int));
                dataTable.Columns.Add("customer_name", typeof(string));
                dataTable.Columns.Add("order_status", typeof(string));
                dataTable.Columns.Add("amount", typeof(decimal));
                dataTable.Columns.Add("order_date", typeof(DateTime));

                dataTable.Rows.Add(101, "Customer A", null, 1500.50m, DateTime.Now.AddDays(-5));
                dataTable.Rows.Add(102, "Customer B", "completed", 2300.75m, DateTime.Now.AddDays(-4));
                dataTable.Rows.Add(103, "Customer C", null, 890.00m, DateTime.Now.AddDays(-3));
            }
            else
            {
                // Generic sample data
                dataTable.Columns.Add("id", typeof(int));
                dataTable.Columns.Add("name", typeof(string));
                dataTable.Columns.Add("value", typeof(string));
                dataTable.Columns.Add("timestamp", typeof(DateTime));

                dataTable.Rows.Add(1, "Sample 1", "Value 1", DateTime.Now);
                dataTable.Rows.Add(2, "Sample 2", "Value 2", DateTime.Now);
                dataTable.Rows.Add(3, "Sample 3", "Value 3", DateTime.Now);
            }

            return dataTable;
        }
    }
}
