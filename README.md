# Impact Management System

一个基于 C# WPF 的影响管理系统，用于管理数据库连接、异常影响分析和执行 SQL 查询。

## 技术架构

- **框架**: C# .NET 8.0 + WPF
- **模式**: MVVM (Model-View-ViewModel)
- **数据库**: Oracle DB (当前使用模拟器)

## 项目结构

```
ImpactManagement/
├── Models/                      # 数据模型
│   ├── DbConnect.cs            # 数据库连接模型
│   ├── AbnormalImpact.cs       # 异常影响模型
│   ├── ImpactSqlReplace.cs     # SQL替换项模型
│   └── ImpactAction.cs         # 影响操作模型
├── Data/                        # 数据层
│   └── DatabaseSimulator.cs    # 数据库模拟器（模拟Oracle操作）
├── ViewModels/                  # 视图模型
│   ├── ViewModelBase.cs        # ViewModel基类
│   ├── MainViewModel.cs        # 主窗口ViewModel
│   ├── DbConnectManageViewModel.cs
│   ├── ImpactManageViewModel.cs
│   ├── ActionManageViewModel.cs
│   └── ImpactExecuteViewModel.cs
├── Views/                       # 视图
│   ├── DbConnectManageView.xaml
│   ├── ImpactManageView.xaml
│   ├── ActionManageView.xaml
│   └── ImpactExecuteView.xaml
├── Commands/                    # 命令
│   └── RelayCommand.cs         # 通用命令实现
└── MainWindow.xaml             # 主窗口
```

## 数据库表结构

### 表1: db_connect
- `connectName` (唯一键) - 连接名称
- `ip` - IP地址
- `port` - 端口
- `schema` - Schema名称

### 表2: abnormal_impact
- `sceneName` - 场景名称
- `impactName` (唯一键) - 影响名称
- `sql` - SQL语句
- `groupField` - 分组字段
- `connectName` - 连接名称（外键）

### 表3: impact_sql_replace
- `impactId` - 影响ID
- `replaceItem` - 替换项

### 表4: impact_action
- `impactName` - 影响名称
- `groupFieldValue` - 分组字段值（可为空）
- `actionDesc` - 操作描述
- `opOwner` - 操作负责人
- `actionOwner` - 行动负责人
- `actionTool` - 行动工具

## 功能模块

### 1. DB Connect Manage (数据库连接管理)
- ✅ 添加数据库连接
- ✅ 更新数据库连接
- ✅ 删除数据库连接
- ✅ 查询数据库连接

**功能说明**：
- ConnectName 必须唯一
- 支持配置 Oracle 数据库的 IP、端口和 Schema

### 2. Impact Manage (影响管理)
- ✅ 添加异常影响
- ✅ 更新异常影响
- ✅ 删除异常影响
- ✅ 查询异常影响
- ✅ SQL语句重点展示（使用大文本框）
- ✅ 配置 SQL 替换项（支持多个）
- ✅ ConnectName 下拉框选择

**功能说明**：
- ImpactName 必须唯一
- SQL 字段为关键内容，使用大文本框显示
- 可以为 SQL 配置多个替换项（如 {STATUS}, {DATE}）
- 替换项保存到 impact_sql_replace 表

### 3. Action Manage (操作管理)
- ✅ 添加操作
- ✅ 更新操作
- ✅ 删除操作
- ✅ 查询操作
- ✅ ImpactName 下拉框选择

**功能说明**：
- 通过下拉框选择已存在的 ImpactName
- GroupFieldValue 可为空
- 支持配置操作描述、负责人和工具

### 4. Impact Execute (影响执行) ⭐ 核心功能
- ✅ 按场景名称筛选
- ✅ 选择影响名称
- ✅ 显示 SQL 语句（重点展示）
- ✅ 显示数据库连接信息
- ✅ 替换项输入（如果配置了替换项）
- ✅ 执行 SQL 查询
- ✅ 分页表格展示查询结果（重点展示）
- ✅ 导出查询结果到 CSV
- ✅ 按分组字段过滤（如果 groupFlag 为 true）
- ✅ 显示相关操作

**功能说明**：
1. 选择 SceneName 后，自动加载对应的 ImpactName 列表
2. 选择 ImpactName 后：
   - 显示 SQL 语句
   - 显示连接信息：`ConnectName - IP:Port/Schema`
   - 如果有 replaceItem，显示输入框供用户填值
3. 点击 "Execute Query" 执行 SQL：
   - 使用用户输入的值替换 SQL 中的替换项
   - 在表格中显示查询结果
4. 如果 groupFlag 为 true：
   - 显示 GroupFieldValue 下拉框
   - 选择后过滤表格数据
   - 显示该分组值对应的 Actions
5. 导出功能：将查询结果导出为 CSV 文件

## 使用说明

### 运行应用
```bash
cd ImpactManagement
dotnet run
```

### 操作流程示例

#### 1. 配置数据库连接
1. 点击 "DB Connect Manage"
2. 填写连接信息：ConnectName, IP, Port, Schema
3. 点击 "Add" 添加

#### 2. 配置影响
1. 点击 "Impact Manage"
2. 填写影响信息：
   - Scene Name: 如 "Performance Issue"
   - Impact Name: 如 "Slow Query Impact"
   - SQL: `SELECT * FROM USERS WHERE status = '{STATUS}' AND created_date > '{DATE}'`
   - Group Field: 如 "department"
   - Connect Name: 从下拉框选择
3. 添加替换项：
   - 输入 `{STATUS}`，点击 "Add Item"
   - 输入 `{DATE}`，点击 "Add Item"
4. 点击 "Add" 保存

#### 3. 配置操作
1. 点击 "Action Manage"
2. 从下拉框选择 Impact Name
3. 填写 Group Field Value（如 "IT"）
4. 填写操作描述、负责人等信息
5. 点击 "Add" 保存

#### 4. 执行查询
1. 点击 "Impact Execute"
2. 选择 Scene Name
3. 选择 Impact Name
4. 查看 SQL 和连接信息
5. 如果有替换项，填入值：
   - {STATUS}: active
   - {DATE}: 2024-01-01
6. 点击 "Execute Query"
7. 查看查询结果
8. （可选）选择 Group Field Value 进行过滤
9. （可选）点击 "Export to CSV" 导出数据

## 特色功能

### 🎯 重点内容展示
1. **SQL 语句**：
   - Impact Manage 页面：大文本框显示，支持多行
   - Impact Execute 页面：只读文本框，灰色背景突出显示

2. **查询结果**：
   - 使用 DataGrid 自动生成列
   - 支持排序、选择
   - 斑马纹显示（交替行背景色）
   - 蓝色表头，网格线清晰
   - 支持水平和垂直滚动

### 🔧 数据库模拟器
由于当前没有实际的 Oracle 数据库，使用 `DatabaseSimulator` 类模拟：
- 使用 4 个 List 分别存储 4 个表的数据
- 提供完整的增删改查方法
- 内置示例数据
- 模拟 SQL 执行，返回示例结果

### 🎨 界面设计
- 主界面顶部导航栏（蓝色背景）
- 4个页面通过按钮切换
- 左右分栏布局：左侧列表，右侧表单
- 响应式设计，支持窗口缩放

## 技术亮点

1. **MVVM 模式**：完全分离视图和业务逻辑
2. **数据绑定**：所有UI更新自动化
3. **命令模式**：使用 ICommand 实现按钮操作
4. **单例模式**：DatabaseSimulator 使用单例模式
5. **ObservableCollection**：自动通知UI数据变化
6. **DataTemplate**：自动视图切换
7. **值转换器**：Bool/Count 到 Visibility 的转换

## 扩展到真实数据库

要连接真实的 Oracle 数据库，需要：

1. 安装 Oracle.ManagedDataAccess NuGet 包：
```bash
dotnet add package Oracle.ManagedDataAccess.Core
```

2. 在 `DatabaseSimulator.cs` 中替换模拟代码为真实的数据库操作：
```csharp
// 示例：执行查询
public DataTable ExecuteSql(string sql, Dictionary<string, string>? replacements = null)
{
    string connectionString = "Data Source=...;User Id=...;Password=...;";
    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        conn.Open();
        using (OracleCommand cmd = new OracleCommand(sql, conn))
        {
            using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}
```

## 系统要求

- .NET 8.0 SDK
- Windows OS
- Visual Studio 2022 或 VS Code (可选)

## 许可证

MIT License

## 作者

Created by AI Assistant for demo8-impact-qoder project
