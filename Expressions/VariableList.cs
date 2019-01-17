using Expressions.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Expressions {

    /// <summary>
    /// Object containing a list of variable values to be passed into a calculation
    /// </summary>
    public class VariableList : IEnumerable<KeyValuePair<string, Constant>> {

        private Dictionary<string, Constant> _variables;

        public VariableList() {
            _variables = new Dictionary<string, Constant>(StringComparer.OrdinalIgnoreCase);
        }

        public Constant this[string name] {
            get {
                if (_variables.ContainsKey(name)) {
                    return _variables[name];
                }
                else {
                    return null;
                }
            }
            set {
                _variables[name] = value;
            }
        }

        public void Add(string name, string value) {
            _variables[name] = Constant.Create(value);
        }

        public void Add(string name, Constant value) {
            _variables[name] = value;
        }

        public bool ContainsKey(string name) {
            return _variables.ContainsKey(name);
        }

        public long Count {
            get { return _variables.Count; }
        }

        #region Implment IEnumerable
        IEnumerator IEnumerable.GetEnumerator() {     
            return _variables.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string,Constant>> GetEnumerator() {
            return _variables.GetEnumerator();
        }

        #endregion

    }
}
