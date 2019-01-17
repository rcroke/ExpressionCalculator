using System;

namespace Expressions.Tokens {
    public class Operator : Token, ITreeNode {

        /// <summary>
        /// Returns a list of valid operators, assumes single character
        /// </summary>
        public static string Operators = "+-*/()<>=≠÷!^";

        /// <summary>
        /// For two character operators, returns the possible second character
        /// </summary>
        public static string OperatorSecondChar = "=>";

        internal Operator(OperatorTypes opType) {
            _tokenType = TokenTypes.Operator;
            OperatorType = opType;
            if(opType== OperatorTypes.UnaryMinus) {
                Operands = 1;
            }
            LeafNodes = new Token[Operands];
            
        }

        public static Operator CreateUnaryMinus() {
            Operator returnValue = new Operator(OperatorTypes.UnaryMinus);
            returnValue.Precedence = 2;
            return returnValue;
        }

        /// <summary>
        /// Creates a new operator based on the passed in string value.  Returns null if operator is unkown.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Operator Create(string value) {
            Operator returnValue = null;
            switch (value.ToNull()) {
                case "+":
                    returnValue = new Operator(OperatorTypes.Addition);
                    returnValue.Precedence = 2;
                    break;
                case "*":
                case "\\times":
                    returnValue = new Operator(OperatorTypes.Multiplication);
                    returnValue.Precedence = 3;
                    break;
                case "-":
                    returnValue = new Operator(OperatorTypes.Subtraction);
                    returnValue.Precedence = 2;
                    break;
                case "/":
                case "÷":
                case "\\div":
                    returnValue = new Operator(OperatorTypes.Division);
                    returnValue.Precedence = 3;
                    break;
                case "=":
                    returnValue = new Operator(OperatorTypes.Equal);
                    returnValue.Precedence = 0;
                    break;
                case "!=":
                case "<>":
                case "≠":
                case "\\neq":
                case "\\ne":
                    returnValue = new Operator(OperatorTypes.NotEqual);
                    returnValue.Precedence = 0;
                    break;
                case "^":
                    returnValue = new Operator(OperatorTypes.Power);
                    returnValue.RightAssociative = true;
                    returnValue.Precedence = 5;
                    break;
                case "<":
                    returnValue = new Operator(OperatorTypes.LessThan);
                    returnValue.Precedence = 1;
                    break;
                case ">":
                    returnValue = new Operator(OperatorTypes.GreaterThan);
                    returnValue.Precedence = 1;
                    break;
                case "\\land":
                    returnValue = new Operator(OperatorTypes.And);
                    returnValue.Precedence = 0;
                    break;
                case "\\lor":
                    returnValue = new Operator(OperatorTypes.Or);
                    returnValue.Precedence = 0;
                    break;
            }
            if (returnValue == null && !value.IsNull()) {
                if (value.Equals("and", StringComparison.OrdinalIgnoreCase)) {
                    returnValue = new Operator(OperatorTypes.And);
                    returnValue.Precedence = 0;
                }
                else if (value.Equals("or", StringComparison.OrdinalIgnoreCase)) {
                    returnValue = new Operator(OperatorTypes.Or);
                    returnValue.Precedence = 0;
                }
            }
            return returnValue;
        }

        public enum OperatorTypes {
            Unknown,
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Equal,
            NotEqual,
            Power,
            LessThan,
            GreaterThan,
            UnaryMinus,
            And,
            Or
        }

        public OperatorTypes OperatorType { get; private set; }

        public bool RightAssociative { get; private set; } = false;

        public int Precedence { get; private set; } = 0;

        public int Operands { get; } = 2;

        public override string ToString() {
            return ToString(false);
        }

        public override string ToString(bool laTeXFormat) {
            if (OperatorType == OperatorTypes.UnaryMinus) {
                return TypeToString(false);
            }
            else {
                return string.Format(" {0} ", TypeToString(laTeXFormat));
            }
        }

        public Constant Execute(Constant op1, Constant op2) {
            if (OperatorType == OperatorTypes.Equal
                || OperatorType == OperatorTypes.NotEqual
                || OperatorType == OperatorTypes.LessThan
                || OperatorType == OperatorTypes.GreaterThan) {
                if (op1.ConstantType == ConstantDataType.Double && op2.ConstantType == ConstantDataType.Double) {
                    return new Constant(CalculateComparison(op1.ValueDouble, op2.ValueDouble));
                }
                else if (op1.ConstantType == ConstantDataType.Boolean && op2.ConstantType == ConstantDataType.Boolean) {
                    return new Constant(CalculateComparison(op1.ValueBoolean, op2.ValueBoolean));
                }
                else if (op1.ConstantType == ConstantDataType.Boolean && op2.ConstantType == ConstantDataType.Double) {
                    return new Constant(CalculateComparison(op1.ValueBoolean, op2.ValueBoolean));
                }
                else if (op1.ConstantType == ConstantDataType.Double && op2.ConstantType == ConstantDataType.Boolean) {
                    return new Constant(CalculateComparison(op1.ValueBoolean, op2.ValueBoolean));
                }
                else if(op1.ConstantType == ConstantDataType.Date && op2.ConstantType == ConstantDataType.Date) {
                    return new Constant(CalculateComparison(op1.ValueDate, op2.ValueDate));
                }
                else {
                    return new Constant(CalculateComparison(op1.ValueString, op2.ValueString));
                }

            }
            else if(OperatorType== OperatorTypes.And || OperatorType == OperatorTypes.Or) {
                if (op1.ConstantType == ConstantDataType.Boolean && op2.ConstantType == ConstantDataType.Boolean) {
                    return new Constant(CalculateLogical(op1.ValueBoolean, op2.ValueBoolean));
                }
                else {
                    throw new InvalidOperationException("Invalid operation.");
                }
            }
            else {
                if (op1.ConstantType == ConstantDataType.String || op2.ConstantType == ConstantDataType.String) {
                    return new Constant(Calculate(op1.ValueString, op2.ValueString));
                }
                else if(op1.ConstantType==ConstantDataType.Date || op2.ConstantType == ConstantDataType.Date) {
                    return new Constant(Calculate(op1.ValueString, op2.ValueString));  //Treat as strings
                }
                else {
                    return new Constant(Calculate(op1.ValueDouble, op2.ValueDouble));
                }
            }
        }

        public Constant Execute(Constant op1) {
            if (OperatorType == OperatorTypes.UnaryMinus && op1.ConstantType == ConstantDataType.Double) {
                return new Constant(op1.ValueDouble * -1);
            }
            else {
                throw new InvalidOperationException("Invalid operation.");
            }
        }

        #region Private Methods

        internal string TypeToString(bool laTeX=false) {
            string displayValue;
            switch (OperatorType) {
                case OperatorTypes.Addition:
                    displayValue = "+";
                    break;
                case OperatorTypes.Division:
                    displayValue = "/";
                    break;
                case OperatorTypes.Equal:
                    displayValue = "=";
                    break;
                case OperatorTypes.Multiplication:
                    displayValue = "*";
                    break;
                case OperatorTypes.NotEqual:
                    if (laTeX) {
                        displayValue = @"\neq";
                    }
                    else {
                        displayValue = "≠";
                    }
                    break;
                case OperatorTypes.Subtraction:
                case OperatorTypes.UnaryMinus:
                    displayValue = "-";
                    break;
                case OperatorTypes.Power:
                    displayValue = "^";
                    break;
                case OperatorTypes.LessThan:
                    displayValue = "<";
                    break;
                case OperatorTypes.GreaterThan:
                    displayValue = ">";
                    break;
                case OperatorTypes.And:
                    if (laTeX) {
                        displayValue = @"\land";
                    }
                    else {
                        displayValue = "And";
                    }
                    break;
                case OperatorTypes.Or:
                    if (laTeX) {
                        displayValue = @"\lor";
                    }
                    else {
                        displayValue = "Or";
                    }
                    break;
                default:
                    displayValue = "";
                    break;
            }
            return displayValue;
        }

        private bool CalculateComparison(string op1, string op2) {
            bool result = false;
            int index = string.Compare(op1, op2, StringComparison.CurrentCultureIgnoreCase);
            switch (OperatorType) {
                case OperatorTypes.Equal:
                    result = index == 0;
                    break;
                case OperatorTypes.NotEqual:
                    result = index != 0;
                    break;
                case OperatorTypes.LessThan:
                    result = index < 0;
                    break;
                case OperatorTypes.GreaterThan:
                    result = index > 0;
                    break;
            }
            return result;
        }

        private bool CalculateComparison(double op1, double op2) {
            bool result = false;
            switch (OperatorType) {
                case OperatorTypes.Equal:
                    result = op1 == op2;
                    break;
                case OperatorTypes.NotEqual:
                    result = op1 != op2;
                    break;
                case OperatorTypes.LessThan:
                    result = op1 < op2;
                    break;
                case OperatorTypes.GreaterThan:
                    result = op1 > op2;
                    break;
            }
            return result;
        }

        private bool CalculateComparison(bool op1, bool op2) {
            bool result = false;
            switch (OperatorType) {
                case OperatorTypes.Equal:
                    result = op1 == op2;
                    break;
                case OperatorTypes.NotEqual:
                    result = op1 != op2;
                    break;
            }
            return result;
        }

        private bool CalculateComparison(DateTime op1, DateTime op2) {
            bool result = false;
            switch (OperatorType) {
                case OperatorTypes.Equal:
                    result = op1 == op2;
                    break;
                case OperatorTypes.NotEqual:
                    result = op1 != op2;
                    break;
                case OperatorTypes.LessThan:
                    result = op1 < op2;
                    break;
                case OperatorTypes.GreaterThan:
                    result = op1 > op2;
                    break;
            }
            return result;
        }

        private bool CalculateLogical(bool op1, bool op2) {
            bool result = false;
            switch (OperatorType) {
                case OperatorTypes.And:
                    result = op1 & op2;
                    break;
                case OperatorTypes.Or:
                    result = op1 | op2;
                    break;
            }
            return result;
        }

        private double Calculate(double op1, double op2) {
            double result = 0;
            switch (OperatorType) {
                case OperatorTypes.Addition:
                    result = op1 + op2;
                    break;
                case OperatorTypes.Subtraction:
                    result = op1 - op2;
                    break;
                case OperatorTypes.Multiplication:
                    result = op1 * op2;
                    break;
                case OperatorTypes.Division:
                    result = op1 / op2;
                    break;
                case OperatorTypes.Power:
                    result = Math.Pow(op1, op2);
                    break;
            }
            return result;
        }

        private string Calculate(string op1, string op2) {
            if (OperatorType == OperatorTypes.Addition) {
                return op1 + op2;
            }
            else {
                throw new InvalidOperationException("Invalid string operation.");
            }
        }

        #endregion

        #region Implement ITreeNode

        
        public Token[] LeafNodes { get; private set; }

        public Token Left { get { return LeafNodes[0]; } }

        public Token Right {
            get {
                if (Operands == 1) {
                    return null;
                }
                else {
                    return LeafNodes[1];
                }
            }
        }

        public ShortCircuitType ShortCircuit { get { return ShortCircuitType.None; } }

        #endregion
    }
}
