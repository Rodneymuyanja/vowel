using Vowel.Interfaces;
using Vowel.Nodes;

namespace Vowel.Runtime
{
    internal class VowelClass(Stmt.ClassStatement _vowel_class) : ICallable
    {
        public Stmt.ClassStatement vowel_class = _vowel_class;
        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return new VowelInstance(this);
        }

        public override string ToString() 
        {
            return $"{vowel_class.token.lexeme}";
        }
    }
}
