
using Vowel.vScanner;

namespace Vowel.Errors
{
    internal class VowelError(Token _token, string _message) : Exception
    {
        public Token token = _token;
        public string message = _message;
    }

    internal class RuntimeError(string message) : Exception
    {
        public string message = message;    
    }
}
