using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EquationParser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParserTest()
        {

            Dictionary<string, decimal> validEquations = new Dictionary<string, decimal>() { 
                { "-4-(5-10)", 1m},
                { "4 + 1 + 4 + 10", 19m},
                { "5 + 4 + 1 + 4 + 10", 24m},
                { "5 + (4 + 1 + 4) + (4 + 1 + 4) + 10", 33m},
                { "2 + 2 + 2 + 2 + 2 + 2", 12m},
                { "2 + 3 + 2 + 3 + 2 + 3", 15m},
                { "1+1+4*9-10*3", 8m },
                { "-5 + -4", -9m},    
                { "-4", -4m},
                { "-4 + 5 * 4", 16m},
                { "-4 + 5 * 4 + (-4 + 5 * 4)", 32m},
                { "-4-5-10", -19m},
                { "10 - 1 + 10", 19m},
                { "5 + -4", 1m},
                { "5 + -6", -1m},
                { "2+2", 4m },
                { "2 + 2 * 3", 8m },
                { "2 + 2+2 * 3", 10m },
                { "2+2+2+2+2", 10m },
                { "2+2+2*3", 10m },
                { "(2 + 2) * 3", 12m },
                { "(2 + 2) * (3 + 2)", 20m },
                { "(2 + (8 / 4) * 3) * 3", 24m },
                { "(2 + (((((16/4)+4)/2)+(2*2)) / (2+ (4/2))) * 3) * 3", 24m }
            };

            //make sure each valid equation worked
            foreach (var validEquation in validEquations)
            {
                Assert.AreEqual(validEquation.Value, Parser.ParseEquation(validEquation.Key));
            }

            Dictionary<string, decimal> invalidEquations = new Dictionary<string, decimal>() { 
                { "2++2", 0m },
                { "2+2+", 0m },
                { "+2+2", 0m },
                { "(2+2", 0m },
                { "2+2)", 0m },
            };

            foreach (var invalidEquation in invalidEquations)
            {
    
                var result = Parser.ParseEquation(invalidEquation.Key);
                if (result != Convert.ToDecimal(int.MinValue))
                    Assert.Fail("The equation was invalid and should have returned int.minvalue");

            }

        }

        [TestMethod]
        public void MultipleParserTest()
        {

            Dictionary<string, bool> equations = new Dictionary<string, bool>() {
                { "9=5+4", true},
                { "(2+2)*3=8=3+4", false},
                { "(2+2)*3=12=4+4+4", true},
                { "-5=7-12=1*(-5)", true},
                { "-5=7-12", true}
            };

            //make sure each valid equation worked
            foreach (var equation in equations)
            {
                Assert.AreEqual(equation.Value, 
                    Parser.ParseMultipleEquations(equation.Key).Success, equation.Key);
            }

        }

    }

}
