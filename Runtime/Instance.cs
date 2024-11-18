
using Vowel.Errors;

namespace Vowel.Runtime
{
    internal class VowelInstance(VowelClass _vowel_class)
    {
        public VowelClass vowel_class = _vowel_class;
        private Dictionary<string, object> fields = [];

        public object Get(string lexeme)
        {
            if (fields.ContainsKey(lexeme))
            {
                return fields[lexeme];
            }

            throw new RuntimeError($"Undefined property {lexeme}");
        }

        public void Set(string lexeme, object obj)
        {
            fields.Add(lexeme, obj);
        }
        public override string ToString()
        {
            return $"{vowel_class} instance";
        }
    }
}
