namespace Expressions.Tokens {

    public class Parenthesis : Token {

        internal Parenthesis() {
            _tokenType = TokenTypes.Parentheses;
        }

        public enum ParenthesisType {
            Left,
            Right
        }
        public const string Parentheses = "()";

        public static Parenthesis Create(string value) {
            Parenthesis result = new Parenthesis();
            if (value == "(") {
                result.Type = ParenthesisType.Left;
            }
            else if (value == ")") {
                result.Type = ParenthesisType.Right;
            }
            else {
                result = null;
            }
            return result;
        }

        private ParenthesisType _type = ParenthesisType.Left;
        public ParenthesisType Type {
            get { return _type; }
            private set { _type = value; }
        }

        public override string ToString() {
            if (Type == ParenthesisType.Left) {
                return "(";
            }
            else {
                return ")";
            }
        }
    }
}
