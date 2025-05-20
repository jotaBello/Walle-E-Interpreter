using System.Collections.Generic;
using Godot;
using Microsoft.CSharp.RuntimeBinder;
public abstract class Stmt
{
    virtual public void accept(Interpreter interpreter)
    {
        //TEMPORAL
    }
}

public class PrintStmt : Stmt
{
    public Expr expression;

    public PrintStmt(Expr expression)
    {
        this.expression = expression;
    }
    override public void accept(Interpreter interpreter)
    {
        interpreter.visitPrintStmt(this);
    }

}
public class ExpressionStmt : Stmt
{
    public Expr expression;

    public ExpressionStmt(Expr expression)
    {
        this.expression = expression;
    }
    override public void accept(Interpreter interpreter)
    {
        interpreter.visitExpressionStmt(this);
    }

}