
using Vowel.vScanner;

namespace Vowel.Nodes
{
    public interface IStmtVisitor<T>
    {
        public T VisitPrintStatement(Stmt.PrintStatement stmt);
        public T VisitExpressionStatement(Stmt.ExpressionStatement stmt);
        public T VisitVarStatement(Stmt.VarStatement stmt);
        public T VisitBlockStatement(Stmt.BlockStatement stmt);
        public T VisitIfStatement(Stmt.IFStatement stmt);   
    }
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
        public class PrintStatement(Expr _printable): Stmt
        {
            public Expr printable = _printable;
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitPrintStatement(this);
            }
        }

        public class ExpressionStatement(Expr _expression) : Stmt
        {
            public Expr expression = _expression;
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitExpressionStatement(this);
            }
        }

        public class VarStatement(Token _keyword, Token _identifier, Expr _initializer): Stmt
        {
            public Token keyword = _keyword;
            public Token identifier = _identifier;
            public Expr initializer = _initializer;
            
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitVarStatement(this);
            }
        }

        public class BlockStatement(List<Stmt> _statements) :Stmt
        {
            public List<Stmt> statements = _statements;
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitBlockStatement(this);
            }
        }

        public class IFStatement(Expr _condition, Stmt _then_branch, Stmt _else_branch): Stmt
        {
            public Expr condition = _condition;
            public Stmt then_branch = _then_branch;
            public Stmt else_branch = _else_branch;
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitIfStatement(this);
            }
        }
    }
}
