using System;

namespace Expressions.Tokens {


    /// <summary>
    /// List of Token Types (Function, operator, constant, etc)
    /// </summary>
    [Serializable]
    public enum TokenTypes {
        Constant,
        Operator,
        Function,
        Variable,
        Parentheses,
        Comma
    }
}
