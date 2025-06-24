using System;
using System.Collections.Generic;

public abstract class Expr
{
	virtual public Object accept(Visitor<object> visitor)
	{
		return null;
	}

	public interface Visitor<T>
	{
		public T visitBinaryExpr(Binary expr);
		public T visitGroupingExpr(Grouping expr);
		public T visitLiteralExpr(Literal expr);
		public T visitUnaryExpr(Unary expr);
		public T visitVariableExpr(Variable expr);
		public T visitLogicalExpr(Logical xpr);
		public T visitCallExpr(Call expr);
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
	override public Object accept(Visitor<object> visitor)
	{
		return visitor.visitBinaryExpr(this);
	}
}

public class Grouping : Expr
{
	public Expr expression;
	public Grouping(Expr expression)
	{
		this.expression = expression;
	}
	override public Object accept(Visitor<object> visitor)
	{
		return visitor.visitGroupingExpr(this);
	}
}

public class Literal : Expr
{
	public Object value;
	public Literal(Object value)
	{
		this.value = value;
	}
	override public Object accept(Visitor<object> visitor)
	{
		return visitor.visitLiteralExpr(this);
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
	override public Object accept(Visitor<object> visitor)
	{
		return visitor.visitUnaryExpr(this);
	}
}

public class Variable : Expr
{
	public Token name;
	public Variable(Token name)
	{
		this.name = name;
	}
	override public Object accept(Visitor<object> visitor)
	{
		return visitor.visitVariableExpr(this);
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
	override public Object accept(Visitor<object> visitor)
	{
		return visitor.visitLogicalExpr(this);
	}
}

public class Call : Expr
{
	public int loopCount=0;
	public Token name;
	public Token paren;
	public List<Expr> parameters;
	public int Arity;

	public Call(Token name, Token paren, List<Expr> arguments)
	{
		this.name = name;
		this.paren = paren;
		this.parameters = arguments;
		Arity=arguments.Count;
	}
	override public Object accept(Visitor<object> visitor)
	{
		return visitor.visitCallExpr(this);
	}
}
