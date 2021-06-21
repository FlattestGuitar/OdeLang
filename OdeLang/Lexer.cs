using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using static OdeLang.Tokens;

namespace OdeLang
{
    public class Lexer
    {
        private static readonly Regex digitRegex = new Regex(@"\d", RegexOptions.Compiled);
        
        //digits, optional (period and optional digits), anything
        private static readonly Regex numberAtStartOfStringRegex = new Regex(@"^\d+(\.\d*)?", RegexOptions.Compiled);


        private string code;

        public Lexer(string code)
        {
            this.code = code;
        }

        //analyze code and return tokens
        public IList<Token> LexicalAnalysis()
        {
            var result = code.Split("\n")
                .Select(ProcessLine)
                .Aggregate((a, b) => a.Concat(b).ToList());
            
            result.Add(EOF());
            return result;
        }

        //this looks kind of ugly, but depending on the type of token read we need full control over the iteration
        private IList<Token> ProcessLine(string line)
        {
            int iterator = 0;
            int length = line.Length;

            List<Token> result = new List<Token>();

            while (iterator < length)
            {
                var character = line[iterator];
                if (character == '+')
                {
                    result.Add(Plus());
                    iterator++;
                } else if (character == '-')
                {
                    result.Add(Minus());
                    iterator++;
                } else if (IsDigit(character))
                {
                    var numberString = numberAtStartOfStringRegex.Match(line.Substring(iterator)).ToString();
                    iterator += numberString.Length;
                    result.Add(Number(float.Parse(numberString)));
                } else if (character == '(')
                {
                    result.Add(OpenParenthesis());
                    iterator++;
                } else if (character == ')')
                {
                    result.Add(CloseParenthesis());
                    iterator++;
                }
                else
                {
                    //todo print line and column #
                    throw new ArgumentException("Unexpected character: " + character);
                }

            }
            result.Add(Newline());
            return result;
        }

        private bool IsDigit(char character)
        {
            return digitRegex.IsMatch(character.ToString());
        }
    }
}