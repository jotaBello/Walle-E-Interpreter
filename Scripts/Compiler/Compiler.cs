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
		
		Resolver resolver = new Resolver(interpreter);
		resolver.resolve(statements);
		if (hadError) return;
		
		interpreter =new Interpreter();
		interpreter.interpret(statements);
	}
	public static void resolve(string source)
	{
		interpreter = new Interpreter();
		Scanner scanner = new Scanner(source);
		List<Token> tokens = scanner.scanTokens();
		if (hadError) return;

		Parser parser = new Parser(tokens);
		List<Stmt> statements = parser.parse();
		if (hadError) return;

		Resolver resolver = new Resolver(interpreter);
		resolver.resolveLabels(statements);
		resolver.resolve(statements);
	}
	public static void Lexicalerror(string c,int line, string message)
	{
		if (c.Length == 1)
		{
			report(line, $" at '{c}'","Lexical", message);
		}
		else
		{
			report(line, $" at {c}", "Lexical", message);
		}
	}

	public static void SyntacticError(Token token, string message)
	{
		if (token.type == TokenType.EOL)
		{
			report(token.line, " at end of line", "Syntactic",message);
		}
		else
		{
			report(token.line, " at '" + token.lexeme + "'", "Syntactic",message);
		}
	}
	public static void SemanticError(Token token, string message)
	{
		if (token.type == TokenType.EOL)
		{
			report(token.line, " at end of line", "Semantic", message);
		}
		else
		{
			report(token.line, " at '" + token.lexeme + "'", "Semantic", message);
		}
	}

	private static void report(int line, string where, string errorType, string message)
	{

		GD.Print("[line " + line + "] " + errorType + " error" + where + ": " + message);
		//throw new Exception();
		hadError = true;
	}

	public static void runtimeError(RuntimeError error)
	{
		report(error.token.line,"","Runtime",error.message);
		//throw new Exception();
		hadRuntimeError = true;
	}
 
}
