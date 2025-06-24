using System.Collections.Generic;

public static class Compiler
{
	private static Interpreter interpreter;
	public static bool hadError = false;
	public static bool hadRuntimeError = false;
	private static List<string> errors = new List<string>();


	public static void run(string source)
	{
		LogReporter.CleanLog();
		errors = new List<string>();
		interpreter = new Interpreter();

		Scanner scanner = new Scanner(source);
		List<Token> tokens = scanner.scanTokens();
		if (hadError) return;

		Parser parser = new Parser(tokens);
		List<Stmt> statements = parser.parse();
		if (hadError) return;

		Resolver resolver = new Resolver(interpreter);
		resolver.Resolve(statements);
		if (hadError) return;

		interpreter.interpret(statements);
	}
	public static void resolve(string source)
	{
		LogReporter.CleanLog();
		errors = new List<string>();

		interpreter = new Interpreter();
		Scanner scanner = new Scanner(source);
		List<Token> tokens = scanner.scanTokens();
		if (hadError) return;

		Parser parser = new Parser(tokens);
		List<Stmt> statements = parser.parse();
		if (hadError) return;

		Resolver resolver = new Resolver(interpreter);
		resolver.Resolve(statements);
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

		LogReporter.LogMessage("[line " + line + "] " + errorType + " error" + where + ": " + message );
		//throw new Exception();
		hadError = true;
	}

	public static void runtimeError(RuntimeError error)
	{
		report(error.token.line," at " +error.token.lexeme,"Runtime",error.message);
		hadRuntimeError = true;
		//throw new Exception();
	}
 
}
