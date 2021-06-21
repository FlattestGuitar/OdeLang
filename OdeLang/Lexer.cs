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
                    var start = iterator;
                    while (iterator < length && IsDigit(line[iterator]))
                    {
                        iterator++;
                    }
                    var end = iterator;
                    var number = line.Substring(start, end - start);
                    result.Add(Number(float.Parse(number)));
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
                    //todo throw an exception with the entire next token
                    throw new ArgumentException("Unknown character: " + character);
                }

            }
            result.Add(Newline());
            return result;
        }

        //todo use regex?
        private bool IsDigit(char character)
        {
            return digitRegex.IsMatch(character.ToString());
        }
    }
}