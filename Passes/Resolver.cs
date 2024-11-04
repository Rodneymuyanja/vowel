
using Vowel.Errors;
using Vowel.Nodes;
using Vowel.Runtime;
using Vowel.vScanner;

namespace Vowel.Passes
{
    internal class Resolver: IExprVisitor<object>,IStmtVisitor<object>
    {
        private Stack<Dictionary<string, bool>> scopes = new ();
        private readonly Interpreter vowel_interpreter = new();
        private void Declare(Token name)
        {
            if (scopes.Count > 0) return;

            var scope = scopes.Peek();
            scope.Add(name.lexeme,false);
        }
        private void Define(Token name)
        {
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
        private void EndScope() 
        {
            scopes.Pop();
        }

        private void ResolveLocalVariableToInterpreter(Expr expr)
        {
            int scopes_size = scopes.Count;
            //we need to stop on top
            //the inner most scope
            for (int i = scopes_size; i >=0; i--)
            {
                var scope = scopes.
            }
        }

        public object VisitAssignStatement(Expr.AssignStatement stmt)
        {
            Resolve(stmt.assignment_target);
            return Vowel.NULL;
        }

        public object VisitBinaryExpr(Expr.BinaryExpr expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return Vowel.NULL;
        }

        public object VisitBlockStatement(Stmt.BlockStatement stmt)
        {
            BeginScope();
            foreach (var statement in stmt.statements)
            {
                Resolve(statement);
            }
            EndScope();
            return Vowel.NULL;
        }

        public object VisitExpressionStatement(Stmt.ExpressionStatement stmt)
        {
            Resolve(stmt.expression);
            return Vowel.NULL;
        }

        public object VisitGroupExpr(Expr.GroupExpr expr)
        {
            Resolve(expr.expression);
            return Vowel.NULL;
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return Vowel.NULL;
        }

        public object VisitPrintStatement(Stmt.PrintStatement stmt)
        {
            Resolve(stmt.printable);
            return Vowel.NULL;
        }

        public object VisitUnaryExpr(Expr.UnaryExpr expr)
        {
            Resolve(expr.operand);
            return Vowel.NULL;
        }

        public object VisitVariable(Expr.Variable expr)
        {
            //the assumption is this a global variable
            if (scopes.Count == 0) return Vowel.NULL;

            var scope = scopes.Peek();
            var found = scope[expr.variable.lexeme];

            if (!found) throw new VowelError(expr.variable, $"Undefined variable '{expr.variable.lexeme}' .");

        }

        public object VisitVarStatement(Stmt.VarStatement stmt)
        {
            BeginScope();
            Declare(stmt.identifier);

            if (stmt.initializer is not null)
            {
                Resolve(stmt.initializer);
            }

            Define(stmt.identifier);
            return Vowel.NULL;
        }
    }
}
