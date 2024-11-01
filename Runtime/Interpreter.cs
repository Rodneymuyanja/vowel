using Vowel.Errors;
using Vowel.Nodes;
using Vowel.vScanner;

namespace Vowel.Runtime
{
    internal class Interpreter : IExprVisitor<object>
    {
        public void Interpret(Expr expr) 
        {
            var result = Evaluate(expr);
        }

        public object VisitBinaryExpr(Expr.BinaryExpr expr)
        {
            TokenType _operator = expr._operator.token_type;
            object left = Evaluate(expr.left);
            object right = Evaluate(expr.right);
            switch (_operator)
            {
                case TokenType.PLUS:
                    if(left is double left_value && right is double right_value)
                    {
                        return  left_value + right_value;
                    }

                    return $"{left}{right}";

                case TokenType.MINUS:
                    CheckForDouble(left, right, expr._operator);
                    return (double)left - (double)right;

                case TokenType.STAR:
                    CheckForDouble(left, right, expr._operator);
                    return (double)left * (double)right;

                case TokenType.SLASH:
                    CheckForDouble(left, right, expr._operator);

                    if((double)right == 0)
                    {
                        throw new RuntimeError("Zero division");
                    }
                    return (double)left / (double)right;

                case TokenType.PERCENT:
                    CheckForDouble(left, right, expr._operator);
                    if ((double)right == 0)
                    {
                        throw new RuntimeError("Zero division");
                    }
                    return (double)left % (double)right;

                case TokenType.CARET:
                    CheckForDouble(left, right, expr._operator);
                    if ((double)right == 0)
                    {
                       return 1;
                    }
                    return Power((double)left,(double)right);

                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);

                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);

                case TokenType.GREATER:
                    return Compare(left, right, _operator);

                case TokenType.LESS:
                    return Compare(left, right, _operator);

                case TokenType.GREATER_EQUAL:
                    return Compare(left, right, _operator);

                case TokenType.LESS_EQUAL:
                    return Compare(left, right, _operator);

                default:
                    throw new RuntimeError("Could not perform operation");
            }
        }

        public object VisitGroupExpr(Expr.GroupExpr expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object VisitUnaryExpr(Expr.UnaryExpr expr)
        {
            TokenType _operator = expr._operator.token_type;
            object operand = Evaluate(expr.operand);
            switch (_operator)
            {
                case TokenType.MINUS:
                    if(operand is not double)
                    {
                        throw new RuntimeError($"unary operand must be double");
                    }
                    return (double)operand;
                case TokenType.BANG:
                default:
                    throw new RuntimeError("Unknown operator");
            }
        }

        //this is our bread and butt--uhhh
        private object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }

        private static void CheckForDouble(object left, object right, Token _operator)
        {
            if(left is not double)
            {
                throw new RuntimeError($"{_operator.lexeme} requires left value to be type 'double' but found {left}");
            }

            if (right is not double)
            {
                throw new RuntimeError($"{_operator.lexeme} requires right value to be type 'double' but found {right}");
            }
        }

        private static double Power(double value,double power)
        {
            if (power == 0) return 1;

            return value * Power(value, power - 1);
        }
        private static bool IsEqual(object left, object right)
        {
            if (right is null && left is null) return true;
            if(left is null) return false;
            return left.Equals(right);
        }

        private bool IsTruthy(object value)
        {
            if(value is null) return false;

            if(value is bool v)
            {
                return v;
            }

            return true;
        }

        private bool Compare(object left, object right, TokenType token_type)
        {
            if(left  == null || right == null) return false;

            switch (token_type)
            {
                case TokenType.GREATER:
                    if (left is double && right is double)
                    {
                        return (double)left > (double)right;
                    }

                    //this implies that comparing strings and numbers is allowed
                    //for example "class string" > 4 would be true

                    //strange and could be a source of bugs
                    if (left is string && right is double)
                    {
                        return left.ToString()!.Length > (double)right;
                    }

                    if (left is double && right is string)
                    {
                        return (double)left > right.ToString()!.Length;
                    }

                    if (left is string && right is string)
                    {
                        return left.ToString()!.Length > right.ToString()!.Length;
                    }


                    break;
                case TokenType.GREATER_EQUAL:
                    if (left is double && right is double)
                    {
                        return (double)left >= (double)right;
                    }

                    if (left is string && right is double)
                    {
                        return  left.ToString()!.Length >= (double)right;
                    }

                    if (left is double  && right is string)
                    {
                        return (double)left >= right.ToString()!.Length;
                    }

                    if (left is string && right is string )
                    {
                        return left.ToString()!.Length >= right.ToString()!.Length;
                    }

                    break;
                case TokenType.LESS:
                    if (left is double && right is double)
                    {
                        return (double)left < (double)right;
                    }

                    if (left is string && right is double)
                    {
                        return left.ToString()!.Length < (double)right;
                    }

                    if (left is double && right is string)
                    {
                        return (double)left < right.ToString()!.Length;
                    }

                    if (left is string && right is string)
                    {
                        return left.ToString()!.Length < right.ToString()!.Length;
                    }

                    break;
                case TokenType.LESS_EQUAL:
                    if (left is double && right is double)
                    {
                        return (double)left <= (double)right;
                    }

                    if (left is string && right is double)
                    {
                        return left.ToString()!.Length <= (double)right;
                    }

                    if (left is double && right is string)
                    {
                        return (double)left <= right.ToString()!.Length;
                    }

                    if (left is string && right is string)
                    {
                        return left.ToString()!.Length <= right.ToString()!.Length;
                    }

                    break;

                default:
                    return false;
            }

            return false;
        }
    }
}
