using System;

namespace Expressions.Tokens {

    /// <summary>
    /// Abstract base class for all token types
    /// </summary>
    [Serializable]
    public abstract class Token {

        protected TokenTypes _tokenType = TokenTypes.Constant;

        /// <summary>
        /// Type of token
        /// </summary>
        public TokenTypes TokenType {
            get {
                return _tokenType;
            }
        }

        public virtual string ToString(bool laTeXFormat) {
            return ToString();
        }
    }
}
