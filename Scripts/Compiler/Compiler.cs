using System;
using System.Collections.Generic;
using Godot;

public static class Compiler
{
	private static Interpreter interpreter = new Interpreter();
	static bool hadError = false;
	static bool hadRuntimeError = false;


	public static void run(string source)
	{
		Scanner scanner = new Scanner(source);
		List<Token> tokens = scanner.scanTokens();

		Parser parser = new Parser(tokens);
		List<Stmt> statements = parser.parse();

		if (hadError) return;
		if (hadRuntimeError) return;

		interpreter.interpret(statements);
	}

	public static void error(Token token, string message)
	{
		if (token.type == TokenType.EOF)
		{
			report(token.line, " at end", message);
		}
		else
		{
			report(token.line, " at '" + token.lexeme + "'", message);
		}
	}

	private static void report(int line, string where, string message)
	{
		//TEMPORAL
		GD.Print("[line " + line + "] Error" + where + ": " + message);
		throw new Exception();
		//hadError = true;
	}

	public static void runtimeError(RuntimeError error)
	{
		//TEMPORAL
		GD.Print(error.message +
		"\n[line " + error.token.line + "]");
		throw new Exception();
		//hadRuntimeError = true;
	}
 
}
