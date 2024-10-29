
namespace Vowel.vScanner
{
    internal enum TokenType
    {
        //singles
        BANG,UNDER_SCORE,DOLLAR,CARET,AMPERSAND,
        EQUAL,EQUAL_EQUAL,LEFT_PAREN,RIGHT_PAREN,
        LEFT_BRACE,RIGHT_BRACE,COMMA,DOT,
        //maths
        PLUS,MINUS,SLASH,PERCENT,

        //comparison
        GREATER,LESS,GREATER_EQUAL,LESS_EQUAL,LOGICAL_OR,
        LOGICAL_AND, 

        //literals
        IDENTIFIER,NUMBER,STRING,
         
        //keywords
        CLASS,VAR,PRINT, ELSE, IF, RETURN, WHILE, FOR,
        TRUE,FALSE,NIL,

        EOF
    }
}
