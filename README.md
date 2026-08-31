# StringExpression

将数学表达式字符串解析为可求值的表达式树，并计算其结果。基于 .NET Framework / C# 的轻量级表达式解析器，不依赖第三方库。

## 功能特性

### 运算符

| 类别 | 支持内容 | 优先级 |
|---|---|---|
| 四则运算 | `+` `-` `*` `/` | `*` `/` 高于 `+` `-` |
| 括号 | `(...)`，支持任意深度嵌套 | 最高 |
| 一元负号 / 正号 | `-(2+3)`、`-sin(5)`、`2*-3` | 介于乘除与函数之间 |

### 数学函数（13个）

| 类别 | 函数 |
|---|---|
| 三角 | `sin(x)` `cos(x)` `tan(x)` `asin(x)` `acos(x)` `atan(x)` |
| 开方 | `sqrt(x)` |
| 对数 | `log(x)`（自然对数）`ln(x)`（`log` 别名）`log10(x)` |
| 其他 | `abs(x)` `exp(x)` `pow(x, y)`（双参数） |

### 其他能力

- **文化无关（InvariantCulture）**：解析、计算、输出全程使用 `CultureInfo.InvariantCulture`，在 `de-DE` 等逗号小数区域设置下结果一致
- **除零保护**：`1/0` 抛出 `ParserException` 而非返回 `Infinity`
- **显式错误报告**：非法输入（括号不匹配、逗号越界、残缺数字、未知函数等）抛出带出错位置的 `ParserException`
- **重复求值缓存**：表达式树求值结果按节点缓存（树变更时自动失效），适合一次解析多次求值

## 快速开始

```csharp
using ExpressionParser;

// 解析并求值
Expression root = ExpressionBuilder.Instance.ParseToExpr("(1 + 2) * sin(3.14159 / 2)");
string result = root.CalcValue();   // "3"

// 错误处理
try
{
    var bad = ExpressionBuilder.Instance.ParseToExpr("1 / 0");
    bad.CalcValue();
}
catch (ParserException ex)
{
    Console.WriteLine(ex.Message);  // Division by zero is not allowed
}
```

## 项目结构

```
StringExpression/
├── ExpressionParser/                  # 解析器核心库
│   ├── ExpressionBuilder.cs           # 入口：单遍字符级 FSA 解析主循环
│   ├── Expression.cs                  # 表达式树节点（可变树 + 求值缓存）
│   ├── Operator.cs                    # 运算符基类与四则运算 / 一元负号
│   ├── FunctionOperator.cs            # 13 个数学函数
│   ├── OperatorFactory.cs             # 运算符工厂（字典 O(1) 查找）
│   ├── ParserException.cs             # 带出错位置的解析异常
│   └── Util.cs                        # 字符分类工具
├── ExpressionParserUnitTest/          # MSTest 单元测试
├── docs/
│   ├── expression-parser-code-review.md        # 代码审查报告（含算法横向对比附录）
│   └── expression-parser-improvement-plan.md   # 改进方案与实施记录、微基准结果
└── StringExpression/                  # WinForms 计算器示例程序
```

## 架构概览

解析器采用**单遍字符级状态机 + 可变表达式树**的设计：不引入独立 tokenizer，不使用栈式优先级表，运算符优先级通过树节点的就地旋转（wrap / descend / climb）在属性赋值时隐式完成。设计取舍、与 shunting-yard / Pratt / 递归下降等经典算法的时间空间对比，见 [docs/expression-parser-code-review.md](docs/expression-parser-code-review.md) 附录 C。

## 构建与测试

要求：Visual Studio 2022（含 MSBuild 与 MSTest）或 Build Tools。

```powershell
# 构建
msbuild ExpressionParserUnitTest\ExpressionParserUnitTest.csproj /p:Configuration=Debug

# 运行测试（146 个用例）
vstest.console.exe ExpressionParserUnitTest\bin\Debug\ExpressionParserUnitTest.dll
```

测试覆盖：

- 原有功能与函数用例（`FunctionUnitTest.cs`，73 例）
- 解析器缺陷回归套件（`ParserBugRegressionTests.cs`，39 例）：括号优先级、一元负号、climb 拼接、文化无关性（de-DE）、非法输入防护
