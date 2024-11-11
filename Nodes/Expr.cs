
using Vowel.vScanner;

namespace Vowel.Nodes
{
    public interface IExprVisitor<T>
    {
        public T VisitUnaryExpr(Expr.UnaryExpr expr);
        public T VisitBinaryExpr(Expr.BinaryExpr expr);
        public T VisitGroupExpr(Expr.GroupExpr expr);
        public T VisitLiteralExpr(Expr.Literal expr);
        public T VisitVariable(Expr.Variable expr);
        public T VisitAssignStatement(Expr.AssignStatement stmt);
        public T VisitLogicalExpr(Expr.Logical expr);
        public T VisitTenaryExpr(Expr.TenaryExpr expr);
        public T VisitCallExpr(Expr.CallExpression expr);
    }
    public abstract class Expr
    {
        public abstract T Accept<T>(IExprVisitor<T> visitor);
        public class BinaryExpr(Expr _left_operand,Token _operator, Expr _right_operand) : Expr
        { 
            public Expr left = _left_operand;
            public Token _operator = _operator;
            public Expr right = _right_operand;

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        public class UnaryExpr(Token _operator,Expr _operand) : Expr
        {
            public Expr operand = _operand;
            public Token _operator = _operator;
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        public class GroupExpr(Expr _expression): Expr
        {
            public Expr expression = _expression;
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitGroupExpr(this);
            }
        }

        public class Literal(object obj): Expr
        {
            public object value = obj;
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        public class Variable(Token _variable) : Expr
        {
            public Token variable = _variable;

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitVariable(this);
            }
        }

        //this is in here because it's slips just between an expression
        //and equality..
        public class AssignStatement(Token _name, Expr _assignment_target) : Expr
        {
            public Token name = _name;
            public Expr assignment_target = _assignment_target;
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitAssignStatement(this);
            }
        }

        public class Logical(Expr _left, Token _operator, Expr _right) : Expr
        {
            public Expr left = _left;
            public Token _operator = _operator;
            public Expr right = _right;
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }

        public class TenaryExpr(Expr _condition, Expr _then_branch, Expr _else_branch) : Expr
        {
            public Expr condition = _condition;
            public Expr then_branch = _then_branch;
            public Expr else_branch = _else_branch;

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitTenaryExpr(this);
            }
        }

        public class CallExpression(Token _callee, List<Expr> _arguments) : Expr
        {
            public Token callee = _callee;
            public List<Expr> arguments = _arguments;

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }
    }
}
