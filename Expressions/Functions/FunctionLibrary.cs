using Expressions.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using static Expressions.Tokens.Operator;

namespace Expressions.Functions {

    /// <summary>
    /// Library of functions available to the Calculator
    /// </summary>
    public class FunctionLibrary {

        internal static readonly Dictionary<string, FunctionInfo> library = new Dictionary<string, Functions.FunctionInfo>();

        internal static readonly List<OperatorInfo> operatorList = new List<OperatorInfo>();

        static FunctionLibrary() {
            LoadFunctions();
            LoadOperators();
        }

        internal FunctionLibrary() { }

        /// <summary>
        /// Retrives a function from the Function Library
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FunctionInfo GetFunc(string name) {

            if (name.IsNull()) {
                throw new ArgumentNullException();
            }
            name = name.ToUpper().ToNull();
            FunctionInfo result;
            if(library.TryGetValue(name, out result)) {
                return result;
            }
            else {
                throw new ArgumentException($"Undefined function: {name}");
            }
        }

        /// <summary>
        /// Returns a list of function names
        /// </summary>
        /// <returns></returns>
        public static List<string> GetNameList() {
            return library.Keys.ToList();
        }

        /// <summary>
        /// Returns a list of valid functions
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, FunctionInfo> GetFunctions() {
            return library;
        }

        /// <summary>
        /// Returns a list of valid operators
        /// </summary>
        /// <returns></returns>
        public static List<OperatorInfo> GetOperators() {
            return operatorList;
        }

        private static void LoadOperators() {
            foreach (OperatorTypes op in Enum.GetValues(typeof(OperatorTypes))) {
                if(op == OperatorTypes.UnaryMinus || op == OperatorTypes.Unknown) {
                    continue;
                }
                OperatorInfo info = new OperatorInfo() { OperatorType = op };
                Operator opObject = new Operator(op);
                info.Symbol = opObject.TypeToString();
                info.Description = Properties.Resources.ResourceManager.GetString("op_" + op.ToString());
                operatorList.Add(info);
            }

        }
        private static void LoadFunctions() {

            //Number Functions
            library.Add("ISNUMBER", new FunctionInfo(
                (Func<Constant, bool>)((value) => value.ConstantType == ConstantDataType.Double),
                "ISNUMBER(value)",
                Properties.Resources.ISNUMBER));
            library.Add("MAX", new FunctionInfo(
                (Func<double, double, double>)((a, b) => a > b ? a : b),
                "MAX(a, b)",
                Properties.Resources.MAX));
            library.Add("MIN", new FunctionInfo(
                (Func<double, double, double>)((a, b) => a < b ? a : b),
                "MIN(a, b)",
                Properties.Resources.MIN));
            library.Add("ROUND", new FunctionInfo(
                (Func<double, double, double>)((a,b ) => Math.Round(a,(int)b)),
                "ROUND(x, d)",
                Properties.Resources.ROUND));
            library.Add("SIN", new FunctionInfo(
                (Func<double, double>)((a) => Math.Sin(a)),
                "SIN(x)",
                Properties.Resources.SIN));
            library.Add("COS", new FunctionInfo(
                (Func<double, double>)((a) => Math.Cos(a)),
                "COS(x)",
                Properties.Resources.COS));
            library.Add("SQRT", new FunctionInfo(
                (Func<double, double>)((a) => Math.Sqrt(a)),
                "SQRT(x)",
                Properties.Resources.SQRT));
            library.Add("ABS", new FunctionInfo(
                (Func<double, double>)((a) => Math.Abs(a)),
                "ABS(x)",
                Properties.Resources.ABS));


            //String Functions
            library.Add("SUBSTRING", new FunctionInfo(
                (Func<string, double, double, string>)((a, b, c) => a?.Substring((int)b, (int)c)),
                "SUBSTRING(s,x,y)",
                Properties.Resources.SUBSTRING));
            library.Add("TRIM", new FunctionInfo(
                (Func<string, string>)((a) => a?.Trim()),
                "TRIM(s)",
                Properties.Resources.TRIM));
            library.Add("LENGTH", new FunctionInfo(
                (Func<string, double>)((a) => a == null ? 0 : a.Length),
                "LENGTH(s)",
                Properties.Resources.LENGTH));
            library.Add("UPPER", new FunctionInfo(
                (Func<string, string>)((a) => a?.ToUpper()),
                "UPPER(s)",
                Properties.Resources.UPPER));
            library.Add("LOWER", new FunctionInfo(
                (Func<string, string>)((a) => a?.ToLower()),
                "LOWER(s)",
                Properties.Resources.LOWER));
            library.Add("LEFT", new FunctionInfo(
                (Func<string, double, string>)((str, len) => str?.Length > len ? str.Substring(0, (int)len) : str),
                "LEFT(s, len)",
                Properties.Resources.LEFT
                ));
            library.Add("RIGHT", new FunctionInfo(
                (Func<string, double, string>)((str, len) => str?.Length > len ? str.Substring(str.Length - (int)len, (int)len) : str),
                "RIGHT(s, len)",
                Properties.Resources.RIGHT
                ));
            library.Add("CONCAT", new FunctionInfo(
                (Func<string, string, string>)((str1, str2) => str1 + str2),
                "CONCAT(str1, str2)",
                Properties.Resources.CONCAT
                ));
            library.Add("REPLACE", new FunctionInfo(
                (Func<string, string, string, string>)((str, oldValue, newValue) => str?.Replace(oldValue, newValue)),
                "REPLACE(str, oldValue, newValue)",
                Properties.Resources.REPLACE
                ));
            library.Add("FORMATNUMBER", new FunctionInfo(
                (Func<double, string, string>)((n, format) => n.ToString(format)),
                "FORMATNUMBER(n, format)",
                Properties.Resources.FORMATNUMBER));


            //boolean
            library.Add("IF", new FunctionInfo(
                (Func<bool, Constant, Constant, Constant>)((expr, true_result, false_result) => expr ? true_result : false_result),
                ShortCircuitType.IF,
                "IF(expr, trueValue, falseValue)",
                Properties.Resources.IF));
            library.Add("AND", new FunctionInfo(
                (Func<bool, bool, bool>)((expr1, expr2) => expr1 && expr2),
                ShortCircuitType.AND,
                "AND(expr1, expr2)",
                Properties.Resources.AND));
            library.Add("OR", new FunctionInfo(
                (Func<bool, bool, bool>)((expr1, expr2) => expr1 || expr2),
                ShortCircuitType.OR,
                "OR(expr1, expr2)",
                Properties.Resources.OR));
            library.Add("NOT", new FunctionInfo(
                (Func<bool, bool>)((expr1) => !expr1),
                "NOT(expr1)", Properties.Resources.NOT));
            library.Add("ISNULL", new FunctionInfo(
                (Func<Constant, bool>)((expr) => expr.ConstantType == ConstantDataType.Null ? true : false),
                "ISNULL(expr)",
                Properties.Resources.ISNULL
                ));
            library.Add("ISNOTNULL", new FunctionInfo(
                (Func<Constant, bool>)((expr) => expr.ConstantType == ConstantDataType.Null ? false : true),
                "ISNOTNULL(expr)",
                Properties.Resources.ISNOTNULL
                ));

            //Date functions

            library.Add("ISDATE", new FunctionInfo(
                (Func<Constant, bool>)((value) => value.ConstantType == ConstantDataType.Date),
                "ISDATE(value)",
                Properties.Resources.ISDATE));
            library.Add("DATE", new FunctionInfo(
                (Func<double, double, double, DateTime>)((year, month, day) => new DateTime((int)year, (int)month, (int)day)),
                "DATE(y, m, d)",
                Properties.Resources.DATE));
            library.Add("DAY", new FunctionInfo(
                (Func<DateTime, double>)((date) => date.Day),
                "DAY(date)",
                Properties.Resources.DAY));
            library.Add("MONTH", new FunctionInfo(
                (Func<DateTime, double>)((date) => date.Month),
                "MONTH(date)",
                Properties.Resources.MONTH));
            library.Add("YEAR", new FunctionInfo(
                (Func<DateTime, double>)((date) => date.Year),
                "YEAR(date)",
                Properties.Resources.YEAR));            
                
            library.Add("TODAY", new FunctionInfo(
                (Func<DateTime>)(() => DateTime.Today),
                "TODAY()",
                Properties.Resources.TODAY));
            library.Add("ADDDAYS", new FunctionInfo(
                (Func<DateTime, double, DateTime>)((date, days) => date.AddDays((int)days)),
                "ADDDAYS(date, x)",
                Properties.Resources.ADDDAYS));
            library.Add("ADDMONTHS", new FunctionInfo(
                (Func<DateTime, double, DateTime>)((date, months) => date.AddMonths((int)months)),
                "ADDMONTHS(date, x)",
                Properties.Resources.ADDMONTHS));
            library.Add("ADDYEARS", new FunctionInfo(
                (Func<DateTime, double, DateTime>)((date, years) => date.AddYears((int)years)),
                "ADDYEARS(date, x)",
                Properties.Resources.ADDYEARS));
            library.Add("DATEDIFF", new FunctionInfo(
                (Func<DateTime, DateTime, double>)((date1, date2) => (date2-date1).Days),
                "DATEDIFF(date1, date2)",
                Properties.Resources.DATEDIFF));
        }
    }
}
