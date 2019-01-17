using Expressions.Tokens;
using System.Collections.Generic;
using System.Text;

namespace Expressions {

    /// <summary>
    /// A list of expression tokens placed into Postfix format.  Tokens are rearranged from the initial expression into a format that considers order of operations.
    /// This list is used by the calculate method to perform the actual calculation.  This list CAN NOT be turned back into a string expression.
    /// This list is normally used internally, but exposed publicly for performance reasons.  If the same expression must be run mutiple times with different variables, passing
    /// this list to the calculate method will improve performance.  Using this list means that order of operations only needs to be determined once.
    /// </summary>
    public class PostfixTokenList {
        public List<Token> Tokens { get; set; }

        public override string ToString() {
            if (Tokens == null || Tokens.Count == 0) {
                return null;
            }
            else {
                StringBuilder sb = new StringBuilder();
                foreach (Token token in Tokens) {
                    sb.Append(token.ToString());
                }
                return sb.ToString();
            }
        }
    }
}
