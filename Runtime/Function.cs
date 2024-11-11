using Vowel.Errors;
using Vowel.Interfaces;
using Vowel.Nodes;

namespace Vowel.Runtime
{
    internal class Function(Stmt.FunctionDeclaration _function_declaration) : ICallable
    {
        private Stmt.FunctionDeclaration function_declaration = _function_declaration;
        public int Arity()
        {
            return function_declaration.parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            //each function execution creates a new environment
            //on CALLING it not declaring it
            //the arity check helps us know that we have the right number of
            //thats why its not checked here
            VowelEnvironment _function_env = new(interpreter.env);
            for (int i = 0; i < arguments.Count; i++)
            {
                string parameter = function_declaration.parameters[i].lexeme;
                object argument = arguments[i];

                _function_env.Define(parameter, argument);
            }

            try
            {
                interpreter.ExecuteBlock(function_declaration.block, _function_env);

                //this is a semantic choice
                //returns are implemented are exceptions 
                //so if no return is found then we implicitly return a nil
                return Vowel.NIL;
            }
            catch (Return r)
            {
                return r.expression;
            }
        }
    }
}
