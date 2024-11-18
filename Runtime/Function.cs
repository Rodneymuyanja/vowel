using Vowel.Errors;
using Vowel.Interfaces;
using Vowel.Nodes;

namespace Vowel.Runtime
{
    public class Function(Stmt.FunctionDeclaration _function_declaration) : ICallable
    {
        public readonly Stmt.FunctionDeclaration function_declaration = _function_declaration;
        public int Arity()
        {
            return function_declaration.parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            //this may look strange, taking a snapshot for each argument
            //but the language definition on VisitVarStatement
            //literally takes a new snapshot for new variable
            //parameters are variables so following the same approach
            //we create new snapshots

            //this approach comes in handy when we have a recursive function
            string function_key = $"{function_declaration.token.lexeme}${Arity()}";
            Snapshot snapshot = interpreter.snapshot_bindings[function_key];
            for (int i = 0; i < arguments.Count; i++)
            {
                string parameter = function_declaration.parameters[i].lexeme;
                object argument = arguments[i];

                snapshot.TakeSnapshot(parameter, argument);
            }

            try
            {
                interpreter.ExecuteBlock((Stmt.BlockStatement)function_declaration.block, snapshot);

                //this is a semantic choice
                //returns are implemented are exceptions 
                //so if no return is found then we implicitly return a nil
                
                return Vowel.NIL;
            }
            catch (Return r)
            {
                RestoreSnapShot(snapshot, arguments.Count);
                return r.expression;
            }
        }

        public override string ToString() 
        {
            return $"<fn {function_declaration.token.lexeme} {Arity()}>";
        }

        //when function is done executing, we can remove the argument frames
        private static void RestoreSnapShot(Snapshot snapshot, int arg_count)
        {
            for (int i = 0; i < arg_count; i++)
            {
                snapshot.PopFrame();
            }
        }
    }
}
