using Expressions.Functions;
using System;
using System.Collections.Generic;

namespace Expressions.Tokens {

    /// <summary>
    /// Function type operation
    /// </summary>
    public class Function : Token, ITreeNode {
        internal Function() {
            _tokenType = TokenTypes.Function;
        }

        public static Function Create(string value) {
            if (value.IsNull()) {
                return null;
            }
            else {
                if (value.StartsWith(@"\") && value.Length>1) {
                    value = value.Substring(1);
                }
                Function function = new Function();
                function.Name = value.ToNull();
                function.LeafNodes = new Token[function.ParameterCount];
                return function;
            }
        }

        private string _Name;

        public string Name {
            get { return _Name; }
            private set {
                value = value.ToNull();
                if (value.IsNull()) {
                    throw new InvalidOperationException("Function Name is required");
                }
                else {
                    value = value.ToUpper();
                    FunctionInfo result = FunctionLibrary.GetFunc(value);
                    if (result == null) {
                        throw new UnknownFunctionException(value);
                    }
                    else {
                        _Name = value;
                        _info = result;
                    }
                }
            }
        }


        private FunctionInfo _info = null;

        public int ParameterCount {
            get {
                if (_info != null) {
                    return _info.Operands;
                }
                else {
                    return 0;
                }
            }
        }


        /// <summary>
        /// Execute a function using the supplied values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public Constant Execute(Constant[] values) {
            if (_info == null) {
                _info = FunctionLibrary.GetFunc(Name);
            }
            if (_info.ReturnType == ConstantDataType.Double) {
                double result = 0;
                switch (_info.Pattern) {
                    case "D":
                        result = ((Func<double, double>)_info.Function).Invoke(values[0].ValueDouble);
                        break;
                    case "DD":
                        result = ((Func<double, double, double>)_info.Function).Invoke(values[1].ValueDouble, values[0].ValueDouble);
                        break;
                    case "DDD":
                        result = ((Func<double, double, double, double>)_info.Function).Invoke(values[2].ValueDouble, values[1].ValueDouble, values[0].ValueDouble);
                        break;
                    case "S":
                        result = result = ((Func<string, double>)_info.Function).Invoke(values[0].ValueString);
                        break;
                    case "T":
                        result = ((Func<DateTime, double>)_info.Function).Invoke(values[0].ValueDate);
                        break;
                    case "TT":
                        result = ((Func<DateTime, DateTime, double>)_info.Function).Invoke(values[1].ValueDate, values[0].ValueDate);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown function pattern");
                }
                return new Constant(result);
            }

            else if (_info.ReturnType == ConstantDataType.String) {
                string result = null;
                switch (_info.Pattern) {
                    case "S":
                        result = ((Func<string, string>)_info.Function).Invoke(values[0].ValueString);
                        break;
                    case "SS":
                        result = ((Func<string, string, string>)_info.Function).Invoke(values[1].ValueString, values[0].ValueString);
                        break;
                    case "SD":
                        result = ((Func<string, double, string>)_info.Function).Invoke(values[1].ValueString, values[0].ValueDouble);
                        break;
                    case "SDD":
                        result = ((Func<string, double, double, string>)_info.Function).Invoke(values[2].ValueString, values[1].ValueDouble, values[0].ValueDouble);
                        break;
                    case "SSS":
                        result = ((Func<string, string, string, string>)_info.Function).Invoke(values[2].ValueString, values[1].ValueString, values[0].ValueString);
                        break;
                    case "DS":
                        result = ((Func<double, string, string>)_info.Function).Invoke(values[1].ValueDouble, values[0].ValueString);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown function pattern");
                }
                return new Constant(result);
            }
            else if (_info.ReturnType == ConstantDataType.Boolean) {
                bool result;
                switch (_info.Pattern) {
                    case "B":
                        result = ((Func<bool, bool>)_info.Function).Invoke(values[0].ValueBoolean);
                        break;
                    case "BB":
                        result = ((Func<bool, bool, bool>)_info.Function).Invoke(values[1].ValueBoolean, values[0].ValueBoolean);
                        break;
                    case "BBB":
                        result = ((Func<bool, bool, bool, bool>)_info.Function).Invoke(values[2].ValueBoolean, values[1].ValueBoolean, values[0].ValueBoolean);
                        break;
                    case "C":
                        result = ((Func<Constant, bool>)_info.Function).Invoke(values[0]);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown function pattern");
                }
                return new Constant(result);
            }
            else if(_info.ReturnType== ConstantDataType.Date) {
                DateTime result = DateTime.MinValue;
                switch (_info.Pattern) {
                    case "DDD":
                        result = ((Func<double, double, double, DateTime>)_info.Function).Invoke(values[2].ValueDouble, values[1].ValueDouble, values[0].ValueDouble);
                        break;
                    case "":
                        result = ((Func<DateTime>)_info.Function).Invoke();
                        break;
                    case "TD":
                        result = ((Func<DateTime, double, DateTime>)_info.Function).Invoke(values[1].ValueDate, values[0].ValueDouble);
                        break;
                }
                return new Constant(result);
            }
            else {
                //Constant type (undefined data type)
                Constant result;
                switch (_info.Pattern) {
                    case "BCC":
                        result = ((Func<bool, Constant, Constant, Constant>)_info.Function).Invoke(values[2].ValueBoolean, values[1], values[0]);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown function pattern");
                }
                return result;
            }

        }

        public override string ToString() {
            return ToString(false);
        }

        public override string ToString(bool laTeXFormat) {
            if (laTeXFormat) {
                return @"\" + Name.ToLower();
            }
            else {
                return Name;
            }
        }

        #region Implment ITreeNode

        public Token[] LeafNodes { get; private set; }

        public Token Left {
            get { return LeafNodes[0]; }
        }

        public Token Right {
            get {
                if (ParameterCount > 1) {
                    return LeafNodes[ParameterCount - 1];
                }
                else {
                    return null;
                }
            }
        }

        public ShortCircuitType ShortCircuit {
            get {
                return _info.ShortCircuit;
            }
        }
        



        #endregion
    }
}
