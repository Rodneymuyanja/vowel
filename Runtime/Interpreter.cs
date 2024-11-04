﻿using Vowel.Errors;
using Vowel.Nodes;
using Vowel.vScanner;

namespace Vowel.Runtime
{
    public partial class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        //this is the global environment so we have no enclosing env
        private readonly VowelEnvironment env = new (null!);
        public void Interpret(List<Stmt> statements) 
        {
            try
            {
                foreach (var statement in statements)
                {
                    Evaluate(statement);
                }
            }
            catch (RuntimeError)
            {
                throw;
            }
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
                    return -(double)operand;
                case TokenType.BANG:
                    return !IsTruthy(operand);
                default:
                    throw new RuntimeError("Unknown operator");
            }
        }

        public object VisitVariable(Expr.Variable expr)
        {
            return env.Get(expr.variable.lexeme);
        }
        
        public object VisitExpressionStatement(Stmt.ExpressionStatement stmt)
        {
            return Evaluate(stmt.expression);
        }

        public object VisitPrintStatement(Stmt.PrintStatement stmt)
        {
            object expression = Evaluate(stmt.printable);
            Console.WriteLine(expression.ToString());
            return null!;
        }

        //this a variable declaration
        public object VisitVarStatement(Stmt.VarStatement stmt)
        {
         //    Token keyword = _keyword;
         //Token identifier = _identifier;
         //Expr initializer = _initializer;

            string identifier_lexeme = stmt.identifier.lexeme;
            object initializer = null!;
            if(stmt.initializer is not null)
            {
                initializer = Evaluate(stmt.initializer);
            }

            env.Define(identifier_lexeme, initializer);
            return null!;
        }

        //this is our bread and butt--uhhh
        private object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }

        private object Evaluate(Stmt statement)
        {
            return statement.Accept(this);
        }

    }
}
