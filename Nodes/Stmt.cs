
namespace Vowel.Nodes
{
    public interface IStmtVisitor<T>
    {
        public T VisitPrintStmt(Stmt.PrintStatement stmt);
    }
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
        public class PrintStatement(object _printable): Stmt
        {
            public object printable = _printable;
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }
    }
}
