using Expressions.Functions;
using Expressions.Tokens;
using System;
using Xunit;

namespace Expressions.UnitTest {

    public class FunctionInfoTests {
        [Fact]
        public void TestFunctionInfoConstruct1() {
            var result = new FunctionInfo((Func<double, double>)((a) => Math.Round(a)));

            Assert.Equal("D", result.Pattern);
            Assert.Equal(1, result.Operands);
            Assert.Equal(ConstantDataType.Double, result.ReturnType);
        }

        [Fact]
        public void TestFunctionInfoConstructor2() {
            var result = new FunctionInfo((Func<string, double, double, string>)((a, b, c) => a.Substring((int)b, (int)c)));
            Assert.Equal("SDD", result.Pattern);
            Assert.Equal(3, result.Operands);
            Assert.Equal(ConstantDataType.String, result.ReturnType);
        }

        [Fact]
        public void FunctionIFFTest1() {
            var result = Calculator.Calculate("IF(5<6,\"Hello\", \"World\")", null);
            Assert.Equal("Hello", result.ValueString);
        }

        [Fact]
        public void FunctionIFFTest2() {
            VariableList vars = new VariableList();
            vars.Add("TEST", Constant.Create("sys"));
            vars.Add("EMAIL", Constant.Create("company.org"));
            vars.Add("NAME", Constant.Create("John"));
            var result = Calculator.Calculate("IF(SUBSTRING(TEST,0,1)=\"S\",SUBSTRING(NAME,0,1)+\"@\"+EMAIL, NAME+\"@\"+EMAIL)", vars);
            Assert.Equal("J@company.org", result.ValueString);
        }

        [Fact]
        public void FunctionIFFTest3() {
            VariableList vars = new VariableList();
            vars.Add("TEST", Constant.Create("admin"));
            vars.Add("EMAIL", Constant.Create("company.org"));
            vars.Add("NAME", Constant.Create("John"));
            var result = Calculator.Calculate("IF(SUBSTRING(TEST,0,1)=\"S\",SUBSTRING(NAME,0,1)+\"@\"+EMAIL, NAME+\"@\"+EMAIL)", vars);
            Assert.Equal("John@company.org", result.ValueString);
        }

        [Fact]
        public void FunctionTest_LEFT() {
            var result = Calculator.Calculate("LEFT(\"ABCDEFG\",3)", null);
            Assert.Equal("ABC", result.ValueString);
        }

        /// <summary>
        /// String is shorter then the left index.  Return entire string
        /// </summary>
        [Fact]
        public void FunctionTest_LEFT_ShortString() {
            var result = Calculator.Calculate("LEFT(\"ABC\",5)", null);
            Assert.Equal("ABC", result.ValueString);
        }
        [Fact]
        public void FunctionTest_LEFT_Null() {
            var result = Calculator.Calculate("LEFT(Null,5)", null);
            Assert.Null(result.Value);
        }

        [Fact]
        public void FunctionTest_RIGHT() {
            var result = Calculator.Calculate("RIGHT(\"ABCDEFG\",3)", null);
            Assert.Equal("EFG", result.ValueString);
        }
        [Fact]
        public void FunctionTest_RIGHT_ShortString() {
            var result = Calculator.Calculate("RIGHT(\"ABC\",5)", null);
            Assert.Equal("ABC", result.ValueString);
        }
        [Fact]
        public void FunctionTest_RIGHT_null() {
            var result = Calculator.Calculate("RIGHT(Null,3)", null);
            Assert.Null(result.ValueString);
        }

        [Fact]
        public void FunctionTest_REPLACE() {
            var result = Calculator.Calculate("REPLACE(\"ABCDEFG\",\"DEF\",\"NEWVALUE\")", null);
            Assert.Equal("ABCNEWVALUEG", result.ValueString);
        }

        [Fact]
        public void FunctionTest_REPLACE_Null() {
            var result = Calculator.Calculate("REPLACE(Null,\"DEF\",\"NEWVALUE\")", null);
            Assert.Null(result.ValueString);
        }

        [Fact]
        public void FunctionTest_LOWER() {
            var result = Calculator.Calculate("LOWER(\"ABCdefG\")", null);
            Assert.Equal("abcdefg", result.ValueString);
        }

        [Fact]
        public void FunctionTest_LOWER_Null() {
            var result = Calculator.Calculate("LOWER(Null)", null);
            Assert.Null(result.ValueString);
        }

        [Fact]
        public void FunctionTest_UPPER() {
            var result = Calculator.Calculate("UPPER(\"aBCdefG\")", null);
            Assert.Equal("ABCDEFG", result.ValueString);
        }

        [Fact]
        public void FunctionTest_UPPER_Null() {
            var result = Calculator.Calculate("UPPER(Null)", null);
            Assert.Null(result.ValueString);
        }

        [Fact]
        public void FunctionTest_TRIM() {
            var result = Calculator.Calculate("TRIM(\"  ABC   \",)", null);
            Assert.Equal("ABC", result.ValueString);
        }

        [Fact]
        public void FunctionTest_TRIM_Null() {
            var result = Calculator.Calculate("TRIM(Null)", null);
            Assert.Null(result.Value);
        }

        [Fact]
        public void FunctionTest_SUBSTRING() {
            var result = Calculator.Calculate("SUBSTRING(\"ABCDEFG\",2,4)", null);
            Assert.Equal("CDEF", result.ValueString);
        }
        [Fact]
        public void FunctionTest_SUBSTRING_Null() {
            var result = Calculator.Calculate("SUBSTRING(null,2,4)", null);
            Assert.Null(result.ValueString);
        }

        [Fact]
        public void FunctionTest_ROUND() {
            var result = Calculator.Calculate("ROUND(5.5,0)");
            Assert.Equal(6, result.ValueDouble);
        }

        [Fact]
        public void FunctionTest_ROUND2() {
            var result = Calculator.Calculate("ROUND(5.4, 0)");
            Assert.Equal(5, result.ValueDouble);
        }

        [Fact]
        public void FunctionTest_ROUND3() {
            var result = Calculator.Calculate("ROUND(5.6666666666, 2)");
            Assert.Equal(5.67, result.ValueDouble);
        }

        [Fact]
        public void FunctionTest_MAX() {
            var result = Calculator.Calculate("MAX(6.2,8)");
            Assert.Equal(8, result.ValueDouble);
        }

        [Fact]
        public void FunctionTest_MIN() {
            var result = Calculator.Calculate("MIN(6.2,8)");
            Assert.Equal(6.2, result.ValueDouble);
        }
        [Fact]
        public void FunctionTest_ANDTrue() {
            var result = Calculator.Calculate("AND(True, True)");
            Assert.True(result.ValueBoolean);
        }
        [Fact]
        public void FunctionTest_ANDFalse() {
            var result = Calculator.Calculate("AND(True, False)");
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void FunctionTest_ORTrue() {
            var result = Calculator.Calculate("OR(True, True)");
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void FunctionTest_ORTrue2() {
            var result = Calculator.Calculate("OR(False, True)");
            Assert.True(result.ValueBoolean);
        }
        [Fact]
        public void FunctionTest_ORFalse() {
            var result = Calculator.Calculate("OR(False, False)");
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void FunctionISNull() {
            var result = Calculator.Calculate("ISNULL(NULL)");
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void FunctionISNull_CaseInsensitive() {
            var result = Calculator.Calculate("ISNULL(null)");
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void FunctionISNotNull() {
            var result = Calculator.Calculate("ISNOTNULL(null)");
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void FunctionISNotNullTrue() {
            var result = Calculator.Calculate("ISNOTNULL(\"Hello World\")");
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void FunctionISNotNullEmptyString() {
            var result = Calculator.Calculate("ISNOTNULL(\"\")");
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void FunctionISNull_EmptyString() {
            var result = Calculator.Calculate("ISNULL(\"\")");
            Assert.False(result.ValueBoolean);
        }
        [Fact]
        public void ComplexLogicTest() {
            var result = Calculator.Calculate("IF(AND(LENGTH(\"TestString\") > 4, LENGTH(\"TestString\") < 11), 5 * 5, 4 * 5)");
            Assert.Equal(25, result.ValueDouble);
        }

               

        [Fact]
        public void FunctionConcat() {
            var result = Calculator.Calculate("CONCAT(\"123\",\"456\")");
            Assert.Equal("123456", result.ValueString);
        }

        [Fact]
        public void FunctionAbsTest() {

            //TODO failing test - minus sign in the number is being tokenized as a subtration operator and not a unary minus
            var result = Calculator.Calculate("ABS(-1.5)");
            Assert.Equal(1.5, result.ValueDouble);
        }

        [Fact]
        public void FunctionFormatNumberText() {
            var result = Calculator.Calculate("FORMATNUMBER(1002,\"#,##0.00\")");
            Assert.Equal("1,002.00", result.ValueString);
        }

        [Fact]
        public void FunctionFormatNumberText2() {
            VariableList list = new VariableList();
            list.Add("input", "01");
            var result = Calculator.Calculate("FORMATNUMBER(input,\"0000\")", list);
            Assert.Equal("0001",result.ValueString);
        }
        #region Date Functions

        [Fact]
        public void FunctionYear() {
            var result = Calculator.Calculate("YEAR(DATE(2016,10,25))");
            Assert.Equal(2016d, result.ValueDouble);
        }

        [Fact]
        public void FunctionMonth() {
            var result = Calculator.Calculate("MONTH(DATE(2016,10,25))");
            Assert.Equal(10d, result.ValueDouble);
        }

        [Fact]
        public void FunctionDAY() {
            var result = Calculator.Calculate("DAY(DATE(2016,10,25))");
            Assert.Equal(25d, result.ValueDouble);
        }

        [Fact]
        public void FunctionToday() {
            var result = Calculator.Calculate("TODAY()");
            Assert.Equal(DateTime.Today, result.ValueDate);
        }

        [Fact]
        public void FunctionAddDays() {
            VariableList variables = new VariableList();
            variables.Add("startDate", "4/2/2018");
            var result = Calculator.Calculate("AddDays(startDate,30)", variables);
            Assert.Equal(new DateTime(2018, 5, 2), result.ValueDate);
        }

        [Fact]
        public void FunctionAddDays2() {
            VariableList variables = new VariableList();
            variables.Add("startDate", "4/2/2018");
            var result = Calculator.Calculate("AddDays(startDate,-170)", variables);
            Assert.Equal(new DateTime(2017, 10, 14), result.ValueDate);
        }

        [Fact]
        public void FunctionAddMonths() {
            VariableList variables = new VariableList();
            variables.Add("startDate", "4/2/2018");
            var result = Calculator.Calculate("AddMonths(startDate,2)", variables);
            Assert.Equal(new DateTime(2018, 6, 2), result.ValueDate);
        }

        [Fact]
        public void FunctionAddMonths2() {
            VariableList variables = new VariableList();
            variables.Add("startDate", "4/2/2018");
            var result = Calculator.Calculate("AddMonths(startDate,-17)", variables);
            Assert.Equal(new DateTime(2016, 11, 2), result.ValueDate);
        }

        [Fact]
        public void FunctionAddYears() {
            VariableList variables = new VariableList();
            variables.Add("startDate", "4/2/2018");
            var result = Calculator.Calculate("AddYears(startDate,5)", variables);
            Assert.Equal(new DateTime(2023, 4, 2), result.ValueDate);
        }

        [Fact]
        public void FunctionAddYears2() {
            VariableList variables = new VariableList();
            variables.Add("startDate", "4/2/2018");
            var result = Calculator.Calculate("AddYears(startDate,-20)", variables);
            Assert.Equal(new DateTime(1998, 4, 2), result.ValueDate);
        }

        [Fact]
        public void FunctionDateDiff() {
            VariableList variables = new VariableList();
            variables.Add("startDate", "4/2/2017");
            variables.Add("endDate", "6/1/2018");
            var result = Calculator.Calculate("DateDiff(startDate,endDate)", variables);
            Assert.Equal(425d, result.ValueDouble);
        }

        [Fact]
        public void FunctionIsDate() {

            VariableList variables = new VariableList();
            variables.Add("val", "");
            var result = Calculator.Calculate("IsDate(val)", variables);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void FunctionIsDate2() {

            VariableList variables = new VariableList();
            variables.Add("val", "5/15/2015");
            var result = Calculator.Calculate("IsDate(val)", variables);
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void FunctionIsDate3() {

            VariableList variables = new VariableList();
            variables.Add("val", "13/15/2015");
            var result = Calculator.Calculate("IsDate(val)", variables);
            Assert.False(result.ValueBoolean);
        }
        [Fact]
        public void FunctionIsDate4() {

            VariableList variables = new VariableList();
            variables.Add("val", new Constant(null));
            var result = Calculator.Calculate("IsDate(val)", variables);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void FunctionIsNumber() {

            VariableList variables = new VariableList();
            variables.Add("val", "");
            var result = Calculator.Calculate("IsNumber(val)", variables);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void FunctionIsNumber2() {

            VariableList variables = new VariableList();
            variables.Add("val", "25");
            var result = Calculator.Calculate("IsNumber(val)", variables);
            Assert.True(result.ValueBoolean);
        }

        [Fact]
        public void FunctionIsNumber3() {

            VariableList variables = new VariableList();
            variables.Add("val", "13.486");
            var result = Calculator.Calculate("IsNumber(val)", variables);
            Assert.True(result.ValueBoolean);
        }
        [Fact]
        public void FunctionIsNumber4() {

            VariableList variables = new VariableList();
            variables.Add("val", new Constant(null));
            var result = Calculator.Calculate("IsNumber(val)", variables);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void FunctionIsNumber5() {

            VariableList variables = new VariableList();
            variables.Add("val", new Constant("1,432.68"));
            var result = Calculator.Calculate("IsNumber(val)", variables);
            Assert.False(result.ValueBoolean);
        }

        [Fact]
        public void FunctionTest_NOT() {
            var result = Calculator.Calculate("NOT(5=4)");
            Assert.True(result.ValueBoolean);
        }
        #endregion
    }
}
