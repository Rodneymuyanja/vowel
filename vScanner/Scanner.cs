
using System.Text;

namespace Vowel.vScanner
{
    internal class Scanner
    {
        private string source_code;
        private int current_location = 0;
        private List<Token> tokens = [];
        private int line = 1;
        //private int cursor = 0;
        private readonly Dictionary<string, TokenType> keywords = [];

        public Scanner(string _source_code)
        {
            source_code = _source_code;
            LoadKeywords();
        }

        public List<Token> ScanSourceCode()
        {
            while (!EOF())
            {
                ScanTokens();
            }

            AddToken(TokenType.EOF, string.Empty, null);
            return tokens;
        }

        private void LoadKeywords()
        {
            keywords.Add("ne", TokenType.LOGICAL_AND);//and
            keywords.Add("class", TokenType.CLASS);
            keywords.Add("else", TokenType.ELSE);
            keywords.Add("false", TokenType.FALSE);
            keywords.Add("for", TokenType.FOR);
            keywords.Add("func", TokenType.FUNC);
            keywords.Add("if", TokenType.IF);
            keywords.Add("nil", TokenType.NIL);
            keywords.Add("oba", TokenType.LOGICAL_OR);//or
            keywords.Add("wandika", TokenType.PRINT);//wandika
            keywords.Add("return", TokenType.RETURN);
            keywords.Add("super", TokenType.BASE);
            keywords.Add("nze", TokenType.THIS);
            keywords.Add("true", TokenType.TRUE);
            keywords.Add("var", TokenType.VAR);
            keywords.Add("albeit", TokenType.WHILE);
        }

        private void ScanTokens()
        {
            char current_char = Current();
            switch (current_char)
            {
                case '\n':
                    line++;break;
                case '(':
                    Add(TokenType.LEFT_PAREN);break;
                case ')':
                    Add(TokenType.RIGHT_PAREN);break;
                case '{':
                    Add(TokenType.LEFT_PAREN);break;
                case '}':
                    Add(TokenType.RIGHT_PAREN);break;
                case '.':
                    Add(TokenType.DOT);break;
                case '_':
                    Add(TokenType.UNDER_SCORE);break;
                case '^':
                    Add(TokenType.CARET);break;
                case '$':
                    Add(TokenType.DOLLAR);break;
                case '&':
                    Add(TokenType.AMPERSAND);break;
                case '%':
                    Add(TokenType.PERCENT);break;
                case '[':
                    Add(TokenType.LEFT_SQUARE);break;
                case ']':
                    Add(TokenType.RIGHT_SQUARE);break;
                case '=':
                    Add(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);break;
                case '!':
                    Add(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);break;
                case '>':
                    Add(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '<':
                    Add(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '+':
                    Add(TokenType.PLUS); break;
                case '-':
                    Add(TokenType.MINUS); break;
                case ',':
                    Add(TokenType.COMMA); break;
                case '*':
                    Add(TokenType.STAR); break;
                case ';':
                    Add(TokenType.SEMICOLON); break;
                case '"':
                    ScanString();break;
                case '\r':
                case '\t':
                    //whitespace
                    break;
                case '/':
                    bool is_comment = Match('/');
                    if (is_comment)
                    {
                        ScanComment();
                    }
                    else
                    {
                        Add(TokenType.SLASH);
                    }

                    break;

                default:
                    if (IsDigit(current_char))
                    {
                        ScanNumber();
                    }
                    else if (IsAlpha(current_char))
                    {
                        ScanIdentifier();
                    }

                    //this is an error "unexpected character"
                    break;
            }
        }

        private void Add(TokenType token)
        {
            //we trackback because Current() goes forward
            char lexeme = TrackBack();
            AddToken(token, lexeme.ToString(), null);
        }

        private void AddToken(TokenType token_type, string lexeme, object? literal)
        {
            TokenLocationInfo location_info = new(current_location, line);
            Token token = new(token_type, lexeme, literal!, location_info);

            tokens.Add(token);
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z'
                || c >= 'A' && c <= 'Z';
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsDigit(c) || IsAlpha(c);
        }

        private bool Match(char expected)
        {
            if (EOF()) return false;

            if (source_code[current_location] == expected) return true;

            current_location++;
            return false;
        }

        private bool EOF()
        {
            return current_location >= source_code.Length;
        }
        private char Current()
        {
            //get the current token
            char c = source_code[current_location];
            //move on
            MoveOn();
            return c;
        }
        private void MoveOn()
        {
            current_location++;
        }

        private void Rewind()
        {
            current_location--;
        }

        private char TrackBack()
        {
            return source_code[current_location--];
        }

        private void ScanComment()
        {
            while (Peek() != '\0')
            {
                if (Current() == '\n')
                {
                    break;
                }
            }
        }

        private char Peek()
        {
            if (EOF()) return '\0';

            return source_code[current_location++];
        }

        private void ScanNumber()
        {
            StringBuilder sb = new();

            void build_digit()
            {
                while (IsDigit(Peek()))
                {
                    sb.Append(Current());
                }
            }
            //this is because current()
            //increased current_location
            Rewind();

            build_digit();

            if (Peek() == '.')
            {
                build_digit();
            }

            string number = sb.ToString();
            double built_number = double.Parse(number);

            AddToken(TokenType.NUMBER, number, built_number);
        }

        private void ScanIdentifier()
        {
            StringBuilder sb = new();

            void build_string()
            {
                while (IsAlphaNumeric(Peek()))
                {
                    sb.Append(Current());
                }
            }
            //this is because current()
            //increased current_location
            Rewind();
            build_string();
            string built_string = sb.ToString();

            if (keywords.TryGetValue(built_string, out TokenType keyword))
            {
                AddToken(keyword, built_string, null);
                return;
            }

            AddToken(TokenType.IDENTIFIER, built_string, built_string);
        }

        private void ScanString()
        {
            StringBuilder sb = new();

            void build_string()
            {
                while (IsAlphaNumeric(Peek()))
                {
                    sb.Append(Current());
                }
            }
           
            build_string();
            string built_string = sb.ToString().Trim('"');

            AddToken(TokenType.STRING, built_string, built_string);
        }
    }
}
