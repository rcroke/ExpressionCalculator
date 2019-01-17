using Expressions.Tokens;
using System;
using System.Collections.Generic;
using Xunit;

namespace Expressions.UnitTest {

    public class ExpressionTests {
        [Fact]
        public void MathTest() {
            Constant result = Calculator.Calculate("3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3", null);          
            Assert.Equal(3.0001220703125, result.ValueDouble);
        }

        /// <summary>
        /// Ensure Order of operations stays consistent.  Most calculation engines consider the power operator to be higher precendence than unary minus.  Computer languages can vary.
        /// This equation should evaluate as 1 squared, then apply the negative. 
        /// </summary>
        [Fact]
        public void MathTest2() {
            Constant result = Calculator.Calculate("-1^2", null);
            Assert.Equal(-1, result.ValueDouble);
        }


        [Fact]
        public void MathTest3() {
            Constant result = Calculator.Calculate("(-1)^2", null);
            Assert.Equal(1, result.ValueDouble);
        }

        [Fact]
        public void MathTest4() {
            VariableList vars = new VariableList();
            vars.Add("Salary", "90101.00");

            Constant result = Calculator.Calculate("Salary/12", vars);
            Assert.Equal(7508.42, Math.Round(result.ValueDouble, 2));
        }
        [Fact]
        public void PostFixNotation() {
            PostfixTokenList postfix = Calculator.TokenPostfixFormatter(Calculator.Tokenize("3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3"));
            Assert.Equal("342 * 15 - 23 ^  ^  /  + ", postfix.ToString());
        }

        [Fact]
        public void ExpressionTreeMath() {
            PostfixTokenList postfix = Calculator.TokenPostfixFormatter(Calculator.Tokenize("3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3"));
            Token result = Calculator.ExpressionTreeFormatter(postfix.Tokens);
            Assert.Equal(Operator.OperatorTypes.Subtraction, ((Operator)((ITreeNode)((ITreeNode)((ITreeNode)result).Left).Left).Right).OperatorType);
            Assert.Equal(Operator.OperatorTypes.Addition, ((Operator)result).OperatorType);
        }

        [Fact]
        public void CalculateExpression() {
            PostfixTokenList postfix = Calculator.TokenPostfixFormatter(Calculator.Tokenize("3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3"));
            Token rootNode = Calculator.ExpressionTreeFormatter(postfix.Tokens);
            Constant result = Calculator.Solve(rootNode);
            Assert.Equal(3.0001220703125, result.ValueDouble);
        }

        [Fact]
        public void ExpressionTestWithStrings() {

            VariableList vars = new VariableList();
            vars.Add("First", Constant.Create("John"));
            vars.Add("Last", Constant.Create("Smith"));

            Constant result = Calculator.Calculate("SUBSTRING(TRIM(First), 0, 1)+ \".\" + Last +\"@somecompany.com\"", vars);

            Assert.Equal("j.smith@somecompany.com", result.ValueString, true);
        }

        [Fact]
        public void ShortCircuitTestIFTrue() {
            VariableList vars = new VariableList();
            vars.Add("firstname", Constant.Create(""));
            Constant result = Calculator.Calculate("IF(firstname=\"\",\"\",SUBSTRING(firstname,0,1))", vars);
            Assert.Equal("", result.ValueString);
        }
        [Fact]
        public void ShortCircuitTestIFFalse() {
            VariableList vars = new VariableList();
            vars.Add("firstname", Constant.Create("John"));
            Constant result = Calculator.Calculate("IF(firstname=\"\",\"\",SUBSTRING(firstname,0,1))", vars);
            Assert.Equal("J", result.ValueString);
        }

        [Fact]
        public void ShortCircuitAND() {
            Constant result = Calculator.Calculate("AND(False, SUBSTRING(\"\",1,2))");  //SUBSTRING("",1,2) will cause exception if it executes
            Assert.False(result.ValueBoolean);
        }

        
        [Fact]
        public void ShortCircuitAND2() {
            Assert.Throws<System.ArgumentOutOfRangeException>(()=> Calculator.Calculate("AND(True, SUBSTRING(\"\",1,2))"));  //SUBSTRING("",1,2) will casuse exception if it executes
        }

        /// <summary>
        /// Prove that OR is short circuited.  Since first parameter is true, second parameter should never execute. An error will occur if it does
        /// </summary>
        [Fact]
        public void ShortCircuitOR() {
            Constant result = Calculator.Calculate("OR(True, SUBSTRING(\"\",1,2))");  //SUBSTRING("",1,2) will cause exception if it executes
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void BooleanTest1() {
            Constant result = Calculator.Calculate("3 < 5", null);
            Assert.True(result.ValueBoolean);
        }


        [Fact]
        public void MissingAllVariables() {
            Assert.Throws<MissingVariableException>(() => Calculator.Calculate("x < 5", null));
        }

        
        [Fact]
        public void MissingSingleVariable() {

            VariableList vars = new VariableList();
            vars.Add("FIRST", Constant.Create("John"));

            Assert.Throws<MissingVariableException>(() => Calculator.Calculate("SUBSTRING(TRIM(First), 0, 1)+ \".\" + Last +\"@somecompany.com\"", vars));
        }

        [Fact]
        public void SingleVariableCaseInsensitive() {

            VariableList vars = new VariableList();
            vars.Add("FIRST", Constant.Create("John"));

            Constant result = Calculator.Calculate("SUBSTRING(TRIM(First), 0, 1)+ \".\" + \"name@somecompany.com\"", vars);
            Assert.Equal("J.name@somecompany.com", result.ValueString);
        }
        [Fact]
        public void GetVariablesTest() {
            List<Variable> variables = Calculator.GetVariables("SUBSTRING(TRIM(First), 0, 1)+ \".\" + Last +\"@somecompany.com\"");
            Assert.Equal(2, variables.Count);
            Assert.Equal("First", variables[0].Name);
            Assert.Equal("Last", variables[1].Name);
        }

        [Fact]
        public void ValidateTest_Invalid() {
            string result = Calculator.Validate("5+(6*round(45)");
            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateTest_Valid() {
            string result = Calculator.Validate("5+(6*round(45,0))");
            Assert.Null(result);
        }
        [Fact]
        public void RetrieveOperationInfoTest() {
            var operations = Functions.FunctionLibrary.GetOperators();
            Assert.NotNull(operations);
            Assert.True(operations.Count > 0);

        }

        [Fact]
        public void TestNotEqual() {
            Constant result = Calculator.Calculate("5<>6");
            Assert.True(result.ValueBoolean);
            result = Calculator.Calculate("5!=6");
            Assert.True(result.ValueBoolean);
            result = Calculator.Calculate("5 ≠ 6");
            Assert.True(result.ValueBoolean);
        }


        [Fact]
        public void ParseTestWithSpecialChar() {
            var result = Calculator.Calculate("REPLACE(\"864-555-1212\",\"-\",\"\")");
            Assert.Equal("8645551212",result.ValueString);
        }

        [Fact]
        public void CreateDate() {
            var result = Calculator.Calculate("DATE(2016,7,15)");
            Assert.Equal(new DateTime(2016, 7, 15), result.ValueDate);
        }

        [Fact]
        public void CreateDateFromString() {
            var result = Constant.Create("12/11/15");
            Assert.Equal(new DateTime(2015, 12, 11), result.ValueDate);
        }

        [Fact]
        public void DateCompareEqual() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/10/2018"));

            var result = Calculator.Calculate("Date(2018,1,10) = endDate", list);
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void DateCompareEqual2() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/9/2018"));
            var result = Calculator.Calculate("Date(2018,1,10) = endDate", list);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void DateCompareNotEqual() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/10/2018"));

            var result = Calculator.Calculate("Date(2018,1,10) <> endDate", list);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void DateCompareNotEqual2() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/9/2018"));
            var result = Calculator.Calculate("Date(2018,1,10) <> endDate", list);
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void DateCompareLessThanl() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/15/2018"));

            var result = Calculator.Calculate("Date(2018,1,10) < endDate", list);
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void DateCompareLessThan2() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/9/2018"));
            var result = Calculator.Calculate("Date(2018,1,10) < endDate", list);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void DateCompareGreaterThanl() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/15/2018"));

            var result = Calculator.Calculate("Date(2018,1,10) > endDate", list);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void DateCompareGreaterThan2() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/9/2018"));
            var result = Calculator.Calculate("Date(2018,1,10) > endDate", list);
            Assert.True(result.ValueBoolean);
        }
        [Fact]
        public void DateCompareGreaterThan3() {
            VariableList list = new VariableList();
            list.Add("endDate", Constant.Create("1/9/2018"));
            var result = Calculator.Calculate("Date(2018,1,7) > endDate", list);
            Assert.False(result.ValueBoolean);
        }


        #region LaTeX format
        [Fact]
        public void LatexOperators1() {
            var result = Calculator.Tokenize(@"15 \div 5 \neq 4");
            Assert.Equal("15 / 5 ≠ 4", result.ToString());
        }



        [Fact]
        public void ConvertToLaTeX() {
            var result = Calculator.Tokenize("sin(15/4)<>1");
            Assert.Equal(@"\sin(15 / 4) \neq 1", result.ToString(true));
        }

        [Fact]
        public void LatexCalc1() {
            var result = Calculator.Calculate(@"\sin(15 / 4) \neq 1");
            Assert.True(result.ValueBoolean);
        }
        #endregion

        [Fact]
        public void NumberStringTest() {

            VariableList list = new VariableList();
            list.Add("inputVar", "0001");
            var result1 = Calculator.Calculate("12345 + inputVar", list);
            var result2 = Calculator.Calculate("CONCAT(12345, inputVar)", list);
            Assert.Equal(12346, result1.ValueDouble);
            Assert.Equal("123450001", result2.ValueString);
        }

        [Fact]
        public void SimpleAnd() {
            var result = Calculator.Calculate("True and false");
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void SimpleOr() {
            var result = Calculator.Calculate("True Or false");
            Assert.True(result.ValueBoolean);
        }

        [Fact] void LogicTest() {
            VariableList list = new VariableList();
            list.Add("x", "6");
            var result = Calculator.Calculate("(x>5 and x<10) or (x+10<0 or x+5>100)", list);
            Assert.True(result.ValueBoolean);
        }
    }
}
