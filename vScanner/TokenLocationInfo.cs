
namespace Vowel.vScanner
{
    public class TokenLocationInfo(int _line, int _column)
    {
        public int line = _line; 
        public int column = _column;

        public override string ToString()
        {
            return $"line {line} column {column}";
        }
    }
}
