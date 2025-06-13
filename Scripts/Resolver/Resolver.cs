using System;
using System.Collections.Generic;
using System.Linq;

public class Resolver : Expr.Visitor<object>, Stmt.Visitor
{
	private class ResolverError : Exception { }
	private Interpreter interpreter;
	private Stack<Environment> scopes = new Stack<Environment>();
	int current = 0;

	public Resolver(Interpreter interpreter)
	{
		this.interpreter = interpreter;
	}

	public void resolveLabels(List<Stmt> stmts)
	{
		for (; current < stmts.Count; current++)
		{
			if (stmts[current] is LabelStmt)
			{
				resolve(stmts[current]);
				visitLabelBody((LabelStmt)stmts[current]);
			}
		}
		current = 0;
	}

	public void Resolve(List<Stmt> stmts)
	{
		beginScope();
		resolveLabels(stmts);
		resolveSpawn(stmts);
		
		
		for (; current < stmts.Count; current++)
		{
			if (!(stmts[current] is LabelStmt))
				resolve(stmts[current]);
		}
		endScope();
	}
	public void resolveSpawn(List<Stmt> stmts)
	{
		if (stmts.Count > 0)
		{
			Stmt stmt = stmts[0];
			if (stmt is ExpressionStmt)
			{
				Expr expr = ((ExpressionStmt)stmt).expression;
				if (expr is Call)
				{
					Token name = ((Call)expr).name;
					if (name.lexeme == "Spawn")
					{
						return;
					}
				}
			}
			error(new Token(TokenType.IDENTIFIER,"begin", null,0), "Walle_E must be spawned first");
		}
	}



	public void resolve(List<Stmt> stmts)
	{
		for (; current < stmts.Count; current++)
		{
			if(!(stmts[current] is LabelStmt))
			resolve(stmts[current]);
		}
	}
	private void resolve(Stmt stmt)
	{
		stmt.accept(this);
	}
	private void resolve(Expr expr)
	{
		expr.accept(this);
	}
	private void beginScope()
	{
		scopes.Push(new Environment());
	}
	private void endScope()
	{
		scopes.Pop();
	}
	
	private ResolverError error(Token token, string message)
	{
		Compiler.SemanticError(token, message);
		return new ResolverError();
	}







	public object visitBinaryExpr(Binary expr)
	{
		
		if (expr.left is Literal && expr.right is Literal && expr.operation.type != TokenType.BANG_EQUAL && expr.operation.type != TokenType.EQUAL_EQUAL)
		{
			if (expr.operation.type == TokenType.PLUS)
			{
				object newL = ((Literal)expr.left).value; object newR = ((Literal)expr.right).value;

				if (newL is int && !(newR is int) || !(newL is int) && newR is int)
				{
					error(expr.operation, "Both operands must be numbers or strings.");
				}
			}
			else
			{
				Object left = ((Literal)expr.left).value; Object right = ((Literal)expr.right).value;
				if (!(left is int && right is int || left is string && right is string))
				{
					error(expr.operation, "Both Operands must be numbers.");
				}
			}
		}
		expr.left.accept(this);
		expr.right.accept(this);
		return null;
	}
	public object visitGroupingExpr(Grouping expr)
	{
		expr.expression.accept(this);
		return null;
	}
	public object visitLiteralExpr(Literal expr)
	{
		return null;
	}
	public object visitUnaryExpr(Unary expr)
	{
		if (expr.right is Literal && expr.operation.type is TokenType.MINUS)
		{
			if (!(((Literal)expr.right).value is int))
			{
				error(expr.operation, "Operand must be a number.");
			}
		}
		expr.right.accept(this);
		return null;
	}
	public object visitVariableExpr(Variable expr)
	{
		resolveLocal(expr, expr.name);
		return null;
	}
	private void resolveLocal(Variable expr, Token name)
	{
		for (int i = scopes.Count - 1; i >= 0; i--)
		{
			if (scopes.ElementAt(i).isVariable(name))
			{
				return;
			}
		}
		error(expr.name, "Unassigned variable");
	}

	public object visitLogicalExpr(Logical expr)
	{
		expr.left.accept(this);
		expr.right.accept(this);
		return null;
	}
	public object visitCallExpr(Call expr)
	{
		bool IsFunction = false; bool IsFunctionA = false;
		for (int i = scopes.Count - 1; i >= 0; i--)
		{
			if (scopes.ElementAt(i).IsFunction(expr.name))
			{
				IsFunction = true;
			}

			if (scopes.ElementAt(i).IsFunction(expr.name, expr.Arity))
			{
				IsFunctionA = true;
			}
		}

		if (!IsFunction) error(expr.name, $"This function does'nt exist in this context.");
		else if (!IsFunctionA) error(expr.name, $"This function does'nt take {expr.Arity} arguments.");


		if (interpreter.environment.IsBuiltin(expr.name.lexeme, expr.Arity))
		{
			List<Expr> parameters = expr.parameters;
			switch (expr.name.lexeme)
			{
				case "Spawn":
					ResolveNumbersInFunctions(expr.name, parameters[0], parameters[1]);
					break;
				case "Color":
					ResolveStringInFunctions(expr.name, parameters[0]);
					break;
				case "Size":
					ResolveNumbersInFunctions(expr.name, parameters[0]);
					break;
				case "DrawLine":
					ResolveNumbersInFunctions(expr.name, parameters[0], parameters[1], parameters[2]);
					break;
				case "DrawCircle":
					ResolveNumbersInFunctions(expr.name, parameters[0], parameters[1], parameters[2]);
					break;
				case "DrawRectangle":
					ResolveNumbersInFunctions(expr.name, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
					break;
				case "Fill":
					break;
				case "rand":
					ResolveNumbersInFunctions(expr.name, parameters[0], parameters[1]);
					break;
				case "GetColor":
					ResolveNumbersInFunctions(expr.name, parameters[0], parameters[1]);
					break;
				case "GetActualX":
					break;
				case "GetActualY":
					break;
				case "GetCanvasSize":
					break;
				case "GetColorCount":
					ResolveStringInFunctions(expr.name, parameters[0]);
					ResolveNumbersInFunctions(expr.name, parameters[1], parameters[2], parameters[3], parameters[4]);
					break;
				case "IsBrushColor":
					ResolveStringInFunctions(expr.name, parameters[0]);
					break;
				case "IsBrushSize":
					ResolveNumbersInFunctions(expr.name, parameters[0]);
					break;
				case "IsCanvasColor":
					ResolveStringInFunctions(expr.name, parameters[0]);
					ResolveNumbersInFunctions(expr.name, parameters[1], parameters[2]);
					break;
				case "Move":
					ResolveNumbersInFunctions(expr.name, parameters[0], parameters[1]);
					break;
				default:
					throw new NotImplementedException();
			}
		}
		foreach (var par in expr.parameters)
		{
			resolve(par);
		}
		return null;
	}
	
	private void ResolveStringInFunctions(Token funName, params Expr[] parameters)
	{
		foreach (var par in parameters)
		{
			if (par is Literal)
			{
				if (!(((Literal)par).value is string))
				{
					error(funName, "Argument must be string.");
				}
			}
		}
	}
	private void ResolveNumbersInFunctions(Token funName, params object[] parameters)
	{
		foreach (var par in parameters)
		{
			if (par is Literal)
			{
				if (!(((Literal)par).value is int))
				{
					error(funName, "Arguments must be numbers.");
				}
			}
		}
	}
	


	
	public void VarDeclare(Token name)
	{
		scopes.Peek().define(name.lexeme, null);
	}
	public void FunDeclare(Function stmt)
	{
		try
		{
			scopes.Peek().FunDeclare(stmt);
		}
		catch (RuntimeError e)
		{
			error(stmt.name, e.message);
		}
	}

	public void visitPrintStmt(PrintStmt stmt)
	{
		resolve(stmt.expression);
	}
	public void visitExpressionStmt(ExpressionStmt stmt)
	{
		resolve(stmt.expression);
	}
	public void visitVarStmt(VarStmt stmt)
	{
		VarDeclare(stmt.name);
		resolve(stmt.expression);    
	}
	public void visitBlockStmt(BlockStmt stmt)
	{
		beginScope();
		int temp = current;
		current = 0;
		resolveLabelsBody(stmt);

		foreach (var s in stmt.statements)
		{
			if (!(s is LabelStmt))
			{
				resolve(s);
			}
		}
		current = temp;
		
		endScope();
	}
	void resolveLabelsBody(BlockStmt stmt)
	{
		foreach (var stm in stmt.statements)
		{
			if (stm is LabelStmt)
			{
				visitLabelBody((LabelStmt)stm);
			}
		}
	}


	public void visitIfStmt(IfStmt stmt)
	{
		resolve(stmt.condition);
		resolve(stmt.condition);
		if (stmt.elseBranch != null) resolve(stmt.elseBranch);
	}
	public void visitWhileStmt(WhileStmt stmt)
	{
		resolve(stmt.condition);
		resolve(stmt.body);  
	}
	public void visitFunctionStmt(Function stmt)
	{
		FunDeclare(stmt);

		resolveFunction(stmt);
	}    
	private void resolveFunction(Function function)
	{
		beginScope();
		foreach(Token param in function.parameters)
		{
			VarDeclare(param);
		}
		resolve(function.body);
		endScope();
	}

	public void visitReturnStmt(ReturnStmt stmt)
	{ 
		if (stmt.value != null)
		{
			resolve(stmt.value);
		}
	}
	public void visitLabelStmt(LabelStmt stmt)
	{
		stmt.index = current;
		if (interpreter.environment.islabel(stmt.name))
		{
			error(stmt.name, "label already exist in this context");
		}
		else
		interpreter.environment.labeldefine(stmt.name,stmt.index);
	}
	public void visitLabelBody(LabelStmt stmt)
	{
		stmt.index = current;
		if (scopes.Peek().islabel(stmt.name))
		{
			error(stmt.name, "label already exist in this context");
		}
		else
		scopes.Peek().labeldefine(stmt.name,stmt.index);
	}
	public void visitGoToStmt(GoToStmt stmt)
	{
		resolve(stmt.condition);
		if (!scopes.Peek().islabel(stmt.label))
		{
			error(stmt.label, "label does'nt exist in this context");
		}
	}
}
