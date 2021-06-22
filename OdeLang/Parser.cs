using System;
using System.Collections.Generic;
using static OdeLang.Tokens;

namespace OdeLang
{
    //todo errors should be far more verbose and less copypasty
    public class Parser
    {
        private List<Token> tokens;
        private int i = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private TokenType currentTokenType()
        {
            return tokens[i].GetTokenType();
        }
        private Token popCurrentToken()
        {
            i++;
            return tokens[i - 1];
        }

        private Token peekNextToken()
        {
            return tokens[i + 1];
        }

        private void eatToken(TokenType type)
        {
            var token = popCurrentToken();
            if (token.GetTokenType() != type)
            {
                throw new ArgumentException("Unexpected token");
            }
        }

        private NumberStatement number()
        {
            var currentToken = popCurrentToken();
            if (currentToken.GetTokenType() != TokenType.Number)
            {
                throw new ArgumentException("Expected number here");
            }

            return new NumberStatement((float) currentToken.GetValue());
        }

        private Factor factor()
        {
            bool addition = false;
            var left = number();
            switch (popCurrentToken().GetTokenType())
            {
                case TokenType.Minus:
                    addition = false;
                    break;
                case TokenType.Plus:
                    addition = true;
                    break;
                default:
                    throw new ArgumentException("Unexpected token");
            }
            
            var right = number();
            
            return new Factor(left, right, addition);
        }
        
        public Statement Parse()
        {
            return factor();
        }
    }
}