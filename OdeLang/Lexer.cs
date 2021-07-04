using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static OdeLang.Tokens;

namespace OdeLang
{
    /// <summary>
    /// This class is responsible for tokenizing the source code.
    /// Tokens have their source code positions appended to them, so any substitution operations should not take place here
    /// or they will make it impossible to provide accurate error statements at later stages of execution.
    /// </summary>
    internal class Lexer
    {
        private static readonly Regex DigitRegex = new Regex(@"\d", RegexOptions.Compiled);

        //digits, optional (period and optional digits), anything
        private static readonly Regex NumberAtStartOfStringRegex = new Regex(@"^\d+(\.\d*)?", RegexOptions.Compiled);

        private static readonly Regex LegalStartOfIdentifier = new Regex(@"[a-zA-Z_]", RegexOptions.Compiled);

        private static readonly Regex IdentifierAtStartOfStringRegex =
            new Regex(@"[a-zA-Z_]+[a-zA-Z_0-9]*", RegexOptions.Compiled);

        private static readonly Dictionary<string, Func<int, int, Token>> LiteralMatches = new Dictionary<string, Func<int, int, Token>>
        {
            {"+=", PlusAssignment},
            {"-=", MinusAssignment},
            {"*=", AsteriskAssignment},
            {"/=", SlashAssignment},
            {"%=", ModuloAssignment},
            {"++", Increment},
            {"--", Decrement},
            {"+", Plus},
            {"-", Minus},
            {"*", Asterisk},
            {"/", Slash},
            {"%", Modulo},
            {"!=", NotEqual},
            {"==", Equal}, //the order matters
            {"!", Not},
            {"=", Assignment},
            {"<", LessThan},
            {">", MoreThan},
            {"(", OpenParenthesis},
            {")", CloseParenthesis},
            {"[", OpenSquareBracket},
            {"]", ClosedSquareBracket},
            {"{", OpenCurlyBracket},
            {"}", ClosedCurlyBracket},
            {".", Period},
            {",", Comma},
            {":", Colon},
            {"return", Return},
            {"and", And},
            {"or", Or},
            {"if", If},
            {"while", While},
            {"break", Break},
            {"continue", Continue},
            {"fn", Fn},
            {"true", (ln, col) => Boolean(true, ln, col)},
            {"false", (ln, col) => Boolean(false, ln, col)},
        };

        private string _code;

        private int lineNum;
        private int columnNum;
        List<Token> result;
        
        internal Lexer(string code)
        {
            _code = code;
        }

        //analyze code and return tokens
        internal List<Token> LexicalAnalysis()
        {
            lineNum = 0;
            columnNum = 0;
            result = new List<Token>();

            var splitCode = SplitCode();

            foreach (var line in splitCode)
            {
                ProcessLine(line);
                lineNum++;
            }

            result.Add(Eof(lineNum + 1, 0));
            return new List<Token>(result);
        }

        private string[] SplitCode()
        {
            string[] splitCode;
            if (_code.Contains("\r"))
            {
                splitCode = _code.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            else
            {
                splitCode = _code.Split('\n');
            }

            return splitCode;
        }

        //this looks kind of ugly, but depending on the type of token read we need full control over the iteration
        private void ProcessLine(string line)
        {
            columnNum = 0;
            int length = line.Length;

            while (columnNum < length && line[columnNum] == ' ')
            {
                if (line[columnNum] == ' ' && line[columnNum + 1] == ' ')
                {
                    result.Add(Whitespace(lineNum, columnNum));
                    columnNum += 2;
                }
                else
                {
                    throw new ArgumentException(
                        "Illegal number of spaces at start of line. The number should be divisible by 2.");
                }
            }

            while (columnNum < length)
            {
                var foundLiteral = TryEatSimpleLiteral(line);
                if (foundLiteral)
                {
                    continue;
                }

                char character = line[columnNum];
                if (character == ' ')
                {
                    columnNum++;
                }
                else if (character == '"')
                {
                    bool ignoreNext = false;
                    string resultString = "";

                    columnNum++;

                    while (line[columnNum] != '"' || ignoreNext)
                    {
                        if (line[columnNum] == '\\' && !ignoreNext)
                        {
                            ignoreNext = true;
                            columnNum++;
                            continue;
                        }

                        resultString += line[columnNum];
                        ignoreNext = false;
                        columnNum++;
                    }

                    columnNum++;
                    result.Add(String(resultString, lineNum, columnNum));
                }
               
                else if (IsDigit(character))
                {
                    var numberString = NumberAtStartOfStringRegex.Match(line.Substring(columnNum)).ToString();
                    columnNum += numberString.Length;
                    result.Add(Number(float.Parse(numberString), lineNum, columnNum));
                }
                else if (IsLegalStartOfIdentifier((character)))
                {
                    var id = IdentifierAtStartOfStringRegex.Match(line.Substring(columnNum)).ToString();
                    columnNum += id.Length;
                    result.Add(Identifier(id, lineNum, columnNum));
                }
                else
                {
                    throw new ArgumentException($"Unexpected character {character}, at {lineNum}:{columnNum}");
                }
            }

            result.Add(Newline(lineNum, columnNum));
        }

        private bool TryEatSimpleLiteral(string line)
        {
            foreach (var literal in LiteralMatches)
            {
                if (line.Substring(columnNum).StartsWith(literal.Key))
                {
                    result.Add(literal.Value.Invoke(lineNum, columnNum));
                    columnNum += literal.Key.Length;
                    return true;
                }
            }

            return false;
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