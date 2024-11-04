

using Vowel.Errors;
using Vowel.vScanner;

namespace Vowel.Runtime
{
    public partial class Interpreter
    {
        private static double Power(double value, double power)
        {
            if (power == 0) return 1;

            return value * Power(value, power - 1);
        }
        private static void CheckForDouble(object left, object right, Token _operator)
        {
            if (left is not double)
            {
                throw new RuntimeError($"{_operator.lexeme} requires left value to be type 'double' but found {left}");
            }

            if (right is not double)
            {
                throw new RuntimeError($"{_operator.lexeme} requires right value to be type 'double' but found {right}");
            }
        }

        private static bool IsEqual(object left, object right)
        {
            if (right is null && left is null) return true;
            if (left is null) return false;
            return left.Equals(right);
        }

        private bool IsTruthy(object value)
        {
            if (value is null) return false;

            if (value is bool v)
            {
                return v;
            }

            return true;
        }

        private bool Compare(object left, object right, TokenType token_type)
        {
            if (left == null || right == null) return false;

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
                        return left.ToString()!.Length >= (double)right;
                    }

                    if (left is double && right is string)
                    {
                        return (double)left >= right.ToString()!.Length;
                    }

                    if (left is string && right is string)
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
