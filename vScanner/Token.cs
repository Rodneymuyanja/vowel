
namespace Vowel.vScanner
{
    internal class Token(TokenType _token_type, string _lexeme, object _literal,TokenLocationInfo _location_info)
    {
        public TokenType token_type = _token_type; 
        public string lexeme = _lexeme; 
        public object literal = _literal;
        public TokenLocationInfo location_info = _location_info;

        public override string ToString()
        {
            return $"{token_type} {lexeme}";
        }
    }
}
