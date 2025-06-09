using System;
using System.Collections.Generic;
using Godot;

public static class Compiler
{
	private static Interpreter interpreter;
	public static bool hadError = false;
	public static bool hadRuntimeError = false;


	public static void run(string source)
	{
		Scanner scanner = new Scanner(source);
		List<Token> tokens = scanner.scanTokens();
		if (hadError) return;

		Parser parser = new Parser(tokens);
		List<Stmt> statements = parser.parse();
		if (hadError) return;
		
		interpreter=new Interpreter();
		interpreter.interpret(statements);
	}
	public static void resolve(string source)
	{
		/*interpreter = new Interpreter();
		Scanner scanner = new Scanner(source);
		List<Token> tokens = scanner.scanTokens();

		Parser parser = new Parser(tokens);
		List<Stmt> statements = parser.parse();
		if (hadError) return;
		
		///*/
	}

	public static void error(Token token, string message)
	{
		if (token.type == TokenType.EOF)
		{
			report(token.line, " at end", message);
		}
		else if (token.type == TokenType.EOL)
		{
			report(token.line, " at end of line", message);
		}
		else
		{
			report(token.line, " at '" + token.lexeme + "'", message);
		}
	}

	private static void report(int line, string where, string message)
	{

		GD.Print("[line " + line + "] Error" + where + ": " + message);
		//throw new Exception();
		hadError = true;
	}

	public static void runtimeError(RuntimeError error)
	{
		GD.Print(error.message +
		"\n[line " + error.token.line + "]");
		//throw new Exception();
		hadRuntimeError = true;
	}
 
}
