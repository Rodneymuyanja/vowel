using System.Text;

namespace Vowel.vScanner
{
    internal class Scanner
    {
        private string source_code;
        private int current_location = 0;
        private List<Token> tokens = [];
        private int line = 1;
        private int cursor = 1;
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
            keywords.Add("fn_decl", TokenType.FUNC);
            keywords.Add("if", TokenType.IF);
            keywords.Add("nil", TokenType.NIL);
            keywords.Add("oba", TokenType.LOGICAL_OR);//or
            keywords.Add("wandika", TokenType.PRINT);//print
            keywords.Add("return", TokenType.RETURN);
            keywords.Add("super", TokenType.BASE);
            keywords.Add("nze", TokenType.THIS);
            keywords.Add("true", TokenType.TRUE);
            keywords.Add("var", TokenType.VAR);
            keywords.Add("while", TokenType.WHILE);
        }

        private void ScanTokens()
        {
            char current_char = Current();
            switch (current_char)
            {
                case '\n':
                    line++;
                    cursor = 1;
                    break;
                case '(':
                    Add(TokenType.LEFT_PAREN);break;
                case ')':
                    Add(TokenType.RIGHT_PAREN);break;
                case '{':
                    Add(TokenType.LEFT_BRACE);break;
                case '}':
                    Add(TokenType.RIGHT_BRACE);break;
                case '.':
                    Add(TokenType.DOT);break;
                case '_':
                    Add(TokenType.UNDER_SCORE);break;
                case '^':
                    Add(TokenType.CARET);break;
                case '$':
                    Add(TokenType.SENTINEL);break;
                case '&':
                    Add(TokenType.AMPERSAND);break;
                case '%':
                    Add(TokenType.PERCENT);break;
                case '[':
                    Add(TokenType.LEFT_SQUARE);break;
                case ']':
                    Add(TokenType.RIGHT_SQUARE);break;
                case '?':
                    Add(TokenType.QUESTION_MARK); break;
                case ':':
                    Add(TokenType.FULL_COLON); break;
                case '=':
                    if (Match('='))
                    {
                        AddSpecial(TokenType.EQUAL_EQUAL);
                    }
                    else
                    {
                        Add(TokenType.EQUAL);
                    }
                    break;
                case '!':
                    if (Match('='))
                    {
                        AddSpecial(TokenType.BANG_EQUAL);
                    }
                    else
                    {
                        Add(TokenType.BANG);
                    }
                    break;
                case '>':
                    if (Match('='))
                    {
                        AddSpecial(TokenType.GREATER_EQUAL);
                    }
                    else
                    {
                        Add(TokenType.GREATER);
                    }
                    break;
                case '<':
                    if (Match('='))
                    {
                        AddSpecial(TokenType.LESS_EQUAL);
                    }
                    else
                    {
                        Add(TokenType.LESS);
                    }
                    break;
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
                case ' ':
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
                    else
                    {
                        //this is an error "unexpected character"
                        string message = $"Unexpected character '{current_char}' on line {line}, position {cursor}";
                        Vowel.Error(message);
                    }

                    
                    break;
            }
        }

        private void Add(TokenType token)
        {
            //we trackback because Current() goes forward
            char lexeme = TrackBack();
            AddToken(token, lexeme.ToString(), null);
        }

        private void AddSpecial(TokenType token)
        {
            //we trackback because Current() goes forward
            char previous = TrackBack();
            //move forward for the following character
            //things like !=
            char current = Current();
            string lexeme = $"{previous}{current}";
            AddToken(token, lexeme.ToString(), null);
        }

        private void AddToken(TokenType token_type, string lexeme, object? literal)
        {
            TokenLocationInfo location_info = new(line, cursor);
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

        private static bool IsAlphaSpecial(char c)
        {
            return c >= 'a' && c <= 'z'
                || c >= 'A' && c <= 'Z'
                || c == ' ' || c == '_';
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsDigit(c) || IsAlpha(c);
        }

        private static bool IsAlphaNumericSpecial(char c)
        {
            return IsDigit(c) || IsAlphaSpecial(c);
        }

        private bool Match(char expected)
        {
            if (EOF()) return false;

            if (Peek() == expected) return true;

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

            MoveOn();
            return c;
        }
        private void MoveOn()
        {
            cursor++;
            current_location++;
        }

        private void Rewind()
        {
            current_location--;
        }

        private char TrackBack()
        {
            int track_back_offset = current_location;
            return source_code[track_back_offset-1];
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
            int peek_offset = current_location;

            char c = source_code[peek_offset++];

            return c;
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
                while (IsAlphaNumericSpecial(Peek()))
                {
                    sb.Append(Current());
                }
            }
           
            build_string();
            string built_string = sb.ToString().Trim('"');

            //this is because we've consumed that last '"'
            MoveOn();

            AddToken(TokenType.STRING, built_string, built_string);
        }
    }
}
