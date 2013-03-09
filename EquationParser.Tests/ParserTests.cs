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
                { "1+1+4*9-10*3", 8m },
                { "-5 + -4", -9m},    
                { "-4", -4m},
                { "-4 + 5 * 4", 16m},
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

                try
                {
                    Parser.ParseEquation(invalidEquation.Key);
                    Assert.Fail("Parser should have thrown an exception and didn't");
                }
                catch
                {
                }

            }

        }

    }

}
