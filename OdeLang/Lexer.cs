﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Token> LexicalAnalysis()
        {
            int lineNum = 0;
            var splitCode = code.Split('\n');

            List<Token> result = new List<Token>();
            
            foreach (var line in splitCode)
            {
                result.AddRange(ProcessLine(line, lineNum));
                lineNum++;
            }

            result.Add(EOF(lineNum + 1, 0));
            return new List<Token>(result);
        }

        //this looks kind of ugly, but depending on the type of token read we need full control over the iteration
        private IList<Token> ProcessLine(string line, int lineNum)
        {
            int iterator = 0;
            int length = line.Length;

            List<Token> result = new List<Token>();

            while (iterator < length && line[iterator] == ' ')
            {
                if (line[iterator] == ' ' && line[iterator + 1] == ' ')
                {
                    result.Add(Whitespace(lineNum, iterator));
                    iterator += 2;
                }
                else
                {
                    throw new ArgumentException(
                        "Illegal number of spaces at start of line. The number should be divisible by 2.");
                }
            }

            while (iterator < length)
            {
                var character = line[iterator];
                if (character == '+')
                {
                    result.Add(Plus(lineNum, iterator));
                    iterator++;
                } else if (character == '-')
                {
                    result.Add(Minus(lineNum, iterator));
                    iterator++;
                } else if (character == '*')
                {
                    result.Add(Asterisk(lineNum, iterator));
                    iterator++;
                } else if (character == '/')
                {
                    result.Add(Slash(lineNum, iterator));
                    iterator++;
                }
                else if (IsDigit(character))
                {
                    var numberString = numberAtStartOfStringRegex.Match(line.Substring(iterator)).ToString();
                    iterator += numberString.Length;
                    result.Add(Number(float.Parse(numberString), lineNum, iterator));
                } else if (character == '(')
                {
                    result.Add(OpenParenthesis(lineNum, iterator));
                    iterator++;
                } else if (character == ')')
                {
                    result.Add(CloseParenthesis(lineNum, iterator));
                    iterator++;
                }
                else
                {
                    throw new ArgumentException(String.Format("Unexpected character {0}, at {1}:{2}", character, lineNum, iterator));
                }

            }
            result.Add(Newline(lineNum, iterator));
            return result;
        }

        private bool IsDigit(char character)
        {
            return digitRegex.IsMatch(character.ToString());
        }
    }
}