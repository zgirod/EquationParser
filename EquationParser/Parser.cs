using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationParser
{

    public class MultipleParseResult
    {
        public bool Success { get; set; }
        public List<string> Equations { get; set; }
        public List<decimal> EquationResults { get; set; }
    }

    public static class Parser
    {

        private static char[] _firstOperators = new char[] { '*', '/' };
        private static char[] _secondOperators = new char[] { '+', '-' };
        private static List<string> _parenthesis = new List<string>() { "(", ")" };

        public static decimal ParseEquation(string equation)
        {

            try
            {
                //set the equation
                equation = equation.Replace(" ", "").Trim();

                //parse out the equation
                equation = Parser.ProcessParenthesis(equation);
                equation = Parser.ProcessMultiDivideOperators(Parser._firstOperators, equation);
                equation = Parser.ProcessAdditionDivideOperators(Parser._secondOperators, equation);

                //return the result
                return Convert.ToDecimal(equation);
            }
            catch
            {

                //if we have an error, just return int.MinValue
                return Convert.ToDecimal(int.MinValue);

            }

        }

        public static MultipleParseResult ParseMultipleEquations(string equations)
        {

            //if we don't have an equation, return false
            if (string.IsNullOrWhiteSpace(equations))
                return new MultipleParseResult() { Success = false };

            //get each equation part
            var equationParts = equations.Split(new char[] {'='}).ToList();

            //if we have only 1 equation part, return false
            if (equationParts.Count() == 1)
                return new MultipleParseResult() { Success = false };

            //for each equation part, put them together
            var equationResults = new List<decimal>();
            var returnEquations = new List<string>();
            foreach (var equationPart in equationParts)
            {
                returnEquations.Add(equationPart);
                equationResults.Add(Parser.ParseEquation(equationPart));
            }
                

            //return the result
            return new MultipleParseResult()
            {
                Success = equationResults.Distinct().Count() == 1,
                Equations = returnEquations,
                EquationResults = equationResults
            };

        }

        private static string ProcessParenthesis(string equation)
        {

            //get the index of a parenthesis
            var index = equation.LastIndexOf("(");

            //while we found our opening parenthesis
            while (index >= 0)
            {

                //get the subequation that we need to process
                var subEquation = equation.Substring(index);

                //get the ending parentheses
                var indexRight = subEquation.IndexOf(")");

                //throw and error if the closing parenthesis isn't found
                if (indexRight < 0)
                    throw new Exception();

                //get the equation we need to process
                var equationToProcess = subEquation.Substring(0, indexRight).Trim(new char[] { '(' });

                //parse the equation
                var result = Parser.ParseEquation(equationToProcess);

                //update our equation
                equation = equation.Replace("(" + equationToProcess + ")", result.ToString());

                //get the next parenthesis that we need to process
                index = equation.LastIndexOf("(");

            }

            return equation;

        }

        private static string ProcessMultiDivideOperators(char[] operators, string equation)
        {

            //find the operator
            var index = equation.IndexOfAny(operators);

            //while we found our operator
            while (index >= 0)
            {

                //get the operator
                var op = equation[index].ToString();

                //get my two number
                var leftNumber = Parser.GetNumber(index, true, equation, true);
                var rightNumber = Parser.GetNumber(index, false, equation, true);

                //get the result of the operations
                var result = Parser.ProcessOperator(leftNumber, rightNumber, op);

                //get the sub equation to insert
                var subEquationToInsert = string.Format("{0}{1}{2}", leftNumber.ToString(), op, rightNumber.ToString());

                //update the equation
                equation = ReplaceEquation(equation, subEquationToInsert, result.ToString());

                //try and see if we have that operator again
                index = equation.IndexOfAny(operators);

            }

            //return the parsed equation
            return equation;

        }

        private static string ProcessAdditionDivideOperators(char[] operators, string equation)
        {

            //find the operator
            var index = equation.IndexOfAny(operators);

            //while we found our operator
            while (index >= 0)
            {

                //if our first chara is a '-'
                var startNegative = false;
                if (index == 0 && equation[0].ToString() == "-")
                {

                    //keep going to the right, until we find a second operator
                    index++;
                    startNegative = true;
                    while (index < equation.Length && (equation[index].ToString() != "+" && equation[index].ToString() != "-"))
                        index++;

                    //if we traversed the whole equation, return
                    if (index == equation.Length)
                        return equation;

                }

                //get the operator
                var op = equation[index].ToString();

                //get my two number
                var leftNumber = Parser.GetNumber(index, true, equation, true);
                var rightNumber = Parser.GetNumber(index, false, equation, true);

                //get the result of the operations
                var result = Parser.ProcessOperator(leftNumber, rightNumber, op);

                //get the sub equation to insert
                var subEquationToInsert = string.Format("{0}{1}{2}", leftNumber.ToString(), op, rightNumber.ToString());
                if (subEquationToInsert.StartsWith("-") && startNegative == false)
                    subEquationToInsert = subEquationToInsert.Substring(1);

                //update the equation
                equation = ReplaceEquation(equation, subEquationToInsert, result.ToString());

                //try and see if we have that operator again
                index = equation.IndexOfAny(operators);

            }

            //return the parsed equation
            return equation;

        }

        private static string ReplaceEquation(string equation, string subEquation, string newResult)
        {

            var tempEquation = "";
            var index = 0;

            do
            {

                //get the index of the sub
                index = equation.IndexOf(subEquation, index);

                //if what matches is an actual match
                if (index == 0 || (index + subEquation.Length == equation.Length && index != -1) ||
                    ((index - 1 > 0 && !Char.IsNumber(equation[index - 1])) //this line makes sure the left side of the equation is not a number
                    && (!Char.IsNumber(equation[index + subEquation.Length])))) //this line makes sure the right side of the equation is not a number 
                {

                    //if we have values to the left
                    if (index > 0)
                        tempEquation = equation.Substring(0, index);
                    else
                        tempEquation = "";

                    //add in the new result
                    tempEquation += newResult;

                    //add int the rest of the equation
                    tempEquation += equation.Substring(index + subEquation.Length);

                    //set the new equation
                    equation = tempEquation;

                    //reset the start index
                    index = 0;

                }
                else if (index >= 0)
                {

                    index = index + subEquation.Length;

                }

            }
            while (index >= 0);

            //return the equation
            return equation;

        }

        private static decimal ProcessOperator(decimal leftNumber, decimal rightNumber, string op)
        {

            if (op == "+")
                return leftNumber + rightNumber;
            else if (op == "-")
                return leftNumber - rightNumber;
            else if (op == "/")
                return leftNumber / rightNumber;
            else if (op == "*")
                return leftNumber * rightNumber;
            else
                throw new Exception();

        }

        private static decimal GetNumber(int index, bool left, string equation, bool careAboutNegative)
        {

            //go back or forth one digit
            if (left)
                index--;
            else
                index++;

            //setup our number
            var number = 0;

            if (int.TryParse(equation[index].ToString(), out number)
                || (left == false && equation[index].ToString() == "-" && careAboutNegative))
            {

                int i = index;
                string value = "";

                //we are allowed to start with a negative
                if (left == false && equation[index].ToString() == "-" && careAboutNegative)
                {
                    value = "-";
                    i++;
                }

                while (int.TryParse(equation[i].ToString(), out number))
                {

                    //go back or forth one digit
                    if (left)
                    {
                        value = string.Format("{0}{1}", number.ToString(), value);
                        i--;
                    }
                    else
                    {
                        value = string.Format("{0}{1}", value, number.ToString());
                        i++;
                    }

                    if (i >= equation.Length || i < 0)
                        return Convert.ToDecimal(value);

                }

                //we need to make sure we process negative numbers
                if (left == true && equation[i].ToString() == "-" && careAboutNegative)
                {
                    value = "-" + value;
                }

                //return the number
                return Convert.ToDecimal(value);

            }
            else
            {

                throw new Exception();

            }

        }

    }
}
