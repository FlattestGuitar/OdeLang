using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static OdeLang.Tokens;

namespace OdeLang
{
    public class Lexer
    {
        private static readonly Regex DigitRegex = new Regex(@"\d", RegexOptions.Compiled);

        //digits, optional (period and optional digits), anything
        private static readonly Regex NumberAtStartOfStringRegex = new Regex(@"^\d+(\.\d*)?", RegexOptions.Compiled);

        private static readonly Regex LegalStartOfIdentifier = new Regex(@"[a-zA-Z_]", RegexOptions.Compiled);

        private static readonly Regex IdentifierAtStartOfStringRegex =
            new Regex(@"[a-zA-Z_]+[a-zA-Z_0-9]*", RegexOptions.Compiled);


        private string _code;

        public Lexer(string code)
        {
            this._code = code;
        }

        //analyze code and return tokens
        public List<Token> LexicalAnalysis()
        {
            int lineNum = 0;
            var splitCode = _code.Split('\n');

            List<Token> result = new List<Token>();

            foreach (var line in splitCode)
            {
                result.AddRange(ProcessLine(line, lineNum));
                lineNum++;
            }

            result.Add(Eof(lineNum + 1, 0));
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
                if (character == ' ')
                {
                    iterator++;
                }
                else if (character == '+')
                {
                    result.Add(Plus(lineNum, iterator));
                    iterator++;
                }
                else if (character == '-')
                {
                    result.Add(Minus(lineNum, iterator));
                    iterator++;
                }
                else if (character == '*')
                {
                    result.Add(Asterisk(lineNum, iterator));
                    iterator++;
                }
                else if (character == '/')
                {
                    result.Add(Slash(lineNum, iterator));
                    iterator++;
                }
                else if (character == '=')
                {
                    result.Add(Assignment(lineNum, iterator));
                    iterator++;
                }
                else if (character == '(')
                {
                    result.Add(OpenParenthesis(lineNum, iterator));
                    iterator++;
                }
                else if (character == ')')
                {
                    result.Add(CloseParenthesis(lineNum, iterator));
                    iterator++;
                }
                else if (IsDigit(character))
                {
                    var numberString = NumberAtStartOfStringRegex.Match(line.Substring(iterator)).ToString();
                    iterator += numberString.Length;
                    result.Add(Number(float.Parse(numberString), lineNum, iterator));
                }
                else if (IsLegalStartOfIdentifier((character)))
                {
                    var id = IdentifierAtStartOfStringRegex.Match(line.Substring(iterator)).ToString();
                    iterator += id.Length;
                    result.Add(Identifier(id, lineNum, iterator));
                }
                else
                {
                    throw new ArgumentException(String.Format("Unexpected character {0}, at {1}:{2}", character,
                        lineNum, iterator));
                }
            }

            result.Add(Newline(lineNum, iterator));
            return result;
        }

        private bool IsDigit(char character)
        {
            return DigitRegex.IsMatch(character.ToString());
        }

        private bool IsLegalStartOfIdentifier(char character)
        {
            return LegalStartOfIdentifier.IsMatch(character.ToString());
        }
    }
}