using System;
using System.Collections.Generic;
public class Interpreter : Expr.Visitor<object>, Stmt.Visitor
{
	public Environment environment = new Environment();

	private int current = 0;

	public void interpret(List<Stmt> statements)
	{
		try
		{
			current = 0;

			for (; current < statements.Count; current++)
			{
				if (!(statements[current] is LabelStmt) && !Compiler.hadRuntimeError)
				{
					execute(statements[current]);
				}
			}

		}
		catch (RuntimeError error)
		{
			Compiler.runtimeError(error);
		}
	}

	private void execute(Stmt stmt)
	{
		stmt.accept(this);
	}



	public object visitLiteralExpr(Literal expr)
	{
		return expr.value;
	}
	public object visitGroupingExpr(Grouping expr)
	{
		return evaluate(expr.expression);
	}
	private object evaluate(Expr expr)
	{
		return expr.accept(this);
	}
	public void visitExpressionStmt(ExpressionStmt stmt)
	{
		evaluate(stmt.expression);
	}

	public void visitPrintStmt(PrintStmt stmt)
	{
		object value = evaluate(stmt.expression);
		if (value != null) LogReporter.LogMessage(value.ToString());
		else LogReporter.LogMessage("null");
	}

	public void visitVarStmt(VarStmt stmt)
	{
		object value = null;

		if (stmt.expression != null)
		{
			value = evaluate(stmt.expression);
		}

		environment.define(stmt.name.lexeme, value);
	}
	public object visitVariableExpr(Variable expr)
	{
		return environment.get(expr.name);
	}

	public void visitBlockStmt(BlockStmt stmt)
	{
		executeBlock(stmt.statements, new Environment(environment));
	}
	public void visitLabelStmt(LabelStmt stmt)
	{
		stmt.index = current;
		environment.labeldefine(stmt.name, stmt.index);
	}
	public void visitGoToStmt(GoToStmt stmt)
	{
		if (stmt.loopCount > 10000) throw new RuntimeError(new Token(TokenType.GOTO, "GoTo", null, current+1),"You entered in an infinite loop");
		if (isTruthy(evaluate(stmt.condition)))
		{
			stmt.loopCount++;
			current = environment.labelget(stmt.label);
		}
	}

	public void executeBlock(List<Stmt> statements, Environment environment)
	{
		int temp = current;
		Environment previous = this.environment;
		try
		{
			this.environment = environment;

			
			current = 0;
			ResolveLabels(statements);

			for (; current < statements.Count; current++)
			{
				if (!(statements[current] is LabelStmt))
				{
					execute(statements[current]);
				}
			}

			current = temp;
		}
		finally
		{
			current = temp;
			this.environment = previous;
		}
	}

	public void ResolveLabels(List<Stmt> stmts)
	{
		foreach (var stmt in stmts)
		{
			if (stmt is LabelStmt)
			{
				execute(stmt);
			}
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

	public object visitCallExpr(Call expr)
	{
		if (expr.loopCount > 100000) throw new RuntimeError(expr.name, "You entered in an infinite recursive loop");

		expr.loopCount++;


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
						return random.Next((int)parameters[0], (int)parameters[1] + 1);
					case "GetColor":
						checkNumbersInFunctions(expr.name, parameters[0], parameters[1]);
						return GetColor((int)parameters[0], (int)parameters[1]);
					case "GetActualX":
						return GetActualX();
					case "GetActualY":
						return GetActualY();
					case "GetCanvasSize":
						return GetCanvasSize();
					case "GetColorCount":
						checkStringInFunctions(expr.name, parameters[0]);
						checkNumbersInFunctions(expr.name, parameters[1], parameters[2], parameters[3], parameters[4]);
						return GetColorCount((string)parameters[0], (int)parameters[1], (int)parameters[2], (int)parameters[3], (int)parameters[4]);
					case "IsBrushColor":
						checkStringInFunctions(expr.name, parameters[0]);
						return IsBrushColor((string)parameters[0]);
					case "IsBrushSize":
						checkNumbersInFunctions(expr.name, parameters[0]);
						return IsBrushSize((int)parameters[0]);
					case "IsCanvasColor":
						checkStringInFunctions(expr.name, parameters[0]);
						checkNumbersInFunctions(expr.name, parameters[1], parameters[2]);
						return IsCanvasColor((string)parameters[0], (int)parameters[1], (int)parameters[2]);
					case "Move":
						checkNumbersInFunctions(expr.name, parameters[0], parameters[1]);
						Move((int)parameters[0], (int)parameters[1]);
						break;
					default:
						throw new NotImplementedException();
				}
				return null;
			}
			catch (RuntimeError error)
			{
				Compiler.runtimeError(new RuntimeError(expr.name, error.message));
			}

			return null;
		}

		if (!environment.IsFunction(expr.name))
		{
			Compiler.runtimeError(new RuntimeError(expr.name, $"{expr.name.lexeme} function doesn't exist in this context."));
			return null;
		}

		if (!environment.IsFunction(expr.name, expr.Arity))
		{
			Compiler.runtimeError(new RuntimeError(expr.name, $"{expr.name.lexeme} function doesn't take {expr.Arity} arguments."));
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


	public object visitUnaryExpr(Unary expr)
	{
		object right = evaluate(expr.right);
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

	public object visitBinaryExpr(Binary expr)
	{
		object left = evaluate(expr.left);
		object right = evaluate(expr.right);
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
				if ((int)right == 0) throw new RuntimeError(expr.operation, "You can't divide by zero.");
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





	private bool isEqual(object a, object b)
	{
		if (a == null && b == null) return true;
		if (a == null || b == null) return false;
		return a.Equals(b);
	}


	private bool isTruthy(object objec)
	{
		if (objec == null) return false;
		if (objec is bool) return (bool)objec;
		if (objec is int) return (int)objec > 0;
		return true;
	}

	private void checkNumberOperand(Token operation, object operand)
	{
		if (operand is int) return;
		throw new RuntimeError(operation, "Operand must be a number.");
	}
	private void checkNumberOperand(Token operation, object left, object right)
	{
		if (left is int && right is int || left is string && right is string) return;

		throw new RuntimeError(operation, "Both operands must be numbers or strings.");
	}
	private void checkNumbersInFunctions(Token funName, params object[] parameters)
	{
		foreach (object obj in parameters)
			if (!(obj is int))
				throw new RuntimeError(funName, "Arguments must be numbers.");
	}
	private void checkStringInFunctions(Token funName, params object[] parameters)
	{
		foreach (object obj in parameters)
			if (!(obj is string))
				throw new RuntimeError(funName, "Argument must be string.");
	}


	public object visitLogicalExpr(Logical expr)
	{
		object left = evaluate(expr.left);
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
		int count = 10000;
		while (isTruthy(evaluate(stmt.condition)))
		{
			execute(stmt.body);
			count--;
			if (count < 0) throw new RuntimeError(new Token(TokenType.WHILE,"while",null,current+1), "You entered in an infinite loop");
		}
	}

	public void visitFunctionStmt(Function stmt)
	{
		environment.FunDeclare(stmt);
	}

	public void visitReturnStmt(ReturnStmt stmt)
	{
		object value = null;
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
	string GetColor(int x, int y)
	{
		return Paint.GetColor(x, y);
	}
	public static int GetActualX()
	{
		return Paint.GetActualX();
	}
	public static int GetActualY()
	{
		return Paint.GetActualY();
	}
	public static int GetCanvasSize()
	{
		return Paint.GetCanvasSize();
	}
	public static int GetColorCount(string color, int x1, int y1, int x2, int y2)
	{
		return Paint.GetColorCount(color, x1, y1, x2, y2);
	}
	public static int IsBrushColor(string color)
	{
		return Paint.IsBrushColor(color);
	}
	public static int IsBrushSize(int size)
	{
		return Paint.IsBrushSize(size);
	}
	public static int IsCanvasColor(string color, int vertical, int horizontal)
	{
		return Paint.IsCanvasColor(color, vertical, horizontal);
	}
	public static void Move(int x, int y)
	{
		Paint.Move(x,y);
	}
	
 
 
 


 
} 
