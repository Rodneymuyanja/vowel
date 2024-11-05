
using System.Text;
using Vowel.Nodes;
using Vowel.Passes;
using Vowel.Runtime;
using Vowel.vParser;
using Vowel.vScanner;

Vowel.Vowel.Main();

namespace Vowel
{
    public class Vowel
    {
        private static bool had_error = false;
        public const object NIL = null!;
        public static void Main()
        {
            ReadVowelSource();
        }

        private static void ReadVowelSource()
        {
            string path = @"C:\disk_d\Programming stuff\vowel_tests\vowel_test.vowel";
            //string path = @"D:\notes2\voel.vowel";
            byte [] byte_array = File.ReadAllBytes(path);
            string content = Encoding.UTF8.GetString(byte_array);
            var tokens = ScanSourceCode(content);

            var expressions = ParseSource(tokens);

            Interpret(expressions);
        }

        private static List<Token> ScanSourceCode(string source_code)
        {
            Scanner scanner = new (source_code);
            List<Token> tokens = scanner.ScanSourceCode();

            if (had_error)
            {
                Environment.Exit(0);
            }

            return tokens;
        }

        private static List<Stmt> ParseSource(List<Token> tokens)
        {
            Parser parser = new (tokens);
            var statements = parser.Parse();

            if (had_error)
            {
                Environment.Exit(0);
            }

            return statements;
        }

        private static void Interpret(List<Stmt> statements)
        {
            Interpreter interpreter = new ();

            Resolver resolver = new (interpreter);

            resolver.Resolve(statements);

            if (had_error)
            {
                Environment.Exit(0);
            }

            interpreter.Interpret(statements);
        }

        public static void Error(string message) 
        { 
            had_error = true;
            Report(message);
        }

        public static void Error(Token token, string message)
        {
            had_error = true;
            Report(token, message);
        }

        private static void Report(Token token,string _message) 
        {
            string message = $"Error: [Line {token.location_info.line},Column {token.location_info.column}] : {_message}";
            Console.WriteLine(message);
        }

        private static void Report(string message)
        {
            Console.WriteLine(message);
        }
    }
}



