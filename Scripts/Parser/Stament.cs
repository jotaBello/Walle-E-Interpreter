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

public class VarStmt : Stmt
{
	public Token name;
	public Expr expression;

	public VarStmt(Token name, Expr expression)
	{
		this.expression = expression;
		this.name = name;
	}
	override public void accept(Interpreter interpreter)
	{
		interpreter.visitVarStmt(this);
	}
}

public class BlockStmt : Stmt
{
	public List<Stmt> statements;

	public BlockStmt(List<Stmt> statements)
	{
		this.statements = statements;
	}
	override public void accept(Interpreter interpreter)
	{
		interpreter.visitBlockStmt(this);
	}
}

public class IfStmt : Stmt
{
	public Expr condition;

	public Stmt thenBranch;
	public Stmt elseBranch;

	public IfStmt(Expr condition, Stmt thenBranch, Stmt elseBranch)
	{
		this.condition = condition;
		this.thenBranch = thenBranch;
		this.elseBranch = elseBranch;
	}
	override public void accept(Interpreter interpreter)
	{
		interpreter.visitIfStmt(this);
	}
}

public class WhileStmt : Stmt
{
	public Expr condition;

	public Stmt body;

	public WhileStmt(Expr condition, Stmt body)
	{
		this.condition = condition;
		this.body = body;
	}
	override public void accept(Interpreter interpreter)
	{
		interpreter.visitWhileStmt(this);
	}
}
