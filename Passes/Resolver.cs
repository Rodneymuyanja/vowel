
using Vowel.Errors;
using Vowel.Nodes;
using Vowel.Runtime;
using Vowel.vScanner;

namespace Vowel.Passes
{
    internal class Resolver(Interpreter _vowel_interpreter): IExprVisitor<object>,IStmtVisitor<object>
    {
        private Stack<Dictionary<string, bool>> scopes = new ();
        private readonly Interpreter vowel_interpreter = _vowel_interpreter;
        private Dictionary<string, Expr> local_vars = [];

        public void Resolve(List<Stmt> statements)
        {
            try
            {
                foreach (var statment in statements)
                {
                    Resolve(statment);
                }
            }
            catch(VowelError v)
            {
                Vowel.Error(v.token, v.message);    
            }
            finally 
            {
                CleanUp();
            }
            
        }

        public object VisitAssignStatement(Expr.AssignStatement expr)
        {
            Resolve(expr.assignment_target);
            ResolveLocalVariableToInterpreter(expr, expr.name);
            return Vowel.NIL;
        }

        public object VisitBinaryExpr(Expr.BinaryExpr expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return Vowel.NIL;
        }

        public object VisitBlockStatement(Stmt.BlockStatement stmt)
        {
            BeginScope();
            foreach (var statement in stmt.statements)
            {
                Resolve(statement);
            }

            return Vowel.NIL;
        }

        public object VisitExpressionStatement(Stmt.ExpressionStatement stmt)
        {
            Resolve(stmt.expression);
            return Vowel.NIL;
        }

        public object VisitGroupExpr(Expr.GroupExpr expr)
        {
            Resolve(expr.expression);
            return Vowel.NIL;
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return Vowel.NIL;
        }

        public object VisitPrintStatement(Stmt.PrintStatement stmt)
        {
            Resolve(stmt.printable);
            return Vowel.NIL;
        }

        public object VisitUnaryExpr(Expr.UnaryExpr expr)
        {
            Resolve(expr.operand);
            return Vowel.NIL;
        }

        public object VisitVariable(Expr.Variable expr)
        {
            //the assumption is this a global variable
            if (scopes.Count == 0) return Vowel.NIL;
            //go tell the interpreter where to find it
            ResolveLocalVariableToInterpreter(expr, expr.variable);

            return Vowel.NIL;
        }

        public object VisitVarStatement(Stmt.VarStatement stmt)
        {
            Declare(stmt.identifier);
           
            if (stmt.initializer is not null)
            {
                Resolve(stmt.initializer);
            }

            Define(stmt.identifier);
            return Vowel.NIL;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return Vowel.NIL;
        }

        public object VisitIfStatement(Stmt.IFStatement stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.then_branch);

            if (stmt.else_branch is not null)
            {
                Resolve(stmt.else_branch);
            }

            return Vowel.NIL;
        }
        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;

            var scope = scopes.Peek();
            scope.Add(name.lexeme, false);
        }
        private void Define(Token name)
        {
            if (scopes.Count == 0) return;

            var scope = scopes.Peek();
            if (!scope.ContainsKey(name.lexeme))
            {
                throw new VowelError(name, $"Undefined variable");
            }

            scope[name.lexeme] = true;
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void BeginScope()
        {
            Dictionary<string, bool> scope = [];
            scopes.Push(scope);
        }
     
        private void ResolveLocalVariableToInterpreter(Expr expr, Token name)
        {
            int scopes_size = scopes.Count;
            //we need to start on top
            //the inner most scope
            for (int i = scopes_size - 1; i >= 0; i--)
            {
                var scope = scopes.ElementAt(i);
                if (scope.ContainsKey(name.lexeme))
                {
                    vowel_interpreter.ResolveLocalVariable(expr, i-1);
                    return;
                }
            }

            //if we dont find it in any of the nested environments
            //then it must be in the global environment

            vowel_interpreter.ResolveLocalVariable(expr, -1);
        }

        public object VisitTenaryExpr(Expr.TenaryExpr expr)
        {
            Resolve(expr.condition);
            Resolve(expr.then_branch);

            if (expr.else_branch is not null) 
            { 
                Resolve(expr.else_branch);
            }
            else
            {
                Vowel.Warning("'?:' does not have else part defined. If the condition evaluates to 'false', the result will be implicitly nil");
            }

            return Vowel.NIL;
        }

        public object VisitWhileStatement(Stmt.WhileStatement stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.body);
            return Vowel.NIL;
        }

        public object VisitCallExpr(Expr.CallExpression expr)
        {
            Resolve(expr.callee);

            foreach (var arg in expr.arguments)
            {
                Resolve(arg);
            }

            return Vowel.NIL;
        }

        public object VisitFunctionDeclaration(Stmt.FunctionDeclaration stmt)
        {
            Declare(stmt.token);
            Define(stmt.token);

            BeginScope();
            foreach (var parameter in stmt.parameters)
            {
                Declare(parameter);
                Define(parameter);
            }

            var block = stmt.block as Stmt.BlockStatement;
            var stmts = block!.statements;

            foreach (var block_stmt in stmts)
            {
                Resolve(block_stmt);
            }
            //Resolve(stmt.block);
            return Vowel.NIL;
        }

        //this releases memory
        //since we are done resolving all variables

        //i wonder how the gc handles the individual
        //scope...
        //for example a certain scope could have resolved 
        //variables 
        //so does doing scopes.Clear() handle the
        //values in scope(dictionary)

        private void CleanUp()
        {
            scopes.Clear();
        }

       
    }
}
