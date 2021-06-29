﻿using System.Collections.Generic;

namespace OdeLang
{
    public class Function
    {
        public Function(List<string> arguments, Statement statement)
        {
            this.arguments = arguments;
            this.statement = statement;
        }

        private List<string> arguments;
        private Statement statement;

        public List<string> Arguments
        {
            get => arguments;
            set => arguments = value;
        }

        public Statement Statement
        {
            get => statement;
            set => statement = value;
        }
    }
}