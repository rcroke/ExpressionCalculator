namespace Expressions.Functions {


    /// <summary>
    /// Contains information about an Operator
    /// </summary>
    public class OperatorInfo {

        /// <summary>
        /// Type of Operator
        /// </summary>
        public Tokens.Operator.OperatorTypes OperatorType { get; internal set; }

        /// <summary>
        /// Operator Symbol
        /// </summary>
        public string Symbol { get; internal set; }

        /// <summary>
        /// Operator Description
        /// </summary>
        public string Description { get; internal set; }
    }
}
