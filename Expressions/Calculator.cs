using Expressions.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions {
    public class Calculator {

        private const char LaTeXPrefix = '\\';

        /// <summary>
        /// Returns a list of Variables from the expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static List<Variable> GetVariables(string expression) {
            if (expression.IsNull()) {
                throw new ArgumentNullException(nameof(expression));
            }
            return GetVariables(Tokenize(expression));
        }

        /// <summary>
        /// Returns a list of variables from the expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static List<Variable> GetVariables(TokenList tokens) {
            if (tokens == null) {
                throw new ArgumentNullException(nameof(tokens));
            }
            return tokens.Tokens.Where(t => t.TokenType == TokenTypes.Variable).Cast<Variable>().GroupBy(v => v.Name.ToUpper()).Select(x => x.First()).ToList();
        }



        /// <summary>
        /// Returns an error message if the expression is invalid.  Returns null if valid.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string Validate(string expression) {
            string result = null;
            try {
                TokenList tokens = Tokenize(expression);
                PostfixTokenList postfixTokens = TokenPostfixFormatter(tokens);

                //If there are no variables in the formula, solve to determine if valid.  We don't care about the result, just if an error occurs.
                List<Variable> variables = GetVariables(tokens);
                if (variables.Count == 0) {
                    Calculate(postfixTokens, null);
                }
            }
            catch(Exception ex) {
                result = ex.Message;
            }
            return result;
        }
        /// <summary>
        /// Accepts an expression as a string, entered by the user, and converts it into tokens a series of Tokens.
        /// Note:  Tokens are in the order
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TokenList Tokenize(string expression) {
            TokenList returnValue = new TokenList();
            List<Token> result = new List<Token>();
            returnValue.Tokens = result;
            if (expression.IsNull()) {
                return returnValue;
            }
            bool inQuotes = false;
            StringBuilder token = new StringBuilder();
            string tokenBreak = Operator.Operators + Parenthesis.Parentheses + Comma.CommaChar + LaTeXPrefix + " ";
            Token last = null;
            for (int i = 0; i < expression.Length; i++) {
                bool twoCharOp = false;
                char current = expression[i];
                if (current == '"') {
                    inQuotes = !inQuotes;
                }
                else if (inQuotes) {
                    token.Append(current);
                    continue;
                }
                
                if (tokenBreak.IndexOf(current) >= 0) {

                    //Begining of LaTeX operator
                    if (current == LaTeXPrefix) {
                        token.Clear();
                        token.Append(current);
                        continue;
                    }

                    //seperator found process token
                    if (token.Length > 0) {
                        Token returned = ProcessToken(token.ToString(), current);
                        if (returned != null) {
                            result.Add(returned);
                            last = returned;
                        }
                        token.Clear();
                        //else raise error for invalid expression?
                    }
                    //Process operator

                    string currentOperator = current.ToString();
                    if (currentOperator == Comma.CommaChar) {
                        last = Comma.Create();
                        result.Add(last);
                    }
                    else if (Parenthesis.Parentheses.IndexOf(currentOperator) >= 0) {
                        Parenthesis parenToken = Parenthesis.Create(currentOperator);
                        if (parenToken != null) {
                            result.Add(parenToken);
                            last = parenToken;
                        }
                    }
                    else {
                        if (Operator.Operators.IndexOf(current) >= 0) {
                            if (current == '-' && (last == null || last.TokenType == TokenTypes.Operator || (last.TokenType == TokenTypes.Parentheses &&
                                ((Parenthesis)last).Type == Parenthesis.ParenthesisType.Left) || last.TokenType == TokenTypes.Comma)) {
                                last = Operator.CreateUnaryMinus();
                                result.Add(last);
                                continue;
                            }
                            if (i <= expression.Length - 2) {
                                char next = expression[i + 1];
                                if (Operator.OperatorSecondChar.IndexOf(next) >= 0) {
                                    currentOperator += next.ToString();
                                    twoCharOp = true;
                                }
                            }
                            Operator operatorToken = Operator.Create(currentOperator);
                            if (operatorToken != null) {
                                result.Add(operatorToken);
                                last = operatorToken;
                                if (twoCharOp) {
                                    i++;  //increment i to move past the next character
                                }
                            }
                        }
                    }

                }
                else {
                    token.Append(current);
                }

            }
            if (token.Length > 0) {
                Token returned = ProcessToken(token.ToString(), ' ');
                if (returned != null) {
                    result.Add(returned);
                }
            }
            return returnValue;
        }


        /// <summary>
        /// Reorders a list of tokens into postfix notation using a shunting yard algorithm, postfix notation places operations into a sequence based on order of operations.  
        /// Postfix lists can be fed to the calculator, but may not be converted into a string expression.
        /// This function is generally only used internally.  External use would only be for perfromance purposes.  When the same expression must be calculated mutliple times with
        /// different inputs, the output of this function can be feed to all calculations.  This eliminates determing order of operations for each calculation.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static PostfixTokenList TokenPostfixFormatter(TokenList tokens) {
            if (tokens == null || tokens.Tokens == null) {
                return null;
            }
            PostfixTokenList returnValue = new PostfixTokenList();
            returnValue.Tokens = new List<Token>();
            List<Token> input = tokens.Tokens;
            Queue<Token> output = new Queue<Token>();
            if (input.Count == 0) {
                return returnValue;
            }
            Stack<Token> stack = new Stack<Token>();

            foreach (Token token in input) {
                if (token.TokenType == TokenTypes.Constant || token.TokenType == TokenTypes.Variable) {
                    output.Enqueue(token);
                }
                else if (token.TokenType == TokenTypes.Function) {
                    stack.Push(token);
                }
                else if (token.TokenType == TokenTypes.Operator) {
                    Operator op1 = (Operator)token;
                    if (stack.Count > 0) {
                        Token peek = stack.Peek();
                        if (peek.TokenType == TokenTypes.Operator) {
                            Operator op2 = (Operator)peek;
                            if (((op1.RightAssociative == false) && op1.Precedence <= op2.Precedence) || (op1.RightAssociative == true && op1.Precedence < op2.Precedence)) {
                                stack.Pop();
                                output.Enqueue(op2);
                            }
                        }
                    }
                    stack.Push(token); //push op1 onto stack;
                }
                else if (token.TokenType == TokenTypes.Parentheses) {
                    if (((Parenthesis)token).Type == Parenthesis.ParenthesisType.Left) {
                        stack.Push(token);
                    }
                    else { //Right
                        while (stack.Count >= 0) {
                            if (stack.Count == 0) {
                                throw new MismatchedParenthesisException("Mismatched parenthesis.");
                            }
                            Token current = stack.Pop();
                            if (current.TokenType == TokenTypes.Parentheses && ((Parenthesis)current).Type == Parenthesis.ParenthesisType.Left) {
                                if (stack.Count > 0) {
                                    Token top = stack.Peek();
                                    if (top.TokenType == TokenTypes.Function) {
                                        output.Enqueue(stack.Pop());
                                    }
                                }
                                break;
                            }
                            else if (current.TokenType == TokenTypes.Operator || current.TokenType == TokenTypes.Function) {
                                output.Enqueue(current);
                            }
                        }
                    }
                }
                else if (token.TokenType == TokenTypes.Comma) {
                    while (stack.Count >= 0) {
                        if (stack.Count == 0) {
                            throw new MismatchedParenthesisException("Mismatched parenthesis.");
                        }
                        Token current = stack.Peek();
                        if (current.TokenType == TokenTypes.Parentheses && ((Parenthesis)current).Type == Parenthesis.ParenthesisType.Left) {
                            break;
                        }
                        else {
                            output.Enqueue(stack.Pop());
                        }
                    }
                }
            }
            while (stack.Count > 0) {
                Token current = stack.Pop();
                if (current.TokenType == TokenTypes.Parentheses) {
                    throw new MismatchedParenthesisException("Mismatched parenthesis.");
                }
                else {
                    output.Enqueue(current);
                }
            }
            returnValue.Tokens = output.ToList();
            return returnValue;
        }


        /// <summary>
        /// Calculates the result of an expression.  Expression must not contain variables.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Constant Calculate(string expression) {
            return Calculate(expression, null);
        }

        /// <summary>
        /// Calculates the result of an expression.  Accepts optional variables.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variables">Variable names are case sensitive.</param>
        /// <returns></returns>
        public static Constant Calculate(string expression, VariableList variables) {
            TokenList tokens = Tokenize(expression);
            if (tokens != null) {
                PostfixTokenList postfix = TokenPostfixFormatter(tokens);
                if (postfix != null) {
                    return Calculate(postfix.Tokens, variables);
                }
            }
            return new Constant();
        }
        



        /// <summary>
        /// Calculates the result of an expression.  Expression  must already be converted to token format.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="variables">Variable names are case sensitive.</param>
        /// <returns></returns>
        public static Constant Calculate(TokenList tokens, VariableList variables) {
            if (tokens != null) {
                PostfixTokenList postfix = TokenPostfixFormatter(tokens);
                if (postfix != null) {
                    return Calculate(postfix.Tokens, variables);
                }
            }
            return new Constant();
        }



        /// <summary>
        /// Calculates the result of an expression.  Expression  must already be converted to postfix token format.
        /// This overload should only be used when the same expression will be calculated numerous times.  Saves the overhead
        /// of parsing the tokens and rearranging into Postfix notation for each calculation.
        /// Most call TokenPostfixFormatter() to get the postfix token list.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="variables">Variable names are case sensitive.</param>
        /// <returns></returns>
        public static Constant Calculate(PostfixTokenList tokens, VariableList variable) {
            if (tokens != null) {
                return Calculate(tokens.Tokens, variable);
            }
            return new Constant();
        }




        #region Private

        /// <summary>
        /// Converts certain string values into tokens.
        /// </summary>
        /// <param name="tokenValue"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private static Token ProcessToken(string tokenValue, char current) {
            Token returnValue = null;
            if (tokenValue.IsNull()) {
                return null;
            }
            //Check if function, always starts with a letter and ends in a open paren
            if (current == '(' && tokenValue.Length > 0 & char.IsLetter(tokenValue[0])) {
                returnValue = Function.Create(tokenValue);
            }
            if (tokenValue[0] == '\\' && tokenValue.Length>1) {
                returnValue = Operator.Create(tokenValue); //LaTeX format operator
                if (returnValue == null) {
                    returnValue = Function.Create(tokenValue);
                }
            }
            if(returnValue == null) {
                returnValue = Operator.Create(tokenValue);
            }
            if (returnValue == null) {
                returnValue = Constant.Create(tokenValue, true); //Check if constant
            }
            if (returnValue == null) {
                returnValue = Variable.Create(tokenValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Converts a post fix list of tokens into an expression tree.  The root token for the expression tree is returned.
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        internal static Token ExpressionTreeFormatter(List<Token> tokenList) {
            if (tokenList == null || tokenList.Count == 0) {
                return null;
            }
            Stack<Token> stack = new Stack<Token>();

            foreach (Token token in tokenList) {
                if (token is ITreeNode) {
                    ITreeNode node = token as ITreeNode;
                    for (int i = 0; i < node.LeafNodes.Length; i++) {
                        node.LeafNodes[i] = stack.Pop();
                    }
                    stack.Push(token);
                }
                else {
                    stack.Push(token);
                }
            }
            if (stack.Count == 1) {
                return stack.Pop();
            }
            throw new InvalidOperationException("Invalid expression");
        }


        /// <summary>
        /// Solve an equation. All variables must already be replaced with constants.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static Constant Solve(Token node) {
            if (node is ITreeNode) {
                ITreeNode treeNode = node as ITreeNode;
                Constant[] constants = new Constant[treeNode.LeafNodes.Length];

                //Operands are in reverse order in the expression tree.
                if (treeNode.ShortCircuit == ShortCircuitType.None) {
                    for (int i = treeNode.LeafNodes.Length - 1; i >= 0; i--) {

                        constants[i] = Solve(treeNode.LeafNodes[i]);
                    }
                }
                else if (treeNode.ShortCircuit == ShortCircuitType.IF) {
                    //IF will have 3 operands.  First is true, excute 2nd else 3rd (operands are in reverse order).
                    //The IF Func won't actually execute, instead IF logic is handled here.
                    Constant result = Solve(treeNode.Right);
                    if (result.ValueBoolean == true) {
                        return Solve(treeNode.LeafNodes[1]);
                    }
                    else {
                        return Solve(treeNode.Left);
                    }
                }
                else if (treeNode.ShortCircuit == ShortCircuitType.AND) {
                    constants[1] = Solve(treeNode.Right);
                    if (constants[1].ValueBoolean == true) {
                        constants[0] = Solve(treeNode.Left); //if first operand is true, then solve the next one.
                    }
                    else {
                        return constants[1]; //if first operand is false, then no need to solve the other
                    }
                }
                else if (treeNode.ShortCircuit == ShortCircuitType.OR) {
                    constants[1] = Solve(treeNode.Right);
                    if (constants[1].ValueBoolean == true) {
                        return constants[1]; //No need to solve the other operand, result is true
                    }
                    else {
                        constants[0] = Solve(treeNode.Left);
                    }
                }

                //Leaf nodes have been solved, now solve the current operation or function
                if (node.TokenType == TokenTypes.Operator) {
                    Operator op = node as Operator;
                    if (op.Operands == 2) {
                        return op.Execute(constants[1], constants[0]);
                    }
                    else {
                        return op.Execute(constants[0]);
                    }
                }
                else {
                    Function func = (Function)node;
                    return func.Execute(constants);
                }
            }
            else {
                return node as Constant;
            }
        }

        /// <summary>
        /// Performs the actual calculation.  Input token list must be in postfix notation.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="variables">Variable names are case sensitive</param>
        /// <returns></returns>
        private static Constant Calculate(List<Token> input, VariableList variables) {
            List<Token> tokens = new List<Token>(input); //create a new list
            Stack<Token> stack = new Stack<Token>();
            Token variable = input.FirstOrDefault(v => v.TokenType == TokenTypes.Variable);
            if (variable != null) {
                if (variables != null && variables.Count > 0) {
                    for (int i = 0; i < tokens.Count; i++) {
                        if (tokens[i].TokenType == TokenTypes.Variable) {
                            string name = ((Variable)input[i]).Name;
                            if (variables.ContainsKey(name)) {
                                tokens[i] = variables[name];
                            }
                            else {
                                throw new MissingVariableException(name, string.Format("Variable {0} not supplied.", name));
                            }
                        }
                    }
                }
                else {
                    throw new MissingVariableException(((Variable)variable).Name, "Expression requires variables, no variables supplied.");
                }
            }
            Token rootNode = ExpressionTreeFormatter(tokens);
            return Solve(rootNode);
            
        }


        #endregion

    }
}
