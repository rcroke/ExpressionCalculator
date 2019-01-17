namespace Expressions.Tokens {
    public class Variable : Token {

        internal Variable() {
            _tokenType = TokenTypes.Variable;
        }

        public static Variable Create(string value) {
            if (value.IsNull()) {
                return null;
            }
            else {
                Variable variable = new Variable();
                variable.Name = value.ToNull();
                return variable;
            }
        }

        private string _name;

        public string Name {
            get { return _name; }
            private set { _name = value; }
        }

        public override string ToString() {
            return Name;
        }
    }
}
