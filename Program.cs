
using System.Text;
using Vowel.vScanner;

Vowel.Vowel.Main();

namespace Vowel
{
    public class Vowel
    {
        public static void Main()
        {
            ReadVowelSource();
        }

        private static void ReadVowelSource()
        {
            string path = @"D:\notes2\voel.vowel";
            byte [] byte_array = File.ReadAllBytes(path);
            string content = Encoding.UTF8.GetString(byte_array);
            ScanSourceCode(content);
        }

        private static void ScanSourceCode(string source_code)
        {
            Scanner scanner = new (source_code);
            scanner.ScanSourceCode();
        }
    }
}



