using System;
using System.Collections.Generic;
using System.Linq;
using static OdeLang.Language;

namespace OdeLang
{

    public class FunctionDefinition
    {
        internal string Name { get; }
        internal List<ArgumentType> ArgumentTypes { get; }
        internal Func<List<Value>, Value> Operation { get; }

        public FunctionDefinition(string name, List<ArgumentType> argumentTypes, Func<List<Value>, object> operation)
        {
            Name = name;
            ArgumentTypes = argumentTypes;
            Operation = WrapFunctionWithTypeMapping(operation);
        }

        public FunctionDefinition(string name, List<ArgumentType> argumentTypes, Action<List<Value>> operation)
        {
            Name = name;
            ArgumentTypes = argumentTypes;
            Operation = args =>
            {
                operation.Invoke(args);
                return null;
            };
        }

        internal Value Eval(List<Value> args)
        {
            if (ArgumentTypes.Count != args.Count)
            {
                throw new ArgumentException(
                    $"Wrong number of arguments. {Name} needs exactly {ArgumentTypes.Count} arguments and received {args.Count}!");
            }

            for (int i = 0; i < args.Count; i++)
            {
                if (!ArgumentTypes[i].Predicate(args[i]))
                {
                    throw new ArgumentException(
                        $"Incorrect type of argument. {Name} requires {ArgumentTypes[i].Name}!");
                }
                
            }

            return Operation.Invoke(args);
        }

        public class ArgumentType
        {
            public Predicate<Value> Predicate { get;  }
            public string Name { get; }

            public ArgumentType(string name, Predicate<Value> predicate)
            {
                Name = name;
                Predicate = predicate;
            }
        }


        //todo make sure these exceptions match what's underneath
        public static ArgumentType AnyArgument()
        {
            return new ArgumentType(
                "any",
                _ => true
            );
        }
        public static ArgumentType StringArgument()
        {
            return SimpleTypeArgument(StringType, value => value.GetStringValue());
        }

        public static ArgumentType NumericalArgument()
        {
            return SimpleTypeArgument(NumberType, value => value.GetNumericalValue());
        }
        
        public static ArgumentType BoolArgument()
        {
            return SimpleTypeArgument(BoolType, value => value.GetBoolValue());
        }

        public static ArgumentType ObjectArgument(Type typeToMatch)
        {
            return new ArgumentType(
                typeToMatch.ToString(),
                val =>
                {
                    try
                    {
                        return val.GetObjectValue().GetType() == typeToMatch;
                    }
                    catch (ArgumentException e)
                    {
                        return false;
                    }
                });
        }


        private static ArgumentType SimpleTypeArgument(string typeName, Action<Value> extractFunc)
        {
            return new ArgumentType(
                typeName,
                val =>
                {
                    try
                    {
                        extractFunc.Invoke(val);
                        return true;
                    }
                    catch (ArgumentException e)
                    {
                        return false;
                    }
                }
            );
        }
    }
    
    
}