﻿using System.Collections.Generic;

namespace OdeLang
{
    internal class CustomFunction
    {
        internal CustomFunction(List<string> arguments, Statement statement)
        {
            this.arguments = arguments;
            this.statement = statement;
        }

        private List<string> arguments;
        private Statement statement;

        internal List<string> Arguments
        {
            get => arguments;
        }

        internal Statement Statement
        {
            get => statement;
        }
    }
}