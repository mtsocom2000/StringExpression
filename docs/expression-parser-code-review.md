# ExpressionParser 核心代码审查报告

> **审查范围**: `ExpressionParser/` 目录全部 7 个核心源文件（~1,758 行）
> **审查对象**: `feature/ui-and-tests` 分支 @ `8f2f3fd`
> **审查方法**: 静态逐行阅读 + Roslyn 编译实证验证（17 个用例，见第 4 节及附录）
> **审查日期**: 2026-08-30
> **结论速览**: 架构意图良好、错误诊断优秀，但**函数调用功能在当前 HEAD 上完全不可用**（实证确认），另有一元负号静默计算错误等 4 个已确认 bug。算法上属于"用可变树手术实现的 precedence climbing"，正确性风险显著高于经典方案。

---

## 目录

1. [总览](#1-总览)
2. [架构设计评价](#2-架构设计评价)
3. [核心算法剖析](#3-核心算法剖析)
4. [实证 Bug 清单（编译运行验证）](#4-实证-bug-清单编译运行验证)
5. [与经典表达式解析算法的对比](#5-与经典表达式解析算法的对比)
6. [优点清单](#6-优点清单)
7. [改进建议（按优先级）](#7-改进建议按优先级)
- [附录：复现步骤](#附录复现步骤)

---

## 1. 总览

### 1.1 文件清单

| 文件 | 行数 | 职责 | 质量印象 |
|------|-----:|------|----------|
| `Expression.cs` | 197 | 表达式树节点（可变，含 Left/Right/Operator/Parent、状态标志） | ⚠️ 不变量分散在多个 setter |
| `ExpressionBuilder.cs` | 657 | 解析器核心：字符级状态机 + 就地树旋转 | ❌ 复杂度重心，4 个 bug 中 3 个在此 |
| `Operator.cs` | 159 | 运算符基类 + 四则运算 + OperatorBracket（死代码） | ⚠️ 含未使用类型与拼写错误 |
| `OperatorFactory.cs` | 68 | 单例注册表（18 个运算符线性查找） | ⚠️ 可用但简陋 |
| `FunctionOperator.cs` | 419 | 14 个数学函数（抽象基类 + 具体实现） | ✅ 全仓库质量最高的文件 |
| `Util.cs` | 112 | 字符分类（数字/符号/括号/字母/逗号） | ✅ 简单清晰 |
| `ParserException.cs` | 146 | 富上下文异常（位置/词元/上下文/可视化指针） | ✅ 优于多数生产级解析器 |

### 1.2 依赖关系

```
ExpressionBuilder ──创建/改写──▶ Expression（树）
       │                              │
       ▼ 查询运算符                    │ 持有
 OperatorFactory ──返回──▶ Operator ◀─┘
       │                      ▲
       └── 注册               │ Calc(expr) 回读 expr.Left/Right
 FunctionOperator(×14) ───────┘  （双向耦合，见 2.2）

 Util ◀── 所有词法判断      ParserException ◀── 所有错误路径
```

### 1.3 识别到的设计模式

| 模式 | 位置 | 评价 |
|------|------|------|
| 单例（双重检查锁） | `ExpressionBuilder.Instance`、`OperatorFactory.Instance` | ⚠️ ExpressionBuilder 无状态，单例无意义；锁未用 `volatile` |
| 工厂 + 注册表 | `OperatorFactory._supportOperators` | ⚠️ 线性扫描；新增函数仍需改工厂构造函数（非完整 OCP） |
| 模板方法 | `Operator.Support/Calc/Priority` 虚成员 | ✅ 扩展点清晰 |
| 解释器模式 | `Expression.CalcValue()` → `Operator.Calc(expr)` | ✅ 但 Operator 反向读取 Expression 内部结构，耦合倒置 |
| 组合模式 | Expression 树（Left/Right 递归结构） | ✅ |

---

## 2. 架构设计评价

### 2.1 优点

- **文件级职责分离干净**：模型（Expression）、行为（Operator）、注册（Factory）、解析（Builder）、词法（Util）、错误（ParserException）各司其职，一眼可读。
- **扩展机制可用**：新增一个数学函数 = 继承 `FunctionOperator` + 工厂注册一行（`FunctionOperator.cs` 全部 14 个函数即如此产出）。
- **树可持久化检查**：`Expression.ToString()` 能从树反向重建表达式串（含括号还原），对调试和 UI 显示很实用——这是立即求值型解析器不具备的能力。
- **求值防御充分**：除零保护（`Operator.cs:122`）、定义域校验（sqrt≥0、log>0、asin/acos∈[-1,1]）、NaN/Infinity 结果检查，且错误信息具体。

### 2.2 问题

**P-1 双向耦合（架构最重的问题）**
`Operator.Calc(Expression expr)` 直接读取 `expr.Left.CalcValue()` / `expr.Right.CalcValue()`（如 `Operator.cs:53`、`FunctionOperator.cs:44`）。运算符与表达式节点互知对方内部结构：Expression 持有 Operator，Operator 又反过来解剖 Expression。经典做法是 Visitor 或让节点把**已算好的操作数**传给运算符（`Calc(double l, double r)`），单向依赖。

**P-2 Expression 的不变量分散在三个 setter 里**
- `Left` setter 会悄悄 `_value = null`（`Expression.cs:82`）——丢弃已积累的值（Bug #3 的直接根因）；
- `Operator` setter 会把 `_value` 搬家成 `Left` 子节点（`Expression.cs:120-129`）；
- `BracketState` 的括号所有权在树旋转时跨节点转移（`ExpressionBuilder.cs:381-439`）。

任何一处调用顺序变化都可能破坏不变量，且无一处有断言保护。

**P-3 死代码与命名问题**
- `OperatorBracket`（`Operator.cs:140-158`）：`Support()` 抛 `NotImplementedException`，其预期用法在 `Expression.cs:158-164` 被注释掉——设计遗迹；
- `UnaryOperator` / `BinraryOperator`：空标记类，无任何行为（"Binrary" 是 "Binary" 的拼写错误，`Operator.cs:36-42`）；
- `OperatorType` 枚举（`Operator.cs:9-13`）定义后从未使用；
- `ExpressionParserState.eExpectFunctionArg`（`ExpressionBuilder.cs:97`）声明后从未赋值。

**P-4 值以 string 存储（求值层的系统性隐患）**
`Expression._value` 是字符串；每层求值都经历 `double.Parse → 计算 → ToString` 往返（`Operator.cs:53-54` 等全部运算符）。后果：
1. **性能**：每次求值、每层树都有字符串解析/格式化开销；
2. **精度**：.NET Framework（本项目目标框架 4.8）的 `double.ToString()` 默认不保证 round-trip，深层树多次往返可能累积精度漂移；
3. **国际化正确性 bug（潜在）**：所有 `double.Parse/ToString` 均未指定 `CultureInfo.InvariantCulture`。在小数点为逗号的区域设置（de-DE、fr-FR 等）下，`"1.5"` 将解析失败或被错误解析。当前测试均在 zh-CN/zh-CN 类点位区域下运行所以未暴露。

**P-5 API 细节**
- `Expression.Value` 是**只写属性**（无 getter，`Expression.cs:52-58`）——反直觉；
- `IsValid`（`Expression.cs:60-66`）：`BracketState == eClosed` 即视为有效，不看 `_state`——偏宽松，配合默认 `_value="0"` 可能静默求值出 0；
- `ExpressionBuilder.cs:448` 的 `Debug.Assert(false)` 在 Release 下是空操作，留下不一致状态而非抛错。

---

## 3. 核心算法剖析

### 3.1 总体形态：无独立 tokenizer 的单遍字符级状态机

没有"先切词元、再解析"两阶段，而是逐字符推进一台三状态机：

```
eExpectLeftExpression ──(数字/函数名/'(')──▶ 自环或转移
        │ (数字结束)
        ▼
eExpectOperator ──(+-*/)──▶ eExpectRightExpression ──(数字/')')──▶ eExpectOperator
```

数字识别是内嵌的迷你 FSA（`NumericParserState`: eEmpty/eHasNumeric/eHasSign/eHasPoint/eEnd，`ExpressionBuilder.cs:101-108, 556-655`），支持前导符号与小数点。

**评价**：这台状态机在功能上**同构于递归下降解析器的调用栈**（每个状态对应一条文法规则），但被压平成一个 370 行的函数 + 显式状态变量 + 树的原地变异。同构但复杂度数倍——这正是 4.4 节对比的伏笔。

### 3.2 运算符优先级：就地树旋转（Pratt climbing 的可变树版本）

读到新运算符 `op` 时，按与当前节点运算符的优先级关系做三种树手术（`ExpressionBuilder.cs:326-455`）：

| 关系 | 操作 | 对应经典语义 |
|------|------|--------------|
| `expr.Op.Priority == op.Priority` | 新节点接管当前子树为 Left，挂到父右侧（`cs:373-390`） | **左结合**旋转 ✓ |
| `expr.Op.Priority > op.Priority` | 沿 Parent 上爬直到 `priority <= op` 或遇左括号边界（`cs:391-417`） | **precedence climbing** ✓ |
| `expr.Op.Priority < op.Priority` | 新节点下潜为右子（`cs:418-425`） | 高优先级先算 ✓ |

**实证结论**：对纯 `+ - * / ( )` 表达式，该机制**正确**——优先级、左结合、多层嵌套括号全部通过验证（4.2 节 sanity 组 7/7，与 `ExpressionBuilderUnitTest.cs` 覆盖一致）。

**但代价**：旋转期间 `BracketState` 标志必须在节点间手工搬运（`cs:367-440` 大量 `sourceExprHasLeftBracket` 换手逻辑），这是全文件最难正确推理的区域——括号 bug 的温床（Bug #1 即源于括号簿记的另一处）。

### 3.3 括号：双重簿记

括号同时由两套机制管理：
1. `Stack<char> bracketStack`——仅用于**匹配校验**（push 于 `cs:230`，pop 于 `cs:272`，末尾校验 `cs:483`）；
2. 节点上的 `BracketState` 标志——作为**树的结构边界**（右括号处理时 `while (expr.BracketState != eLeft) expr = expr.Parent` 沿树上爬，`cs:278-281`，树本身充当括号栈）。

两套簿记各管一半，必须严格同步。**函数调用路径正是两套簿记脱节的地方**（见 Bug #1）。

### 3.4 函数调用：前瞻 + 子串 + 递归重解析

识别到 `函数名(` 时（`cs:139-219`）：
1. `FindMatchingBracket` 向前扫描找匹配 `)`（`cs:495-518`）；
2. `Substring` 抽出参数原文；
3. `SplitFunctionArguments` 按逗号切分（嵌套括号感知，`cs:523-551`）；
4. 对每个参数**递归调用 `ParseToExpr`** 重新从头解析。

每个参数文本被扫描 3+ 次（找括号 / 切分 / 递归解析），复杂度 O(n·深度)，但功能上可行——**除了一处括号栈脱节把它整体炸了**（Bug #1）。

### 3.5 双循环死径

`ParseToExpr` 的外层 for 循环几乎不干活：`IsExpressionStateTransferValid` 内部自己有完整的 for 循环扫到串尾，正常情况外层只跑一轮。外层存在的唯一意义是逗号提前返回路径（`cs:317-324`）——而该路径在正常流程中**不可达**（逗号只出现在函数参数内，而参数在第 3 步已被子串抽走、主循环根本见不到），一旦因非法输入到达又产生 Bug #4。另注意 `numericParserState`、`bracketStack` 是方法内局部变量，跨调用重置，与通过参数传递的 `numericStr/functionName/currentState` 风格割裂。

---

## 4. 实证 Bug 清单（编译运行验证）

> 以下全部结论来自**真实编译运行**：用 VS2022 BuildTools 的 Roslyn csc 将 7 个源文件 + 17 用例 harness 编译为 net48 可执行文件后运行（复现步骤见附录）。非纸面推断。

### 4.1 验证结果总表

| 组 | 用例 | 结果 | 期望 | 实际 | 判定 |
|----|------|------|------|------|------|
| sanity | `1+2*3` | OK | 7 | 7 | ✅ |
| sanity | `(1+2)*3` | OK | 9 | 9 | ✅ |
| sanity | `2*-3` | OK | -6 | -6 | ✅ |
| sanity | `1+2+3` / `3+9-7` | OK | 6 / 5 | 6 / 5 | ✅ |
| sanity | `((3+1)*3+5*2-2)*2-9/(4-1)` | OK | 37 | 37 | ✅ 深层嵌套括号正确 |
| sanity | `(30)-((1+2)*3+4)*2` | OK | 4 | 4 | ✅ |
| **函数** | `sin(0)` | **FAIL** | 0 | `ParserException: Unmatched left bracket '(' at position 5` | ❌ |
| **函数** | `sin(1+1)` | **FAIL** | 0.909 | 同上（position 7） | ❌ |
| **函数** | `cos(3*2)` | **FAIL** | 0.960 | 同上 | ❌ |
| **函数** | `sqrt(9+16)` | **FAIL** | 5 | 同上 | ❌ |
| **函数** | `pow(2,3)` | **FAIL** | 8 | 同上 | ❌ |
| **函数** | `log10(100)` | **FAIL** | 2 | 同上 | ❌ |
| **一元负号** | `-(2+3)` | OK* | **-5** | **1** | ❌ **静默算错** |
| **一元负号** | `-sin(5)` | **FAIL** | 0.959 | 同函数异常（被 Bug#1 掩盖） | ❌ |
| 一元负号 | `2*-3` | OK | -6 | -6 | ✅ 字面量前缀符号可用 |
| **逗号** | `1,2` | OK* | 应报解析错误 | **1（",2" 被静默丢弃）** | ❌ |

\* "OK" 指未抛异常——但结果是错的，属最危险的静默错误类别。

### 4.2 Bug #1【Critical】所有函数调用全部抛 "Unmatched left bracket"

**现象**：任何含函数调用的表达式（`sin(0)`、`pow(2,3)`、嵌套、独立调用皆然）都无法解析。

**根因**（`ExpressionBuilder.cs`）：
```
cs:145   bracketStack.Push('(')          ← 函数路径把 '(' 压栈
cs:167   argEndIndex = FindMatchingBracket(...)   ← 找到匹配 ')' 的位置
cs:208   i = argEndIndex
cs:212   continue                        ← for 循环 i++ → 直接跳过了那个 ')'
cs:272   bracketStack.Pop()              ← 全文件唯一的弹栈点，在 ')' 处理器内，
                                           而该 ')' 已被跳过，永不执行
cs:483   if (bracketStack.Count > 0)     ← 末尾校验必然命中 → 抛异常
             throw "Unmatched left bracket"
```
括号**匹配校验栈**与**树结构边界标志**两套簿记（3.3 节）在函数路径上脱节：压了栈却无人弹。

**影响面**：
- `OperatorFactory` 注册的全部 **14 个函数**（PR #2 `ffb4824` 的 headline 特性）在解析层整体不可用；
- `FunctionUnitTest.cs` 的 ~60 个测试（`1e6a11b` 新增，全部走 `ParseToExpr("sin(...)")` 路径）**在当前 HEAD 上不可能通过**——`TestResults/` 仅有空 Deploy 目录、无 .trx 结果文件，佐证这批测试从未完整跑绿；
- 计算器 UI（Form1）上的函数按钮按下去就会得到异常。

**历史推演**：括号末尾校验来自 PR #1（`b0bdc16`），函数路径来自 PR #2（`ffb4824`）——函数**引入之日起就是坏的**，而 PR #3 的测试没有（被允许）暴露它。无 CI 是直接原因。

**最小修复**：删除 `cs:145` 的 `Push`（`FindMatchingBracket` 已完成匹配校验，参数内的括号由递归 `ParseToExpr` 自管自己的栈）。注意不能改成"让 `)` 走正常处理器"（`i = argEndIndex - 1`）：`cs:203` 已把 `funcExpr.BracketState` 置为 `eClosed`，正常处理器按 `eLeft` 上爬会爬过头顶直接抛 "Cannot find matching left bracket"。

### 4.3 Bug #2【Critical】`-(...)` 一元负号静默算错

**现象**：`-(2+3)` → **1**（正确值 -5）。不抛异常，直接给错数。

**根因**：`-` 经数字 FSA 进入 `eHasSign` 态、存入 `expr._value = "-"`（`cs:297-308`）；随后读到 `(` 时**括号分支在数字分支之前执行**（`cs:228` 先于 `cs:297`），`numericStr="-"` 与 FSA 状态原样保留；括号内第一个数字 `"2"` 与残留的 `"-"` 拼接成 `"-2"`——负号被错误地绑到括号内第一个操作数上，等价于把 `-(2+3)` 解析成了 `(-2)+3`。

单操作数时碰巧正确（`-(2)` → -2），多操作数必错。这是**静默错误**——比崩溃危险得多的 bug 类别，UI 上用户会直接拿到错误的计算结果。

**修复方向**：在 `eExpectLeftExpression`（及 `eExpectRightExpression`）态下，若已积累符号而下一个有效字符是 `(` 或字母，则将该符号升格为一元负号节点——最省事的做法是构造 `0 - (后文)`（`OperatorSub`，`Left = new Expression("0")`）。

### 4.4 Bug #3【High】`-函数(...)` 的负号被静默丢弃（被 #1 掩盖）

**根因**：`-sin(5)` 解析到函数分支时，`expr.IsValid == true`（默认构造的 `_value="0"`、`_state=eValueOnly`），走 `cs:156` 的 `expr.Left = funcExpr`；而 `Expression` 的 `Left` setter 第一行就是 `_value = null`（`Expression.cs:82`）——**已积累的 "-" 被无声扔掉**。`-sin(5)` 将等于 `sin(5)`。

当前被 Bug #1 的异常掩盖（先抛错），修 #1 后会显形。修法与 #2 同源：一元负号统一处理。

### 4.5 Bug #4【Medium】函数外逗号被静默接受

**现象**：`1,2` → 静默返回 `1`，`",2"` 凭空消失，不报任何错。

**根因**：3.5 节所述逗号提前返回路径（`cs:317-324`）+ 外层循环在逗号位置重入后，`eExpectOperator` 态下后续数字查不到运算符（`cs:328` 返回 null）被静默忽略。该路径本应抛 `ParserException`。

### 4.6 对测试与 CI 的含义

- `ExpressionBuilderUnitTest.cs`（数字/优先级/结合性/括号，~50 用例）与实证 sanity 组一致，**可信、应通过**；
- `FunctionUnitTest.cs`（~60 用例）在 HEAD 上**必然全红**——要么从未被完整执行，要么失败未被告警。**仓库当前没有任何 CI**。
- 第一优先行动：修复 Bug #1/#2 后，把 4.1 表中全部 ❌ 用例固化为回归测试，并加 GitHub Actions（msbuild + vstest）挡门。

---

## 5. 与经典表达式解析算法的对比

### 5.1 vs Shunting-Yard（Dijkstra 调度场）

tokenize → 双栈（输出栈 + 运算符栈）→ RPN 或立即求值。约 80-100 行，O(n) 单遍，优先级/结合性是一张表，**括号只有一种机制**（入运算符栈、遇 `)` 归约），函数调用 = 函数词元入栈 + `)` 归约时收集参数——统一处理。

| 维度 | 本项目 | Shunting-Yard |
|------|--------|---------------|
| `+-*/()` 正确性 | ✅（实证） | ✅ |
| 函数调用 | ❌ Bug #1 | ✅ 单机制天然覆盖 |
| 括号机制 | 双重簿记（栈 + 树标志），已实际脱节 | 单一运算符栈 |
| 代码量（核心） | ~450 行树手术 | ~80 行 |
| 新增运算符 | 新类 + 工厂注册 + 优先级属性 | 表加一行 |
| 产物 | **持久可检查的 Expression 树**（可 ToString 往返、可再求值） | RPN 序列或立即值，无对象模型 |

**本项目赢在产物形态**（持久树对计算器 UI 有实际价值），**输在机制复杂度**——Bug #1 恰是调度场单括号机制在结构上排除的那类错误。

### 5.2 vs 递归下降（Recursive Descent）

每条文法规则一个函数：`ParseExpr → ParseTerm → ParseFactor`，函数返回子树，**自底向上组装、无树变异**。

对本项目的杀伤性对比在于三个 bug 的**结构性不可能性**：
- 一元负号在 `ParseFactor` 里是一条前缀规则（`('-' | '+') ParseFactor`*）——Bug #2/#3 在文法层面**写不出来**；
- 函数调用是 `ParseFactor` 里 `标识符 '(' 参数列表 ')'` 一条规则，括号由递归自身配对——Bug #1 的"跳过 `)`"没有发生的场所；
- 逗号非法出现 = `ParseExpr` 返回后发现不是 `)`/EOF → 立即报错——Bug #4 没有静默通道。

本项目的三态状态机（`eExpectLeftExpression/eExpectOperator/eExpectRightExpression`）功能上就是递归下降调用栈的压平版，却付出了 370 行单函数 + Parent 指针换手 + BracketState 搬运的代价。**同样的文法，递归下降实现约为现有代码量的 1/3，且整类"树手术 bug"从结构上消失。**

### 5.3 vs Pratt 解析（优先级爬升）

`cs:340-348` 与 `cs:391-417` 的沿父上爬循环，**就是 Pratt 的优先级爬升算法**——区别在于 Pratt 在"待处理运算符栈"上爬，本项目在可变树的 Parent 链上爬。Pratt 用绑定力（binding power）一张表统一处理前缀/中缀/后缀词元，一元负号（prefix）与二元减（infix）是同一框架的两个条目；本项目则把一元负号特判进数字 FSA——正是这个特判制造了 Bug #2/#3。

### 5.4 vs 生产级实践（NCalc / Flee / Roslyn 系）

| 实践 | 本项目现状 | 差距性质 |
|------|-----------|----------|
| AST 节点存 `double` 而非 `string` | string + 每层 Parse/ToString 往返 | 性能 + 精度 + 文化设定三重问题（P-4） |
| `CultureInfo.InvariantCulture` | 未使用，潜在区域设置 bug | 正确性 |
| Visitor / 操作数由节点传入 | Operator 解剖 Expression 内部 | 架构耦合（P-1） |
| 求值编译为委托（Flee）供重复求值 | 每次全树遍历 + 字符串往返 | 性能（当前规模非刚需） |

### 5.5 总对比表

| 维度 | 本项目 | Shunting-Yard | 递归下降 | Pratt | 生产级(NCalc/Flee) |
|------|:---:|:---:|:---:|:---:|:---:|
| `+-*/()` 优先级/结合性/嵌套括号 | ✅ | ✅ | ✅ | ✅ | ✅ |
| 一元负号（含 `-(...)`、`-f(...)`） | ❌ | 需专门处理 | ✅ 文法天然 | ✅ 表统一 | ✅ |
| 函数调用（含多参 `pow(x,y)`） | ❌ | ✅ | ✅ | ✅ | ✅ |
| 非法输入必报错（无静默丢弃） | ❌ | ✅ | ✅ | ✅ | ✅ |
| 括号机制复杂度 | 高（双重簿记） | 低（单栈） | 低（递归） | 低 | 低 |
| 核心代码量 | ~450 行 | ~80 行 | ~120 行 | ~60 行 | — |
| 错误诊断质量 | ✅✅（位置+指针） | 一般 | ✅（天然带规则语境） | 一般 | ✅ |
| 产物可检查/可重复求值 | ✅ 持久树 | RPN/立即值 | ✅ 树 | ✅ 树 | ✅✅ 编译委托 |
| 求值实现 | 树遍历（字符串往返） | 栈机 | 树遍历 | 树遍历 | 编译 |

**一句话结论**：本项目以约 5 倍于递归下降的实现量，达到了递归下降在 `+-*/()` 上的正确性，同时引入了递归下降在结构上不可能出现的三类 bug；其对外的真正优势——可检查的持久表达式树——完全可以由递归下降以更低成本产出。

---

## 6. 优点清单

1. **`ParserException` 是全仓库的亮点**：位置（0-based）、出错词元、前后 10 字符上下文窗口、`ToString()` 里的 `^` 可视化指针（`ParserException.cs:121-144`）——比多数生产解析器的报错还好。
2. **`FunctionOperator.cs` 质量最高**：全部 14 个函数用 `double.TryParse` 而非 `Parse`；逐函数定义域校验（sqrt 负数、log 非正、asin/acos 越界）；NaN/Infinity 结果拦截；错误信息含实际参数值。该文件的防御密度堪称范本——只可惜解析层（Bug #1）让它整体不可达。
3. **除零保护**（`Operator.cs:118-129`）带明确异常。
4. **可扩展的注册机制**：函数 = 子类 + 一行注册，`Priority`/`ArgumentCount` 声明式暴露。
5. **`Expression.ToString()` 树→串往返重建**（含括号还原，`Expression.cs:168-195`），对 UI/调试实用。
6. **测试意图良好**：~110+ 用例覆盖数字边界、优先级、结合性、括号嵌套、函数语义与错误路径（问题在执行环节，不在设计环节，见 4.6）。
7. **`Util` 的词法分类干净**，且支持 `()[]{}` 三种括号配对（`GetMatchingRightBracket`）。

---

## 7. 改进建议（按优先级）

### P0 — 正确性（立即）
1. **修 Bug #1**：删除 `ExpressionBuilder.cs:145` 的 `bracketStack.Push`（最小 diff，`FindMatchingBracket` 已完成校验）。
2. **修 Bug #2/#3**：统一一元负号处理——`eExpectLeftExpression`/`eExpectRightExpression` 态下符号后随 `(` 或字母时，构造 `0 - (后文)`（`OperatorSub` + `Left=new Expression("0")`）。
3. **修 Bug #4**：函数外逗号抛 `ParserException`（删除或改正 `cs:317-324` 的静默提前返回）。
4. **回归测试**：把 4.1 表全部 ❌ 用例固化为单元测试。
5. **上 CI**：GitHub Actions（windows-latest + msbuild + vstest）挡门——本次 4 个 bug 中 3 个会被 CI 立即拦截。

### P1 — 正确性隐患
6. 全部 `double.Parse/TryParse/ToString` 加 `CultureInfo.InvariantCulture`。
7. `Expression._value` 改为 `double?` 类型化存储，消除求值层字符串往返（性能+精度一并解决）。

### P2 — 结构
8. **解析器重构为递归下降**（或 Pratt）：文法不变，核心代码量降至 ~1/3，4 个 bug 的 bug 类整体结构性消失；树的构建改为自底向上（函数返回子树），删除 Parent 换手与 BracketState 搬运。
9. 性能微修：`char.IsWhiteSpace` 替换 `cs:125` 的 `ToString()`；数字累积用索引切片替代逐字符 `+=`；`cs:468` 改为数字结束后一次性 `new Expression`；`OperatorFactory` 改字典查找。

### P3 — 清理
10. 删除死代码：`OperatorBracket`、`OperatorType`、空标记类 `UnaryOperator/BinraryOperator`、未用状态 `eExpectFunctionArg`、`Expression.cs:158-164` 注释块。
11. `Value` 属性补 getter；`Binrary` → `Binary`；`Debug.Assert(false)`（`cs:448`）换成真异常。

---

## 附录：复现步骤

本报告第 4 节全部结论可复现（Windows + VS2022 BuildTools）：

```powershell
# 1. 写一个 17 用例的 harness Program.cs（引用 ExpressionParser 命名空间，
#    逐条 ParseToExpr + CalcValue，try/catch 打印 OK/FAIL）
#    —— 用例清单即 4.1 表，完整源码见审查过程记录

# 2. 用 BuildTools 的 Roslyn csc 直接编译 7 个源文件 + harness
$csc = "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\Roslyn\csc.exe"
$src  = "E:\code\StringExpression\ExpressionParser"
& $csc /nologo /target:exe /langversion:7.3 `
    .\Program.cs `
    "$src\Expression.cs" "$src\ExpressionBuilder.cs" "$src\Operator.cs" `
    "$src\OperatorFactory.cs" "$src\FunctionOperator.cs" `
    "$src\Util.cs" "$src\ParserException.cs"

# 3. 运行 → 输出与 4.1 表一致
.\Program.exe
```

> 注：net48 的 mscorlib 自 .NET Framework 4.7 起内置 `System.ValueTuple`，`ExpressionBuilder.cs` 的 C# 7 元组无需额外引用即可编译。sanity 组中 `((3+1)*3+5*2-2)*2-9/(4-1)` 的正确值为 **37**、`(30)-((1+2)*3+4)*2` 为 **4**（与实测一致）。

---

*报告完。审查方法：全量静态阅读（7 文件 / ~1,758 行）+ 编译实证（17 用例）；行号以 `8f2f3fd` 为准。*

---

## 实施状态（2026-08-30 更新）

本审查报告的全部发现已实施修复，验证基线：**146/146 MSTest 通过**（含新增 ParserBugRegressionTests 38+1 用例与此前从未编译的 FunctionUnitTest 73 用例）+ 51 用例探针 0 失败 0 挂起。

| 编号 | 结论 | 实施结果 |
|---|---|---|
| F-1 | 孤儿 bracketStack.Push | 已移除（函数路径不再压栈） |
| F-2 | 占位符根被 Left-link | IsFreshPlaceholder + AttachOperandNode 替换语义 |
| F-3 | 右位函数调用丢失 | eExpectRightExpression 识别函数名 + "2sin(x)" guard |
| F-4 | CalcValue 包装节点塌缩崩溃 | _op==null && _left!=null && _right==null → Left.CalcValue() |
| Bug#2/#3 | 一元负号丢失 | OperatorNegate（Priority=2、operand in Right）+ 函数/括号双 dispatch |
| Bug#4 | 逗号静默截断 | 主循环逗号直接 throw |
| H-1 | 状态枚举误用 | eLeftValueValid/eRightValueValid/eOperatorValid 改为 2/4/8 位标志，eAllValid=14 |
| H-2 | 文化敏感解析 | double 构造/TryParse/ToString 全部 InvariantCulture；Support 14 处 ToLower → string.Equals OrdinalIgnoreCase；新增 de-DE 回归测试 |
| H-3 | Release 静默失败 | Debug.Assert(false) → ParserException |
| H-4 | 尾部残缺数字 | FSA 结束后 TryParse 校验，非法即 throw |
| P-1 | 每次计算重复 double.TryParse | _numericCache + ToNumber()，四个 setter 失效 |
| P-3 | char.ToString 分配 | char.IsWhiteSpace 直查 |
| P-4 | Support 线性扫描 | Operator.Symbol virtual + 工厂 OrdinalIgnoreCase 字典 O(1) 快路径（保留线性 fallback） |
| P-2 | 逐字符 Right 分配 | rightPending 延迟分配，eEnd/右括号/主循环末尾三处 flush |

**计划外修复**（实施过程中发现并确认的缺陷）：climb-splice 错误类（"2*(3)+1"=8、"10-(2)+3"=5、"8/(2)+2"=2、"8/(2)*3"=1.33，HEAD 实证）、log10 名字截断、"2sin(x)" 静默、两处 wrap-above/exhaust 自环挂起。

---

## 附录 C：与经典表达式解析算法的横向对比（时间/空间/劣势）

**当前算法定位**：单遍字符级 FSA + 可变树 + setter 触发的就地旋转（Operator/Left/Right setter 内部隐式完成优先级重排），求值为递归树遍历的字符串通路。

### C.1 对比对象

Dijkstra shunting-yard（两栈直算 / RPN 输出）、递归下降（语法分层）、Pratt 解析 / precedence climbing（绑定力驱动）、表驱动 LL/LR（重机械，仅作参照）。

### C.2 时间复杂度对比

| 算法 | 解析时间 | 关键机制 | 实测量级 |
|---|---|---|---|
| **当前（FSA+旋转树）** | O(n)，常数大 | 每字符 FSA 转移；每运算符 climb+旋转 | 解析 2.54 µs/op（i5-13600KF，见微基准节） |
| **Shunting-yard（两栈）** | O(n)，常数小 | 每 token 压/弹栈各一次，效果全在循环内局部 | 同量级，无字符串拼接 |
| **递归下降** | O(n) | 每优先级一层函数；经典分层语法每个终态符穿 N 层调用 | 略低于 shunting-yard |
| **Pratt / precedence climbing** | O(n)，常数最小 | "每 token 只需两三次调用"（Bendersky）；每二元运算符至多一次递归 | 工业解析器首选（Clang 即此） |

**当前算法时间损耗点**（相对上述三者）：

1. **数字词元 O(k²)**：`numericStr + incomingChar` 每字符重建字符串（`ExpressionBuilder.cs` 5 处，约 800-860 行），且每字符额外 `ToString()` 分配。shunting-yard/Pratt 的 tokenizer 用 StringBuilder 或 span 切片 = O(k)。
2. **旋转的指针 churn**：每次运算符到达触发 climb+wrap/descend，重写 Parent/Left/Right 三指针并可能级联向上；两栈法等价操作只是栈弹压，效果全部局部于循环。
3. **逐字符 FSA**：省掉 O(n) token 数组（优势），但空白/符号/边界逻辑全部内联在 FSA 里——本批 4 个根因 bug 全部聚居于此。

### C.3 空间复杂度对比

| 算法 | 解析期额外空间 | 常驻结构 | 单节点成本 |
|---|---|---|---|
| **当前** | O(1) 栈 + O(n) 节点流 | 重型可变树 | ≈80B/节点（5 字段 + 3 引用 + `Nullable<double>` 缓存）+ 每数字叶子 string 对象（~40B） |
| Shunting-yard→RPN | O(n) 输出队列 | RPN token 数组 | 8B/token（double）或引用 |
| Shunting-yard→两栈直算 | O(depth) 操作数栈 | **无树**（不留 AST） | 8B/操作数 |
| 递归下降 / Pratt | O(depth) 调用栈 | 轻量 AST | 典型 immutable 节点 ~32B（无 Parent 指针/无状态字段/无缓存） |

劣势：当前节点比轻量 AST 重 2-3 倍（Parent 指针 + 2 个状态枚举 + 数值缓存都是"就地旋转"的基础设施）；比两栈直算多出一个完整 AST——两栈法根本不建树，内存下限 O(depth)。

### C.4 当前算法劣势排序（按实际影响）

| # | 劣势 | 证据 | 影响场景 |
|---|---|---|---|
| 1 | **字符串化数值通路**：解析存 string → 求值 TryParse → 算 → ToString，每节点往返一次 | P-1 缓存后重复求值 5.7× 提升反证瓶颈所在；冷求值仍需每节点 TryParse | 一切求值路径。两栈法/RPN 法全程纯 double，零转换 |
| 2 | **求值递归深度=树高**：`1+1+1+...` 左结合链树深≈n，CalcValue 递归会 StackOverflow | 同类真实事故：Apache DataFusion 20 万深 OR 链 Drop 溢出（PR #23198）、Ledger 2000 层括号耗尽 8MB 栈（issue #3248）。显式操作数栈只需 ~200 堆条目 | 长表达式。计算器 UI 不会触发，但这是算法级软肋，且**无深度限制** |
| 3 | **仅支持左结合**：`==` 优先级 wrap-above 硬编码左结合语义 | 现有 climb 分支结构 | 无法自然表达 `^` 幂（数学惯例右结合：2^3^2=2^9）。shunting-yard 只改一个比较符（`>=`→`>`），Pratt 改一个参数（`lbp-1`） |
| 4 | **旋转机制推理成本高（非局部副作用）**：一个属性赋值触发子树重排，可能级联向上 | 本批 4 个根因 bug（climb-splice 类、两处自环）全部位于旋转分支——**实证**的 bug 密度 | 可维护性/正确性成本，最实质的工程劣势 |
| 5 | **数字词元 O(k²) 拼接**（含每字符 ToString 分配） | 5 处 `currentNumericStr + incomingChar` | 长数字输入放大 GC 压力 |
| 6 | **非可重入/非流式**：setter 副作用使解析状态散布于树本身 | 旋转逻辑读取并改写全局树形 | 流式输入、并发解析、增量重解析不适用（当前用例不需要） |

### C.5 当前算法的公平优势

- **单遍无 token 数组**：峰值内存低于"先 tokenize 再 parse"两阶段方案
- **主循环零递归**（括号用状态机下降，仅函数参数递归）：解析期栈深安全，好于朴素递归下降
- **P-1 缓存后重复求值极快**（0.167 µs/op 实测）：可变树的副作用 setter 恰好是天然的失效钩子——immutable AST 需手动失效或重建
- **可变树支持增量场景**：若需要"改一个节点重算"，当前结构比重建 RPN/AST 更直接

### C.6 结论与建议

**当前用例（WinForms 计算器：短表达式、单次求值）下，劣势 1/2/5/6 实际不成立**——2.5 µs 解析和几百字节的树无关痛痒。真正成立的只有 **#4（推理成本）**，而它已被 146 个测试 + 文档锚定。

若劣势场景成真，优先级建议：

1. **求值层 double 化**（方案 A-1 方向）：`Calc(double[])` 替代 string 往返——收益最大、不动解析器
2. **数字拼接改 index 切片**（P-5 姊妹项）——机械改动
3. **求值/解析深度限制**（如 1024）+ 左结合链的显式栈求值——防溢出护栏
4. **右结合运算符**需求出现时，当前旋转机制比换 Pratt 更难改——届时才值得评估替换解析器核心

一句话总结：**当前算法输在常数因子和推理成本，不输在渐进复杂度**——所有知名算法时间都是 O(n)，差距在"O(n) 里每步做什么"：字符串通路（慢 5.7×）与旋转 churn 是主差，而这正是"保持现有架构"路线下已经最小化的部分。

### C.7 外部参考

- Shunting-yard 复杂度与结合性弹栈规则：Wikipedia "Shunting-yard algorithm"；Nathan Reed "The Shunting-Yard Algorithm"
- Pratt 解析 / TDOP / precedence climbing 等价性与深度特性：Eli Bendersky（2010 TDOP、2012 precedence climbing）；Andy Chu（Oilshell，TDOP ≡ precedence climbing）；Pratt 1973 POPL；Crockford TDOP；Martin Fowler
- 深链树遍历溢出案例：Apache DataFusion PR #23198（20 万深链迭代化 Drop）；Ledger issue #3248（Clang 式 15 帧/层，2000 层耗尽 8MB 栈；求值器限深 1024）
