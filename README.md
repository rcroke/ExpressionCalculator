# Expression Calculator

This expression calculator library is designed to accept expressions or formulas entered as a string.  Expressions are parsed and solved.  Expressions can contain numbers, strings, dates, and boolean values.  A list of variable names and values can also be given when solving an expression.

## Usage:
```C#
Constant result = Calculator.Calculate(string expression, VariableList variables);
```
Order of operations is fully implemented along with a selection of functions.  Variables are optional.
Sample Expressions:

  3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3
  
  SUBSTRING(TRIM(*First*), 0, 1)+ "." + *Last* +"@someplace.mail"  
  
  
## Process:
1. Parse string into a list of tokens (operators, functions, constants, etc.)
1. Rearrange tokens into PostFix notation.  Uses a shunting yard algorithim, see: https://en.wikipedia.org/wiki/Shunting-yard_algorithm
1. If variables are included, replace variable tokens with constants.
1. Format into an expression tree.
1. Solve

## Functions
The following functions may be used in expressions:

### Math
* ISNUMBER(value)
* MAX(a, b)
* MIN(a, b)
* ROUND(x, d)
* SIN(x)
* COS(x)
* SQRT(x)
* ABS(x)

### String
* SUBSTRING(s,x,y)
* TRIM(s)
* LENGTH(s)
* UPPER(s)
* LOWER(s)
* LEFT(s, len)
* RIGHT(s, len)
* CONCAT(str1, str2)
* REPLACE(str, oldValue, newValue)
* FORMATNUMBER(n, format)

### Boolean
Always returns a true or false result.
* IF(expr, trueValue, falseValue)
* AND(expr1, expr2)
* OR(expr1, expr2)
* NOT(expr)
* ISNULL(expr)
* ISNOTNULL(expr)

### Date
* ISDATE(value)
* DATE(y, m,d)
* DAY(date)
* MONTH(date)
* YEAR(date)
* TODAY()
* ADDMONTHS(date, x)
* ADDDAYS(date, x)
* ADDYEARS(date, x)
* DATEDIFF(date1, date2)
