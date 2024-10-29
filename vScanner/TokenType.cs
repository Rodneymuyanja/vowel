﻿
namespace Vowel.vScanner
{
    internal enum TokenType
    {
        //singles
        BANG,UNDER_SCORE,DOLLAR,CARET,AMPERSAND,
        LEFT_PAREN,RIGHT_PAREN,
        LEFT_BRACE,RIGHT_BRACE,COMMA,DOT,LEFT_SQUARE,RIGHT_SQUARE,
        SEMICOLON,
        //maths
        PLUS,MINUS,SLASH,PERCENT,STAR,

        //comparison
        GREATER,LESS,GREATER_EQUAL,LESS_EQUAL,LOGICAL_OR,
        LOGICAL_AND, BANG_EQUAL, EQUAL, EQUAL_EQUAL,

        //literals
        IDENTIFIER,NUMBER,STRING,
         
        //keywords
        CLASS,VAR,PRINT, ELSE, IF, RETURN, WHILE, FOR,
        TRUE,FALSE,NIL,FUNC,THIS,BASE,

        EOF
    }
}
