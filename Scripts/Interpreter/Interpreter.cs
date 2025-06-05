using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using Godot;
public class Interpreter
{
	public Environment globals = new Environment();

	public Interpreter()
	{
		//globals.define("clock", new Clock());
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

			//TEMPORAL
			switch (expr.name.lexeme)
			{
				case "rand":
					Random result = new Random();
					return result.NextDouble();
				case "cos":
					return (double)Math.Cos((double)parameters[0]);
				case "sin":
					return (double)Math.Sin((double)parameters[0]);
				case "exp":
					return (double)Math.Exp((double)parameters[0]);
				case "print":
					Console.WriteLine(parameters[0]);
					return parameters[0];
				case "sqrt":
					return (double)Math.Sqrt((double)parameters[0]);
				case "log":
					return (double)Math.Log((double)parameters[1], (double)parameters[0]);
				default:
					throw new NotImplementedException();
			}
		}

		if (!environment.IsFunction(expr.name))
		{
			Compiler.runtimeError(new RuntimeError(expr.name, "Expected X function."));
			return null;
		}

		if (!environment.IsFunction(expr.name, expr.Arity))
		{
			Compiler.runtimeError(new RuntimeError(expr.name, "incorrect arity for this function."));
			return null;
		}

		List<Token> args = environment.GetParameters(expr.name.lexeme, expr.Arity);
		Environment closure = environment.GetClosure(expr.name.lexeme,expr.Arity);

		Environment environm = new Environment(closure);

		for(int i=0;i<expr.parameters.Count;i++)
		{
			environm.define(args[i].lexeme,evaluate(expr.parameters[i]));
		}
		var body = new BlockStmt(environment.GetBody(expr.name.lexeme, expr.Arity));

		try
		{
			executeBlock(body.statements,environm);
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
				return (int)left + (int)right;
			case TokenType.MOD:
				checkNumberOperand(expr.operation, left, right);
				return (int)left % (int)right;
			case TokenType.POW:
				checkNumberOperand(expr.operation, left, right);
				return Math.Pow((int)left , (int)right);
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
		if (left is int && right is int) return;

		throw new RuntimeError(operation, "Operands must be numbers.");
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
 
 
 


 
} 
