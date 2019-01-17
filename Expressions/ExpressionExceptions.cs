using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions {
    public class MissingVariableException : Exception {

        public MissingVariableException(string variableName) : base() {
            VariableName = variableName;
        }
        public MissingVariableException(string variableName,string message) : base(message) {
            VariableName = variableName;
        }

        public string VariableName { get; private set; }
    }

    public class MismatchedParenthesisException : Exception {
        public MismatchedParenthesisException() : base() { }

        public MismatchedParenthesisException(string message) : base(message) { }
    }

    public class UnknownFunctionException : Exception {

        public UnknownFunctionException(string function) : base() {
            FunctionName = function;
        }

        public UnknownFunctionException(string function, string message) : base(message) {
            FunctionName = function;
        }

        public string FunctionName { get; private set; }
    }
}
