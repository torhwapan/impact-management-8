# Quick Start Guide - Impact Management System

## 快速开始（3分钟上手）

### 1️⃣ 启动应用

```bash
cd ImpactManagement
dotnet run
```

应用会自动打开，显示主界面。

### 2️⃣ 界面说明

**顶部蓝色导航栏** - 4个功能按钮：
- 🎯 **Impact Execute** - 执行查询（主要功能）
- 🗄️ **DB Connect Manage** - 管理数据库连接
- 📋 **Impact Manage** - 管理影响配置
- ⚙️ **Action Manage** - 管理操作建议

### 3️⃣ 快速体验（已有示例数据）

#### 体验查询功能（最重要）

1. 确保在 **Impact Execute** 页面
2. 选择 Scene Name: `Performance Issue`
3. 选择 Impact Name: `Slow Query Impact`
4. 填写替换项:
   - `{STATUS}`: `active`
   - `{DATE}`: `2024-01-01`
5. 点击绿色 **Execute Query** 按钮
6. 查看查询结果（蓝色表头的大表格）
7. 选择 Group Value: `IT`
8. 查看过滤后的结果和相关操作

#### 体验导出功能

1. 执行查询后（上一步）
2. 点击 **Export to CSV** 按钮
3. 选择保存位置
4. 打开 CSV 文件查看

### 4️⃣ 添加自己的配置

#### 添加数据库连接

1. 点击 **DB Connect Manage**
2. 填写:
   - Connect Name: `MyDB`
   - IP: `192.168.1.200`
   - Port: `1521`
   - Schema: `MY_SCHEMA`
3. 点击 **Add**

#### 添加影响配置

1. 点击 **Impact Manage**
2. 填写:
   - Scene Name: `My Test`
   - Impact Name: `Test Impact`
   - Connect Name: 选择 `MyDB`
   - SQL: `SELECT * FROM MY_TABLE WHERE id = '{ID}'`
   - Group Field: 留空或填写列名
3. 添加替换项:
   - 输入 `{ID}`，点击 **Add Item**
4. 点击 **Add**

#### 执行你的查询

1. 点击 **Impact Execute**
2. 选择你的 Scene 和 Impact
3. 填写 `{ID}` 的值
4. 点击 **Execute Query**
5. 查看结果

### 5️⃣ 常用操作

| 操作 | 步骤 |
|------|------|
| 修改配置 | 列表中点击选择 → 修改右侧表单 → 点击 Update |
| 删除配置 | 列表中点击选择 → 点击 Delete → 确认 |
| 清空表单 | 点击 Clear 按钮 |
| 导出结果 | 执行查询后 → Export to CSV |
| 分组过滤 | 执行查询后 → 选择 Group Value |

### 6️⃣ 重要提示

✅ **推荐流程**:
1. 先配置 DB Connect
2. 再配置 Impact（含 SQL 和替换项）
3. 可选配置 Action
4. 最后在 Execute 页面执行

⚠️ **注意事项**:
- ConnectName 和 ImpactName 必须唯一
- 替换项格式: `{PARAM_NAME}`（必须匹配）
- SQL 字段可多行编辑
- 当前使用模拟数据，返回示例结果

🎯 **核心页面**: Impact Execute
- 这是最重要的页面
- 其他页面都是为它服务的配置页面

### 7️⃣ 示例数据说明

应用已内置示例数据，可以直接使用：

**数据库连接:**
- ProductionDB
- TestDB

**影响配置:**
- Performance Issue → Slow Query Impact（有替换项和分组）
- Data Quality → Missing Data Impact（无替换项）

**操作建议:**
- Slow Query Impact 的 IT 部门操作
- Slow Query Impact 的 Sales 部门操作

### 8️⃣ 需要帮助？

- 📖 详细说明: 查看 `用户操作手册.md`
- 💻 技术文档: 查看 `README.md`
- 📝 项目总结: 查看 `项目完成总结.md`

---

**开始使用吧！** 🚀

最简单的体验方式：
1. `dotnet run`
2. Impact Execute 页面
3. 选择示例数据
4. Execute Query
5. 查看结果

就是这么简单！
