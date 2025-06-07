using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using Godot;
public class Interpreter
{
	public Environment globals = new Environment();

	public Interpreter()
	{
		//
	}


	private Environment environment = new Environment();

	public void interpret(List<Stmt> statements)
	{
		environment = globals;

		try
		{
			foreach (Stmt statement in statements)
			{
				execute(statement);
			}
		}
		catch (RuntimeBinderException)
		{
			//Compiler.runtimeError(error);
		}
	}

	private void execute(Stmt stmt)
	{
		if (stmt != null)
			stmt.accept(this);
	}




	public Object visitLiteralExpr(Literal expr)
	{
		return expr.value;
	}
	public Object visitGroupingExpr(Grouping expr)
	{
		return evaluate(expr.expression);
	}
	private Object evaluate(Expr expr)
	{
		return expr.accept(this);
	}


	public void visitExpressionStmt(ExpressionStmt stmt)
	{
		evaluate(stmt.expression);
	}

	public void visitPrintStmt(PrintStmt stmt)
	{
		Object value = evaluate(stmt.expression);
		//TEMPORAL
		GD.Print(value);
		//Console.WriteLine(value);
	}

	public void visitVarStmt(VarStmt stmt)
	{
		Object value = null;

		if (stmt.expression != null)
		{
			value = evaluate(stmt.expression);
		}

		environment.define(stmt.name.lexeme, value);
	}
	public Object visitVariableExpr(Variable expr)
	{
		return environment.get(expr.name);
	}

	public void visitBlockStmt(BlockStmt stmt)
	{
		executeBlock(stmt.statements, new Environment(environment));
	}

	public void executeBlock(List<Stmt> statements, Environment environment)
	{
		Environment previous = this.environment;
		try
		{
			this.environment = environment;
			foreach (Stmt statement in statements)
			{
				execute(statement);
			}
		}
		finally
		{
			this.environment = previous;
		}
	}

	public void visitIfStmt(IfStmt stmt)
	{
		if (isTruthy(evaluate(stmt.condition)))
		{
			execute(stmt.thenBranch);
		}
		else if (stmt.elseBranch != null)
		{
			execute(stmt.elseBranch);
		}
	}

	public Object visitCallExpr(Call expr)
	{
		if (environment.IsBuiltin(expr.name.lexeme, expr.Arity))
		{
			List<object> parameters = new List<object>();
			foreach (var param in expr.parameters)
			{
				parameters.Add(evaluate(param));
			}

			try
			{
				switch (expr.name.lexeme)
				{
					case "Spawn":
						checkNumbersInFunctions(expr.name, parameters[0], parameters[1]);
						Spawn((int)parameters[0], (int)parameters[1]);
						break;
					case "Color":
						checkStringInFunctions(expr.name, parameters[0]);
						Color((string)parameters[0]);
						break;
					case "Size":
						checkNumbersInFunctions(expr.name, parameters[0]);
						Size((int)parameters[0]);
						break;
					case "DrawLine":
						checkNumbersInFunctions(expr.name, parameters[0], parameters[1], parameters[2]);
						DrawLine((int)parameters[0], (int)parameters[1], (int)parameters[2]);
						break;
					case "DrawCircle":
						checkNumbersInFunctions(expr.name, parameters[0], parameters[1], parameters[2]);
						DrawCircle((int)parameters[0], (int)parameters[1], (int)parameters[2]);
						break;
					case "DrawRectangle":
						checkNumbersInFunctions(expr.name, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
						DrawRectangle((int)parameters[0], (int)parameters[1], (int)parameters[2], (int)parameters[3], (int)parameters[4]);
						break;
					case "Fill":
						Fill();
						break;
					case "rand":
						checkNumbersInFunctions(expr.name, parameters[0], parameters[1]);
						Random random = new Random();
						return random.Next((int)parameters[0], (int)parameters[1]);
					case "GetColor":
						checkNumbersInFunctions(expr.name, parameters[0], parameters[1]);
						return GetColor((int)parameters[0],(int)parameters[1]);
					default:
						throw new NotImplementedException();
				}
				return null;
			}
			catch (RuntimeError error)
			{
				Compiler.runtimeError(new RuntimeError(expr.name, error.message));
			}
		}

		if (!environment.IsFunction(expr.name))
		{
			Compiler.runtimeError(new RuntimeError(expr.name, "Expected function."));
			return null;
		}

		if (!environment.IsFunction(expr.name, expr.Arity))
		{
			Compiler.runtimeError(new RuntimeError(expr.name, "incorrect arity for this function."));
			return null;
		}

		List<Token> args = environment.GetParameters(expr.name.lexeme, expr.Arity);
		Environment closure = environment.GetClosure(expr.name.lexeme, expr.Arity);

		Environment environm = new Environment(closure);

		for (int i = 0; i < expr.parameters.Count; i++)
		{
			environm.define(args[i].lexeme, evaluate(expr.parameters[i]));
		}
		var body = new BlockStmt(environment.GetBody(expr.name.lexeme, expr.Arity));

		try
		{
			executeBlock(body.statements, environm);
		}
		catch (Return ret)
		{
			return ret.value;
		}

		return null;
	}


	public Object visitUnaryExpr(Unary expr)
	{
		Object right = evaluate(expr.right);
		switch (expr.operation.type)
		{
			case TokenType.MINUS:
				checkNumberOperand(expr.operation, right);
				return -(int)right;
			case TokenType.BANG:
				return !isTruthy(right);
		}

		return null;
	}

	public Object visitBinaryExpr(Binary expr)
	{
		Object left = evaluate(expr.left);
		Object right = evaluate(expr.right);
		switch (expr.operation.type)
		{
			case TokenType.GREATER:
				checkNumberOperand(expr.operation, left, right);
				return (int)left > (int)right;
			case TokenType.GREATER_EQUAL:
				checkNumberOperand(expr.operation, left, right);
				return (int)left >= (int)right;
			case TokenType.LESS:
				checkNumberOperand(expr.operation, left, right);
				return (int)left < (int)right;
			case TokenType.LESS_EQUAL:
				checkNumberOperand(expr.operation, left, right);
				return (int)left <= (int)right;

			case TokenType.BANG_EQUAL: return !isEqual(left, right);
			case TokenType.EQUAL_EQUAL: return isEqual(left, right);


			case TokenType.MINUS:
				checkNumberOperand(expr.operation, left, right);
				return (int)left - (int)right;
			case TokenType.SLASH:
				checkNumberOperand(expr.operation, left, right);
				return (int)left / (int)right;
			case TokenType.STAR:
				checkNumberOperand(expr.operation, left, right);
				return (int)left * (int)right;
			case TokenType.PLUS:
				checkNumberOperand(expr.operation, left, right);
				if (left is int) return (int)left + (int)right;
				return (string)left + (string)right;
			case TokenType.MOD:
				checkNumberOperand(expr.operation, left, right);
				return (int)left % (int)right;
			case TokenType.POW:
				checkNumberOperand(expr.operation, left, right);
				return Math.Pow((int)left, (int)right);
		}


		return null;
	}





	private bool isEqual(Object a, Object b)
	{
		if (a == null && b == null) return true;
		if (a == null || b == null) return false;
		return a.Equals(b);
	}


	private bool isTruthy(Object objec)
	{
		if (objec == null) return false;
		if (objec is bool) return (bool)objec;
		return true;
	}

	private void checkNumberOperand(Token operation, Object operand)
	{
		if (operand is int) return;
		throw new RuntimeError(operation, "Operand must be a number.");
	}
	private void checkNumberOperand(Token operation, Object left, Object right)
	{
		if (left is int && right is int || left is string && right is string) return;

		throw new RuntimeError(operation, "Both operands must be numbers or strings.");
	}
	private void checkNumbersInFunctions(Token funName, params Object[] parameters)
	{
		foreach (Object obj in parameters)
			if (!(obj is int))
				throw new RuntimeError(funName, "Arguments must be numbers.");
	}
	private void checkStringInFunctions(Token funName, params Object[] parameters)
	{
		foreach (Object obj in parameters)
			if (!(obj is string))
				throw new RuntimeError(funName, "Argument must be string.");
	}


	public Object visitLogicalExpr(Logical expr)
	{
		Object left = evaluate(expr.left);
		if (expr.operation.type == TokenType.OR)
		{
			if (isTruthy(left)) return left;
		}
		else
		{
			if (!isTruthy(left)) return left;
		}
		return evaluate(expr.right);
	}

	public void visitWhileStmt(WhileStmt stmt)
	{
		while (isTruthy(evaluate(stmt.condition)))
		{
			execute(stmt.body);
		}
	}

	public void visitFunctionStmt(Function stmt)
	{
		environment.FunDeclare(stmt);
	}

	public void visitReturnStmt(ReturnStmt stmt)
	{
		Object value = null;
		if (stmt.value != null) value = evaluate(stmt.value);
		throw new Return(value);
	}

	void Spawn(int x, int y)
	{
		Paint.Spawn(x, y);
	}
	void Color(string color)
	{
		Paint.Color(color);
	}
	void Size(int k)
	{
		Paint.Size(k);
	}
	void DrawLine(int dirX, int dirY, int distance)
	{
		Paint.DrawLine(dirX, dirY, distance);
	}
	void DrawCircle(int dirX, int dirY, int radius)
	{
		Paint.DrawCircle(dirX, dirY, radius);
	}
	void DrawRectangle(int dirX, int dirY, int distance, int width, int height)

	{
		Paint.DrawRectangle(dirX, dirY, distance, width, height);
	}
	void Fill()
	{
		Paint.Fill();
	}
	string GetColor(int x,int y)
	{
		return Paint.GetColor(x,y);
	}
	
 
 
 


 
} 
