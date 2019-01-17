using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions.Tokens {

    public enum ShortCircuitType {
        /// <summary>
        /// No Short circuit
        /// </summary>
        None,
        /// <summary>
        /// If type short circuit.  If first param is true, then execute second, else execute third
        /// </summary>
        IF,
        /// <summary>
        /// If first operand is True, then stop and return true.  This is an OR type operation.
        /// </summary>
        OR,
        /// <summary>
        /// If first operand is False, then stop and return false.  This is an AND type operation.
        /// </summary>
        AND
    }

    internal interface ITreeNode {

        Token[] LeafNodes { get; }

        Token Left { get; }

        Token Right { get; }

        ShortCircuitType ShortCircuit { get; }



    }
}
