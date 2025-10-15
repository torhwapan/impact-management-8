# 项目文件结构

```
demo8-impact-qoder/
│
├── 📄 README.md                           # 技术文档和系统说明
├── 📄 QUICKSTART.md                       # 快速开始指南
├── 📄 用户操作手册.md                      # 详细的用户使用指南
├── 📄 项目完成总结.md                      # 项目总结和验收说明
│
└── ImpactManagement/                      # 主项目目录
    │
    ├── 📄 App.xaml                        # WPF应用程序定义
    ├── 📄 App.xaml.cs                     # 应用程序代码隐藏
    ├── 📄 MainWindow.xaml                 # 主窗口界面（导航栏+视图容器）
    ├── 📄 MainWindow.xaml.cs              # 主窗口代码隐藏
    ├── 📄 AssemblyInfo.cs                 # 程序集信息
    ├── 📄 ImpactManagement.csproj         # 项目配置文件
    │
    ├── 📁 Models/                         # 数据模型层（4个表对应4个模型）
    │   ├── DbConnect.cs                   # 数据库连接模型
    │   ├── AbnormalImpact.cs              # 异常影响模型
    │   ├── ImpactSqlReplace.cs            # SQL替换项模型
    │   └── ImpactAction.cs                # 影响操作模型
    │
    ├── 📁 Data/                           # 数据访问层
    │   └── DatabaseSimulator.cs           # 数据库模拟器（单例模式）
    │                                      # - 4个List模拟4个表
    │                                      # - 完整的CRUD方法
    │                                      # - SQL执行模拟
    │                                      # - 内置示例数据
    │
    ├── 📁 ViewModels/                     # 视图模型层（MVVM模式）
    │   ├── ViewModelBase.cs               # ViewModel基类
    │   │                                  # - INotifyPropertyChanged实现
    │   │                                  # - SetProperty辅助方法
    │   │
    │   ├── MainViewModel.cs               # 主窗口ViewModel
    │   │                                  # - 管理4个子ViewModel
    │   │                                  # - 视图切换命令
    │   │
    │   ├── DbConnectManageViewModel.cs    # DB连接管理ViewModel
    │   │                                  # - 增删改查功能
    │   │                                  # - 数据验证逻辑
    │   │
    │   ├── ImpactManageViewModel.cs       # 影响管理ViewModel
    │   │                                  # - Impact CRUD
    │   │                                  # - ReplaceItem管理
    │   │
    │   ├── ActionManageViewModel.cs       # 操作管理ViewModel
    │   │                                  # - Action CRUD
    │   │
    │   └── ImpactExecuteViewModel.cs      # 影响执行ViewModel（核心）
    │                                      # - SQL查询执行
    │                                      # - 结果展示和过滤
    │                                      # - CSV导出
    │                                      # - 分组功能
    │
    ├── 📁 Views/                          # 视图层（用户界面）
    │   ├── DbConnectManageView.xaml       # DB连接管理界面
    │   ├── DbConnectManageView.xaml.cs    # 代码隐藏
    │   │
    │   ├── ImpactManageView.xaml          # 影响管理界面
    │   ├── ImpactManageView.xaml.cs       # 代码隐藏
    │   │
    │   ├── ActionManageView.xaml          # 操作管理界面
    │   ├── ActionManageView.xaml.cs       # 代码隐藏
    │   │
    │   ├── ImpactExecuteView.xaml         # 影响执行界面（核心页面）
    │   └── ImpactExecuteView.xaml.cs      # 代码隐藏 + 值转换器
    │
    ├── 📁 Commands/                       # 命令层
    │   └── RelayCommand.cs                # 通用命令实现（ICommand）
    │
    ├── 📁 bin/                            # 编译输出目录
    │   └── Debug/
    │       └── net8.0-windows/
    │           └── ImpactManagement.exe   # 可执行文件
    │
    └── 📁 obj/                            # 编译临时文件目录
```

## 文件统计

### 代码文件
- **Models**: 4 个文件
- **ViewModels**: 6 个文件
- **Views**: 8 个文件（4个XAML + 4个CS）
- **Data**: 1 个文件
- **Commands**: 1 个文件
- **Main**: 4 个文件（App, MainWindow各2个）

**总计**: 约 24 个代码文件

### 代码行数估算
- Models: ~50 行
- Data/DatabaseSimulator: ~290 行
- ViewModels: ~1400 行
- Views (XAML): ~600 行
- Views (CS): ~80 行
- Commands: ~35 行
- Main: ~100 行

**总计**: 约 2550+ 行代码

### 文档文件
- README.md: ~260 行
- QUICKSTART.md: ~140 行
- 用户操作手册.md: ~430 行
- 项目完成总结.md: ~350 行

**总计**: 约 1180 行文档

## 主要技术组件说明

### 1. MVVM 架构
```
View (XAML)
    ↕ (DataBinding)
ViewModel (业务逻辑)
    ↕ (数据访问)
Model (数据模型)
    ↕
Data (数据层)
```

### 2. 数据流向

**查询操作流程:**
```
用户操作 → View → Command → ViewModel → DatabaseSimulator → 返回结果 → ViewModel → 更新ObservableCollection → View自动刷新
```

**CRUD操作流程:**
```
用户输入 → View绑定 → ViewModel属性 → Command执行 → DatabaseSimulator方法 → 更新List → LoadData → 刷新UI
```

### 3. 关键技术点

| 技术 | 用途 | 位置 |
|------|------|------|
| INotifyPropertyChanged | 属性变更通知 | ViewModelBase |
| ObservableCollection | 集合变更通知 | 所有ViewModel |
| ICommand | 命令绑定 | RelayCommand |
| DataTemplate | 视图自动切换 | MainWindow.xaml |
| IValueConverter | 值转换 | ImpactExecuteView.xaml.cs |
| DataTable | SQL结果存储 | DatabaseSimulator |
| Singleton | 全局数据访问 | DatabaseSimulator |

## 设计模式应用

1. **MVVM模式** - 整体架构
2. **单例模式** - DatabaseSimulator
3. **命令模式** - RelayCommand
4. **观察者模式** - INotifyPropertyChanged
5. **模板方法模式** - ViewModelBase

## 依赖关系

```
MainWindow
    ├─ MainViewModel
    │   ├─ DbConnectManageViewModel
    │   ├─ ImpactManageViewModel
    │   ├─ ActionManageViewModel
    │   └─ ImpactExecuteViewModel
    │
    └─ Views (自动匹配ViewModel)
        ├─ DbConnectManageView
        ├─ ImpactManageView
        ├─ ActionManageView
        └─ ImpactExecuteView

所有ViewModel → DatabaseSimulator (单例)
所有ViewModel → RelayCommand
所有ViewModel → ViewModelBase
```

## 运行要求

- **.NET SDK**: 8.0+
- **OS**: Windows (WPF框架限制)
- **内存**: 建议 4GB+
- **磁盘**: 约 50MB

## 编译输出

编译后生成的主要文件：
```
bin/Debug/net8.0-windows/
├── ImpactManagement.exe          # 可执行文件
├── ImpactManagement.dll          # 主程序集
├── ImpactManagement.pdb          # 调试符号
└── ImpactManagement.deps.json    # 依赖清单
```

## 快速定位文件

### 需要修改配置？
→ `ImpactManagement.csproj`

### 需要修改主界面？
→ `MainWindow.xaml`

### 需要修改数据库逻辑？
→ `Data/DatabaseSimulator.cs`

### 需要修改某个页面？
→ `Views/[PageName]View.xaml`
→ `ViewModels/[PageName]ViewModel.cs`

### 需要添加新模型？
→ `Models/` 目录

### 需要添加新命令？
→ `Commands/` 目录

### 需要修改示例数据？
→ `Data/DatabaseSimulator.cs` → `InitializeSampleData()`

---

**项目结构清晰，文件组织规范，易于维护和扩展！**
