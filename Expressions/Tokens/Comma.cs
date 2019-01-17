using System;

namespace Expressions.Tokens {
    

    public class Comma : Token {

        private Comma() {
            _tokenType = TokenTypes.Comma;
        }

        public static Comma Create() {
            return new Comma();
        }

        public const string CommaChar = ",";

        public override string ToString() {
            return ", ";
        }
    }
}
