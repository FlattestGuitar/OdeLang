using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static OdeLang.Tokens;

namespace OdeLang
{
    public class Lexer
    {
        private static readonly Regex DigitRegex = new(@"\d", RegexOptions.Compiled);

        //digits, optional (period and optional digits), anything
        private static readonly Regex NumberAtStartOfStringRegex = new(@"^\d+(\.\d*)?", RegexOptions.Compiled);

        private static readonly Regex LegalStartOfIdentifier = new(@"[a-zA-Z_]", RegexOptions.Compiled);

        private static readonly Regex IdentifierAtStartOfStringRegex =
            new(@"[a-zA-Z_]+[a-zA-Z_0-9]*", RegexOptions.Compiled);

        private static readonly Dictionary<string, Func<int, int, Token>> literals = new()
        {
            {"+", Plus},
            {"-", Minus},
            {"*", Asterisk},
            {"/", Slash},
            {"!=", NotEqual},
            {"==", Equal}, //the order matters
            {"=", Assignment},
            {"<", LessThan},
            {">", MoreThan},
            {"(", OpenParenthesis},
            {")", CloseParenthesis},
            {"and", And},
            {"or", Or},
            {"if", If},
            {"true", (ln, col) => Boolean(true, ln, col)},
            {"false", (ln, col) => Boolean(false, ln, col)},
        
        };
        
        private string _code;

        int lineNum = 0;
        int iterator = 0;

        private bool finished = false;
        
        List<Token> result = new List<Token>();
        
        public Lexer(string code)
        {
            _code = code;
        }

        //analyze code and return tokens
        public List<Token> LexicalAnalysis()
        {
            if (finished)
            {
                throw new ApplicationException("Can't run lexical analysis twice using the same Lexer object!");
            }
            finished = true;

            string[] splitCode;
            if (_code.Contains('\r'))
            {
                splitCode = _code.Split("\r\n");
            }
            else
            {
                splitCode = _code.Split("\n");
            }

            foreach (var line in splitCode)
            {
                ProcessLine(line);
                lineNum++;
            }

            result.Add(Eof(lineNum + 1, 0));
            return new List<Token>(result);
        }

        //this looks kind of ugly, but depending on the type of token read we need full control over the iteration
        private IList<Token> ProcessLine(string line)
        {
            iterator = 0;
            int length = line.Length;

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
                bool foundLiteral = false;

                foreach (var literal in literals)
                {
                    var found = TryEatLiteral(line, literal.Key, literal.Value);
                    if (found)
                    {
                        foundLiteral = true;
                        break;
                    }
                }

                if (foundLiteral)
                {
                    continue;
                }

                char character = line[iterator];
                
                if (character == ' ')
                {
                    iterator++;
                }
                else if (character == '"')
                {
                    bool ignoreNext = false;
                    string resultString = "";

                    iterator++;

                    while (line[iterator] != '"' || ignoreNext)
                    {
                        if (line[iterator] == '\\' && !ignoreNext)
                        {
                            ignoreNext = true;
                            iterator++;
                            continue;
                        }

                        resultString += line[iterator];
                        ignoreNext = false;
                        iterator++;
                    }

                    iterator++;
                    result.Add(String(resultString, lineNum, iterator));
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
                    throw new ArgumentException($"Unexpected character {character}, at {lineNum}:{iterator}");
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

        private bool TryEatLiteral(string line, string literal, Func<int, int, Token> generator)
        {
            if (line.Substring(iterator).StartsWith(literal))
            {
                result.Add(generator.Invoke(lineNum, iterator));
                iterator += literal.Length;
                return true;
            }

            return false;
        }
    }
}