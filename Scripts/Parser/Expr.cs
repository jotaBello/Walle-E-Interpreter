using System;

public abstract class Expr
{}
public class Binary : Expr
{
    Expr left;
    Token operation;
    Expr right;
    public Binary(Expr left, Token operation, Expr right)
    {
        this.left = left;
        this.operation = operation;
        this.right = right;
    }
}

public class Grouping : Expr
{
    Expr expression;
    public Grouping(Expr expression)
    {
        this.expression = expression;
    }
}

public class Literal : Expr
{
    Object value;
    public Literal(Object value)
    {
        this.value = value;
    }
}

public class Unary : Expr
{
    Token operation;
    Expr right;
    public Unary(Token operation, Expr right)
    {
        this.operation = operation;
        this.right = right;
    }
}