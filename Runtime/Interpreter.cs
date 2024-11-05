﻿using Vowel.Errors;
using Vowel.Nodes;
using Vowel.vScanner;

namespace Vowel.Runtime
{
    public partial class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        //this is the global environment so we have no enclosing env
        //so it contains the global variables
        private VowelEnvironment env = new (null!);
        private Dictionary<Expr, Int32> local_variables = [];
        private Dictionary<string, Expr> local_vars = [];

        public void ResolveLocalVariable(Expr expr, Int32 scope_distance)
        {
            local_variables.Add(expr, scope_distance);
        }

        public void ResolveLocalVariable(string identifier, Expr expr)
        {
            local_vars.Add(identifier,expr);
        }
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
            if(local_variables.TryGetValue(expr, out int distance))
            {
                return env.GetVariableAt(expr.variable.lexeme, distance);
            }

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
            return Vowel.NIL;
        }

        //this a variable declaration
        public object VisitVarStatement(Stmt.VarStatement stmt)
        {
            string identifier_lexeme = stmt.identifier.lexeme;
            object initializer = Vowel.NIL;

            if(stmt.initializer is not null)
            {
                initializer = Evaluate(stmt.initializer);
            }

            env.Define(identifier_lexeme, initializer);
            return Vowel.NIL;
        }

        public object VisitAssignStatement(Expr.AssignStatement expr)
        {
            object value = Evaluate(expr.assignment_target);

            if(local_variables.TryGetValue(expr, out int distance))
            {
                env.AssignVariableAt(expr.name.lexeme,value,distance);
            }
            else
            {
                env.Assign(expr.name.lexeme, value);
            }
           
            return Vowel.NIL;
        }

        public object VisitBlockStatement(Stmt.BlockStatement stmt)
        {
            //executing a block introduces a new scope
            //atleast according to lexical scoping
            VowelEnvironment current_environment = new (env);
            VowelEnvironment previous_environment = env;
            env = current_environment;

            try
            {
                foreach (var statement in stmt.statements)
                {
                    Evaluate(statement);
                }
            }
            finally
            {
                //restore the previous environment
                env = previous_environment;
            }
          
            return Vowel.NIL;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            switch (expr._operator.token_type)
            {
                case TokenType.LOGICAL_OR:
                    // 'or' short circuits if any of the leaves
                    // is true
                    // so this will only jump to the end of the function 
                    // if the left side is false
                    object left_evaluation = Evaluate(expr.left);
                    if (IsTruthy(left_evaluation)) return expr.left;
                    break;
                case TokenType.LOGICAL_AND:
                    // 'and' short circuits if any of the leaves
                    // is false
                    // so this will only jump to the end of the function 
                    // if the left side is true
                    // just to make sure the semantics of 'and' are true
                    object _left_evaluation = Evaluate(expr.left);
                    if (!IsTruthy(_left_evaluation)) return expr.left;
                    break;
                default:
                    break;
            }

            return Evaluate(expr.right);
        }

        public object VisitIfStatement(Stmt.IFStatement stmt)
        {
            object condition_status = Evaluate(stmt.condition);
            bool is_truthy = IsTruthy(condition_status);

            if (is_truthy)
            {
                return Evaluate(stmt.then_branch);
                
            }

            if(stmt.else_branch is not null)
            {
                return Evaluate(stmt.else_branch);
            }

            return Vowel.NIL;
            
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
