using Expressions.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions {

    /// <summary>
    /// Contains an expression parsed into tokens.  Can be turned back into an expression string by calling tostring.
    /// </summary>
    [Serializable]
    public class TokenList {

        public List<Token> Tokens { get; set; }

        public override string ToString() {
            return ToString(false);
        }

        public string ToString(bool laTeXFormat) {
            if (Tokens == null || Tokens.Count == 0) {
                return null;
            }
            else {
                StringBuilder sb = new StringBuilder();
                foreach (Token token in Tokens) {
                    sb.Append(token.ToString(laTeXFormat));
                }
                return sb.ToString();
            }
        }
    }
}
