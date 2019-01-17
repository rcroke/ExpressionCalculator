using Expressions.Tokens;
using System;
using System.Text;


namespace Expressions.Functions {

    /// <summary>
    /// Contains information about a function.
    /// </summary>
    public class FunctionInfo  {

        public FunctionInfo(Delegate function) : this( function, ShortCircuitType.None, null, null) { }

        public FunctionInfo(Delegate function, string template, string description) : this(function, ShortCircuitType.None, template, description) { }

        /// <summary>
        /// Creates a new FunctionInfo object, requires a function delgate.  Determines the return type, number operands, and the operand pattern.
        /// </summary>
        /// <param name="function"></param>
        public FunctionInfo(Delegate function, ShortCircuitType shortCircuit, string template, string description) {

            if (function == null) {
                throw new ArgumentNullException(nameof(function));
            }

            Template = template;
            Description = description;
            ShortCircuit = shortCircuit;
            switch (function.Method.ReturnType.Name) {
                case nameof(System.Double):
                    ReturnType = ConstantDataType.Double;
                    break;
                case nameof(System.Boolean):
                    ReturnType = ConstantDataType.Boolean;
                    break;
                case nameof(Constant):
                    ReturnType = ConstantDataType.Undefined;
                    break;
                case nameof(System.DateTime):
                    ReturnType = ConstantDataType.Date;
                    break;
                default:
                    ReturnType = ConstantDataType.String;
                    break;
            }
            //Operands = function.Method.GetParameters().Length;

            StringBuilder pattern = new StringBuilder();
            foreach (System.Reflection.ParameterInfo parameterInfo in function.Method.GetParameters()) {
                switch (parameterInfo.ParameterType.Name) {
                    case nameof(System.Double):
                        pattern.Append("D");
                        break;
                    case nameof(System.String):
                        pattern.Append("S");
                        break;
                    case nameof(System.Boolean):
                        pattern.Append("B");
                        break;
                    case nameof(System.DateTime):
                        pattern.Append("T");
                        break;
                    case "List`1": //Generic List
                        pattern.Append("L");
                        break;
                    default:
                        pattern.Append("C");
                        break;
                }
            }
            Pattern = pattern.ToString();
            Operands = Pattern.Replace("L", "").Length; //Remove L from pattern to get number of operands.  L is a special type
            Function = function;
        }

        private string _Template = null;

        /// <summary>
        /// Template, defines how the function should be used in an expression
        /// </summary>
        public string Template {
            get { return _Template; }
            set { _Template = value.ToNull(); }
        }

        private string _Description = null;

        /// <summary>
        /// Description of the function
        /// </summary>
        public string Description {
            get { return _Description; }
            set { _Description = value.ToNull(); }
        }

        /// <summary>
        /// Short circuit behavior of the function.
        /// </summary>
        public ShortCircuitType ShortCircuit { get; private set; }

        /// <summary>
        /// Type of the function return value
        /// </summary>
        public ConstantDataType ReturnType { get; private set; }

        /// <summary>
        /// Number of operands in the function requires
        /// </summary>
        public int Operands { get; private set; }

        /// <summary>
        /// Operand pattern, such as SDD
        /// S - String
        /// D - Double
        /// B - Boolean
        /// T - Date
        /// C - Constant
        /// </summary>
        public string Pattern { get; private set; }

        /// <summary>
        /// Actual function
        /// </summary>
        public Delegate Function { get; private set; }

    
    }

}
