using System;

public abstract class Expr
{
	virtual public Object accept(Interpreter interpreter)
	{
		return this;
	}
}
public class Binary : Expr
{
	public Expr left;
	public Token operation;
	public Expr right;
	public Binary(Expr left, Token operation, Expr right)
	{
		this.left = left;
		this.operation = operation;
		this.right = right;
	}
	override public Object accept(Interpreter interpreter)
	{
		return interpreter.visitBinaryExpr(this);
	}
}

public class Grouping : Expr
{
	public Expr expression;
	public Grouping(Expr expression)
	{
		this.expression = expression;
	}
	override public Object accept(Interpreter interpreter)
	{
		return interpreter.visitGroupingExpr(this);
	}
}

public class Literal : Expr
{
	public Object value;
	public Literal(Object value)
	{
		this.value = value;
	}
	override public Object accept(Interpreter interpreter)
	{
		return interpreter.visitLiteralExpr(this);
	}
}

public class Unary : Expr
{
	public Token operation;
	public Expr right;
	public Unary(Token operation, Expr right)
	{
		this.operation = operation;
		this.right = right;
	}
	override public Object accept(Interpreter interpreter)
	{
		return interpreter.visitUnaryExpr(this);
	}
}

public class Variable : Expr
{
	public Token name;
	public Variable(Token name)
	{
		this.name = name;
	}
	override public Object accept(Interpreter interpreter)
	{
		return interpreter.visitVariableExpr(this);
	}
}

public class Logical : Expr
{
	public Expr left;
	public Token operation;
	public Expr right;

	public Logical(Expr left, Token operation, Expr right)
	{
		this.left = left;
		this.operation = operation;
		this.right = right;
	}
	override public Object accept(Interpreter interpreter)
	{
		return interpreter.visitLogicalExpr(this);
	}
}
