using System;

namespace Expressions.Tokens {

    public enum ConstantDataType {
        Undefined,
        Double,
        String,
        Boolean,
        Null,
        Date
    }

    /// <summary>
    /// Defines a constant type token.  A constant is a fixed value in an expression, may be a string, number, or boolean.
    /// </summary>
    [Serializable]
    public class Constant : Token {

        internal Constant() {
            _tokenType = TokenTypes.Constant;
        }

        internal Constant(string value) {
            _tokenType = TokenTypes.Constant;
            _ConstantType = ConstantDataType.String;
            Constant temp = Constant.Create(value); //Perform datatype conversions.
            ValueString = value;
            _valueDate = temp.ValueDate;
            _valueDouble = temp.ValueDouble;
        }

        internal Constant(double value) {
            _tokenType = TokenTypes.Constant;
            _ConstantType = ConstantDataType.Double;
            _valueDouble = value;
            _valueString = value.ToString();
            _valueDate = DateTime.MinValue;
        }

        internal Constant(bool value) {
            _tokenType = TokenTypes.Constant;
            _ConstantType = ConstantDataType.Boolean;
            _valueDouble = value ? 1 : 0;
            _valueString = value.ToString();
            _valueDate = DateTime.MinValue;
        }

        internal Constant(DateTime value) {
            _tokenType = TokenTypes.Constant;
            if (value != DateTime.MinValue) {
                _ConstantType = ConstantDataType.Date;
                _valueDate = value;
                _valueString = ValueDate.ToShortDateString();
                _valueDouble = 1;
                
            }
            else {
                _ConstantType = ConstantDataType.Null;
                _valueDate = DateTime.MinValue;
                _valueString = null;
                _valueDouble = 0;
            }
        }

        public static Constant Create(string value) {

            double test = 0;
            bool testBool = false;

            Constant result = new Constant();
            if (value == null || value.Equals("Null", StringComparison.OrdinalIgnoreCase)) {
                result.ConstantType = ConstantDataType.Null;
                result._valueString = null;
                result._valueDouble = 0;
                result._valueDate = DateTime.MinValue;
            }
            else if (double.TryParse(value, out test)) {
                result.ValueDouble = test;
                result.ConstantType = ConstantDataType.Double;
                result._valueString = value; //Use unparsed string in order to retain initial string value including leading zeros.
            }
            else if (bool.TryParse(value, out testBool)) {
                result.ConstantType = ConstantDataType.Boolean;
                result.ValueDouble = testBool ? 1 : 0;
                result._valueString = testBool.ToString();
            }
            else if (DateTime.TryParse(value, out DateTime dateTimeVal)) {
                if (dateTimeVal == DateTime.MinValue) {
                    result._ConstantType = ConstantDataType.Null;
                    result._valueDate = DateTime.MinValue;
                    result._valueString = null;
                    result._valueDouble = 0;
                }
                else {
                    result.ConstantType = ConstantDataType.Date;
                    result._valueDate = dateTimeVal;
                    result._valueString = dateTimeVal.ToShortDateString();
                    result.ValueDouble = 1;
                }
            }
            else {
                result.ConstantType = ConstantDataType.String;
                result._valueString = value;
            }
            return result;
        }

        internal static Constant Create(string value, bool quotedStrings) {
            if (quotedStrings == false || value==null) {
                return Create(value);
            }
            

            //wrapped in quotes - automatically a string.
            if (value.Length >= 2 && value.StartsWith("\"") && value.EndsWith("\"")) {
                return new Constant() { ConstantType = ConstantDataType.String, ValueString = value.Substring(1, value.Length - 2) };
            }
            else {
                if (double.TryParse(value, out double test)) {
                    return Create(value);
                }
                else if (bool.TryParse(value, out bool boolVal)) {
                    return Create(value);
                }
                else if(DateTime.TryParse(value, out DateTime dateTimeVal)) {
                    return Create(value);
                }
                else if (value.Equals("Null", StringComparison.OrdinalIgnoreCase)) {
                    return Create(value);
                }
                else {
                    return null; // Must be an unquoted string, this means we found a variable
                }
            }
        }

        private ConstantDataType _ConstantType = ConstantDataType.Undefined;

        public ConstantDataType ConstantType {
            get { return _ConstantType; }
            private set { _ConstantType = value; }
        }

        private double _valueDouble = 0;
        public double ValueDouble {
            get {
                if (ConstantType == ConstantDataType.Null) {
                    return Double.MinValue;
                }
                else {
                    return _valueDouble;
                }
            }
            private set { _valueDouble = value; }
        }


        public bool ValueBoolean {
            get {
                if (ConstantType == ConstantDataType.Null) {
                    return false;
                }
                else {
                    return !(ValueDouble == 0);
                }
            }
        }

        private DateTime _valueDate = DateTime.MinValue;

        public DateTime ValueDate {
            get {
                if (ConstantType == ConstantDataType.Null) {
                    return DateTime.MinValue;
                }
                else {
                    return _valueDate;
                }
            }

        }

        private string _valueString = null;
        public string ValueString {
            get { return _valueString; }
            private set { _valueString = value; }
        }

        public object Value {
            get {
                if (ConstantType == ConstantDataType.Double) {
                    return ValueDouble;
                }
                else if(ConstantType == ConstantDataType.Boolean) {
                    return !(ValueDouble == 0);
                }
                else if(ConstantType== ConstantDataType.Date) {
                    return ValueDate;
                }
                else if(ConstantType == ConstantDataType.Null) {
                    return null;
                }
                else {
                    return ValueString;
                }
            }
        }

        public override string ToString() {
            if (ConstantType == ConstantDataType.Double) {
                return ValueDouble.ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
            else if (ConstantType == ConstantDataType.Boolean) {
                return ValueBoolean.ToString();
            }
            else if (ConstantType == ConstantDataType.String) {
                return string.Format("\"{0}\"", ValueString);
            }
            else if (ConstantType == ConstantDataType.Null) {
                return "NULL";
            }
            else if(ConstantType == ConstantDataType.Date) {
                return ValueDate.ToShortDateString();
            }
            else {
                return ValueString;
            }
        }

       
    }
}
