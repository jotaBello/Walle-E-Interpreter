using System.Collections.Generic;
public abstract class Stmt
{
	virtual public void accept(Visitor visitor)
	{
		//TEMPORAL
	}

	public interface Visitor
	{
		public void visitPrintStmt(PrintStmt stmt);
		public void visitExpressionStmt(ExpressionStmt stmt);
		public void visitVarStmt(VarStmt stmt);
		public void visitBlockStmt(BlockStmt stmt);
		public void visitIfStmt(IfStmt stmt);
		public void visitWhileStmt(WhileStmt stmt);
		public void visitFunctionStmt(Function stmt);
		public void visitReturnStmt(ReturnStmt stmt);
		public void visitLabelStmt(LabelStmt stmt);
		public void visitGoToStmt(GoToStmt stmt);
	}
}

public class PrintStmt : Stmt
{
	public Expr expression;

	public PrintStmt(Expr expression)
	{
		this.expression = expression;
	}
	override public void accept(Visitor visitor)
	{
		visitor.visitPrintStmt(this);
	}

}
public class ExpressionStmt : Stmt
{
	public Expr expression;

	public ExpressionStmt(Expr expression)
	{
		this.expression = expression;
	}
	override public void accept(Visitor visitor)
	{
		visitor.visitExpressionStmt(this);
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
	override public void accept(Visitor visitor)
	{
		visitor.visitVarStmt(this);
	}
}

public class BlockStmt : Stmt
{
	public List<Stmt> statements;

	public BlockStmt(List<Stmt> statements)
	{
		this.statements = statements;
	}
	override public void accept(Visitor visitor)
	{
		visitor.visitBlockStmt(this);
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
	override public void accept(Visitor visitor)
	{
		visitor.visitIfStmt(this);
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
	override public void accept(Visitor visitor)
	{
		visitor.visitWhileStmt(this);
	}
}

public class Function : Stmt
{
	public Token name;
	public List<Token> parameters;
	public List<Stmt> body;
	public Environment closure;
	public int Arity;
	public Function(Token name, List<Token> parameters, List<Stmt> body)
	{
		this.name = name;
		this.parameters = parameters;
		this.body = body;
		Arity =parameters.Count;
	}
	public Function(Token name, List<Token> parameters, List<Stmt> body, Environment closure)
	{
		this.name = name;
		this.parameters = parameters;
		this.body = body;
		this.closure = closure;
		Arity = parameters.Count;
	}
	override public void accept(Visitor visitor)
	{
		visitor.visitFunctionStmt(this);
	}
}
public class ReturnStmt : Stmt
{
	public Token keyword;
	public Expr value;

	public ReturnStmt(Token keyword, Expr value)
	{
		this.keyword = keyword;
		this.value = value;
	}
	override public void accept(Visitor visitor)
	{
		visitor.visitReturnStmt(this);
	}

}
public class LabelStmt : Stmt
{
	public Token name;
	public int index;

	public LabelStmt(Token name)
	{
		this.name = name;
	}
	override public void accept(Visitor visitor)
	{
		visitor.visitLabelStmt(this);
	}

}
public class GoToStmt : Stmt
{
	public int loopCount = 0;
	public Token label;

	public Expr condition;

	public GoToStmt(Token label, Expr condition)
	{
		this.label = label;
		this.condition = condition;
	}
	override public void accept(Visitor visitor)
	{
		visitor.visitGoToStmt(this);
	}

}
