using System.Collections.Generic;

namespace OdeLang
{
    public class CustomFunction
    {
        public CustomFunction(List<string> arguments, Statement statement)
        {
            this.arguments = arguments;
            this.statement = statement;
        }

        private List<string> arguments;
        private Statement statement;

        public List<string> Arguments
        {
            get => arguments;
        }

        public Statement Statement
        {
            get => statement;
        }
    }
}