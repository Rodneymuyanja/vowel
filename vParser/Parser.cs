
using Vowel.Errors;
using Vowel.Nodes;
using Vowel.vScanner;

namespace Vowel.vParser
{
    /// 
    ///          _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ 
    ///         |                                             |
    ///         |                VOWEL GRAMMAR                |
    ///         |_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _|
    /// 
    /// expression      -> equality;
    /// equality        -> comparison (("==" | "!=" ) comparison)*;
    /// comparison      -> term ((">" | ">=" | "<" | ">=") term)*;
    /// term            -> factor (("+" | "-") factor)*;
    /// factor          -> unary (("*" | "/" | "%") unary)*;
    /// unary           -> ("!"|"-") unary 
    ///                    | primary;
    /// primary         -> NUMBER | STRING | "false" | "true" | "nil"
    ///                    | IDENTIFIER | "(" expression ")";
    ///                    
    public class Parser(List<Token> _tokens)
    {
        private readonly List<Token> tokens = _tokens;
        private int current = 0; 

        public List<Expr> Parse()
        {
            List<Expr> expressions = [];
            try
            {
                while (!IsAtEnd())
                {
                    expressions.Add(Expression());
                }
            }
            catch (VowelError v_error)
            {
                Vowel.Error(v_error.token, v_error.message);
            }

            return expressions;
        }

        private void Statement()
        {

        }

        public Expr Expression()
        {
            Expr expr = Equality();
            return expr;
        }
        /// equality        -> comparison (("==" | "!=" ) comparison)*;
        private Expr Equality()
        {
            Expr expression = Comparison();
            List<TokenType> equality_ops = [TokenType.EQUAL_EQUAL, TokenType.BANG_EQUAL];
            while (Match(equality_ops)) 
            {
                Token _operator = TrackBack();
                Expr right = Comparison();
                expression = new Expr.BinaryExpr(expression, _operator, right);
            }

            return expression;
        }
        /// comparison      -> term ((">" | ">=" | "<" | ">=") term)*;
        private Expr Comparison()
        {
            Expr expression = Term();
            List<TokenType> term_ops = [TokenType.GREATER, TokenType.GREATER_EQUAL,TokenType.LESS,TokenType.LESS_EQUAL];

            while (Match(term_ops))
            {
                Token _operator = TrackBack();
                Expr right = Term();
                expression = new Expr.BinaryExpr(expression, _operator, right);
            }

            return expression;
        }
        /// term            -> factor (("+" | "-") factor)*;
        private Expr Term()
        {
            Expr expression = Factor();
            List<TokenType> term_ops = [TokenType.PLUS, TokenType.MINUS];

            while (Match(term_ops))
            {
                Token _operator = TrackBack();
                Expr right = Factor();
                expression = new Expr.BinaryExpr(expression, _operator, right);
            }

            return expression;
        }
        /// factor          -> unary (("*" | "/" | "%") unary)*;
        private Expr Factor() 
        {
            Expr expression = Unary();
            List<TokenType> factor_ops = [TokenType.STAR, TokenType.SLASH,TokenType.PERCENT,TokenType.CARET];

            while (Match(factor_ops)) 
            {
                Token _operator = TrackBack();
                Expr right = Unary();
                expression = new Expr.BinaryExpr(expression, _operator,right);
            }

            return expression;

        }
        /// unary           -> ("!" | "-") unary 
        ///                    | primary;
        private Expr Unary()
        {
            List<TokenType> unary_ops = [TokenType.BANG, TokenType.MINUS];
            if (Match(unary_ops))
            {
                Token _operator = TrackBack();
                Expr expr = Unary();

                return new Expr.UnaryExpr(_operator, expr);
            }

            return Primary();
        }
        /// primary         -> NUMBER | STRING | "false" | "true" | "nil"
        ///                    | IDENTIFIER | "(" expression ")";
        /// 
        private Expr Primary()
        { 
            object literal = Peek().literal;

            if (Match([TokenType.NUMBER, TokenType.STRING])) return new Expr.Literal(literal);
            if (Match([TokenType.FALSE])) return new Expr.Literal("false");
            if (Match([TokenType.TRUE])) return new Expr.Literal("true");
            if (Match([TokenType.NIL])) return new Expr.Literal("nil");
            if (Match([TokenType.IDENTIFIER])) return new Expr.Literal(literal);

            if (Match([TokenType.LEFT_PAREN])) return Grouping();

            throw new RuntimeError("Unexpected token");

        }

        /// grouping      -> "(" expression ")";
        private Expr Grouping()
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_BRACE, "Expected ')' after group expression");
            return new Expr.GroupExpr(expr);
        }

        private void Advance()
        {
            current++;
        }

        private Token Consume(TokenType token_type, string error_message)
        {
            Token current_token = Peek();
            if (current_token.token_type == token_type) {

                Advance();
                return current_token;
            }

            throw new VowelError(current_token,error_message);
        }

        private bool Match(List<TokenType> _token_types)
        {
            foreach (var token_type in _token_types)
            {
                if (Check(token_type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }
        private Token TrackBack()
        {
            int previous = current;
            previous = previous - 1;
            Token token = tokens[previous];
            return token;
        }
        private  bool Check(TokenType token_type) 
        {
            if(IsAtEnd()) return false;

            if (Peek().token_type == token_type)
            {
                return true;
            }

            return false;
        }

        private  Token Peek()
        {
            return tokens[current];
        }
        private bool IsAtEnd()
        {
            return tokens[current].token_type == TokenType.EOF;
        }
    }
}
