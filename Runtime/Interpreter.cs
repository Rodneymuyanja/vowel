using Vowel.Errors;
using Vowel.Nodes;
using Vowel.vScanner;

namespace Vowel.Runtime
{
    public partial class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        //this is the global environment so we have no enclosing env
        //so it contains the global variables
        public Snapshot snapshot = new ();
        public Dictionary<string, Snapshot> snapshot_bindings = [];

        public void Interpret(List<Stmt> statements) 
        {
            try
            {
                foreach (var statement in statements)
                {
                    Evaluate(statement);
                }
            }
            catch (RuntimeError r)
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
            var obj = snapshot.GetValue(expr.variable.lexeme);

            return obj;
        }
        
        public object VisitExpressionStatement(Stmt.ExpressionStatement stmt)
        {
            return Evaluate(stmt.expression);
        }

        public object VisitPrintStatement(Stmt.PrintStatement stmt)
        {
            object expression = Evaluate(stmt.printable);

            if(expression is null)
            {
                Console.WriteLine("nil");
            }
            else
            {
                Console.WriteLine(expression!.ToString());
            }

            return Vowel.NIL;
        }

        //this a variable declaration
        //every variable declaration causes
        //causes a split in the environment
        public object VisitVarStatement(Stmt.VarStatement stmt)
        {
            string identifier_lexeme = stmt.identifier.lexeme;
            object initializer = Vowel.NIL;

            if (stmt.initializer is not null)
            {
                initializer = Evaluate(stmt.initializer);
            }

            snapshot.TakeSnapshot(identifier_lexeme,initializer);
            return Vowel.NIL;
        }

        //variable assignment causes a mutation
        public object VisitAssignStatement(Expr.AssignStatement expr)
        {
            object value = Evaluate(expr.assignment_target);
            snapshot.MutateValue(expr.name.lexeme, value);
            return Vowel.NIL;
        }

        public object VisitBlockStatement(Stmt.BlockStatement stmt)
        {
            return ExecuteBlock(stmt, snapshot);
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
                    if (IsTruthy(left_evaluation)) return left_evaluation;
                    break;
                case TokenType.LOGICAL_AND:
                    // 'and' short circuits if any of the leaves
                    // is false
                    // so this will only jump to the end of the function 
                    // if the left side is true
                    // just to make sure the semantics of 'and' are true
                    object _left_evaluation = Evaluate(expr.left);
                    if (!IsTruthy(_left_evaluation)) return _left_evaluation;
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

        public object VisitTenaryExpr(Expr.TenaryExpr expr)
        {
            object condition = Evaluate(expr.condition);

            if (condition is not Boolean)
            {
                throw new RuntimeError("'?!' condition is expected to evaluate to boolean type");
            }

            bool is_truthy = IsTruthy(condition);

            if (is_truthy)
            {
                return Evaluate(expr.then_branch);
            }

            if (expr.else_branch is not null)
            {
                return Evaluate(expr.else_branch);
            }

            return Vowel.NIL;
        }

        public object VisitWhileStatement(Stmt.WhileStatement stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition)))
            {
                Evaluate(stmt.body);
            }

            return Vowel.NIL;
        }

        public object VisitCallExpr(Expr.CallExpression expr)
        {
            object callee = Evaluate(expr.callee);
            if(callee is not Function)
            {
                throw new RuntimeError("Can only call functions");
            }

            Function function = (Function) callee;
            if(expr.arguments.Count != function.Arity())
            {
                function = FindOverLoad(expr, function);
            }

            List<object> arguments = [];

            foreach (var arg in expr.arguments)
            {
                object evaluation = Evaluate(arg);
                arguments.Add(evaluation);
            }

            return function.Call(this, arguments);
        }

        private Function FindOverLoad(Expr.CallExpression expr, Function function)
        {
            var overload = snapshot.FindFunction(expr.arguments.Count);

            return overload is null
                ? throw new RuntimeError($"No overload of {function.function_declaration.token.lexeme} takes {expr.arguments.Count} argument(s), found " +
                    $"{function} but expects {function.Arity()} argument(s)")
                : overload;
        }

        public object VisitFunctionDeclaration(Stmt.FunctionDeclaration stmt)
        {
            Function vowel_function = new (stmt);
            string function_runtime_name = $"{stmt.token.lexeme}${vowel_function.Arity()}";
            snapshot.TakeSnapshot(stmt.token.lexeme, vowel_function);
            BindSnapshot(function_runtime_name);
            return Vowel.NIL;
        }

        public object VisitClassStatement(Stmt.ClassStatement stmt)
        {
            VowelClass vowel_class = new (stmt);
            snapshot.TakeSnapshot(stmt.token.lexeme, vowel_class);
            BindSnapshot(stmt.token.lexeme);
            return vowel_class;
        }

        public object VisitSetExpr(Expr.SetExpression expr)
        {
            object target = Evaluate(expr.target);

            if(target is not VowelInstance)
            {
                throw new RuntimeError($"Set target should be an object instance {target} is not an instance.");
            }

            //notice the reaosn we the target is because it has
            //the state(field) we're trying to set
            VowelInstance instance = (VowelInstance)target;
            //on that instance is where we plug that value
            instance.Set(expr.identifier.lexeme, expr.value);
            return Vowel.NIL;
        }

        public object VisitGetExpr(Expr.GetExpression expr)
        {
            object source = Evaluate(expr);
            if(source is not VowelInstance)
            {
                throw new RuntimeError($"Only instances have properties that can be accessed via get-expressions, '{source}' is not an object instance.");
            }

            VowelInstance instance = (VowelInstance)source;
            return instance.Get(expr.identifier.lexeme);
        }

        //binding a snapshot refers to attatching 
        //a lexeme to the current snapshot
        //that data structures like functions 
        //are bound to the currently active environment
        private void BindSnapshot(string lexeme)
        {
            Snapshot bound_snapshot = new();
            bound_snapshot.frames.Push(snapshot.GetFrame());
            snapshot_bindings.Add(lexeme, bound_snapshot);
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
