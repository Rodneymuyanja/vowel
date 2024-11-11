using Vowel.Runtime;

namespace Vowel.Interfaces
{
    internal interface ICallable
    {
        public object Call(Interpreter interpreter, List<object> arguments);
        public int Arity();
    }
}
