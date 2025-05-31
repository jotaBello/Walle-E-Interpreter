using System.Collections.Generic;
using Godot;
using Microsoft.CSharp.RuntimeBinder;
public class Parser
{
	private class ParseError : RuntimeBinderException { }
	private List<Token> tokens;
	private int current = 0;

	public Parser(List<Token> tokens)
	{
		this.tokens = tokens;
	}

	public List<Stmt> parse()
	{
		List<Stmt> statements = new List<Stmt>();  
		while (!isAtEnd())
		{
			SkipEOL();if (isAtEnd()) return statements;
			statements.Add(declaration());
		}
		return statements;
	}

	private Stmt statement()
	{
		if (match(TokenType.PRINT)) return printStatement();
		if (match(TokenType.LEFT_BRACE)) return new BlockStmt(block());
		return expressionStatement();
	}

	private Stmt printStatement()
	{
		Expr value = expression();
		consume("Expect EOL or EOF after value.",TokenType.EOL,TokenType.EOF);
		return new PrintStmt(value);
	}

	private Stmt expressionStatement()
	{
		Expr expr = expression();
		consume( "Expect EOL or EOF after expression.",TokenType.EOL,TokenType.EOF);

		return new ExpressionStmt(expr);
	}

	private Stmt varDeclaration()
	{
		Back(); Back();
		Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");
		advance();
		Expr initializer = null;

		initializer = expression();

		consume("Expect EOL or EOF after variable declaration.",TokenType.EOL,TokenType.EOF);
		return new VarStmt(name, initializer);
	}


	private Stmt declaration()
	{
		try
		{
			if (match(TokenType.IDENTIFIER) && match(TokenType.VAR)) return varDeclaration();
			return statement();
		}
		catch (ParseError)
		{
			synchronize();
			return null;
		}
	}

	 private List<Stmt> block()
	{
		List<Stmt> statements = new List<Stmt>();
		while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
		{
			SkipEOL();
			if(peek().type!=TokenType.RIGHT_BRACE)
			statements.Add(declaration());
		}

		consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
		return statements;
	}







	private Expr expression()
	{
		return assignment();
	}

	private Expr assignment() {
		Expr expr = equality();
		if (match(TokenType.VAR)) {
			Token var = previous();
			Expr value = assignment();
			if (expr is Variable) {
				Token name = ((Variable)expr).name;
				//TEMPORAL
				return new Variable(name);
			}
			error(var, "Invalid assignment target.");
		}
		return expr;
	}

	private Expr equality()
	{
		Expr expr = comparison();
		while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
		{
			Token operation = previous();
			Expr right = comparison();
			expr = new Binary(expr, operation, right);
		}
		return expr;
		
	}
	

	private Expr comparison()
	{
		Expr expr = term();
		while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
		{
			Token operation = previous();
			Expr right = term();
			expr = new Binary(expr, operation, right);
		}
		return expr;
	}
	
	private Expr term()
	{
		Expr expr = factor();
		while (match(TokenType.MINUS, TokenType.PLUS))
		{
			Token operation = previous();
			Expr right = factor();
			expr = new Binary(expr, operation, right);
		}  
		return expr;
	}

	private Expr factor()
	{
		Expr expr = unary();
		while (match(TokenType.SLASH, TokenType.STAR))
		{
			Token operation = previous();
			Expr right = unary();
		 expr = new Binary(expr, operation, right);
		}
		return expr;
	}

	private Expr unary()
	{
		if (match(TokenType.BANG, TokenType.MINUS))
		{
			Token operation = previous();
			Expr right = unary();
			return new Unary(operation, right);
		}
		return primary();
	}

	private Expr primary()
	{
		if (match(TokenType.FALSE)) return new Literal(false);
		if (match(TokenType.TRUE)) return new Literal(true);
		if (match(TokenType.NIL)) return new Literal(null);
		if (match(TokenType.NUMBER, TokenType.STRING))
		{
			return new Literal(previous().literal);
		}
		if (match(TokenType.LEFT_PAREN))
		{
			Expr expr = expression();
			consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
			return new Grouping(expr);
		}
		if (match(TokenType.IDENTIFIER))
		{
		 return new Variable(previous());
		}

		throw error(peek(), "Expect expression.");
 }


	private void synchronize()
	{
		advance();
		while (!isAtEnd())
		{
			if (previous().type == TokenType.EOL) return;
			switch (peek().type)
			{
				case TokenType.FUN:
				case TokenType.VAR:
				case TokenType.FOR:
				case TokenType.IF:
				case TokenType.WHILE:
				case TokenType.PRINT:
				case TokenType.RETURN:
					return;
			}
			advance();
		}
	}

	private void SkipEOL()
	{
		while(match(TokenType.EOL)){};
	}







	private bool match(params TokenType[] types)
	{
		foreach (TokenType type in types)
		{
			if (check(type))
			{
				advance();
				return true;
			}
		}
		return false;
	}

	private Token consume(TokenType type, string message)
	{
		if (check(type)) return advance();
		throw error(peek(), message);
	}
	private Token consume(string message, params TokenType[] types)
	{
		foreach (var type in types)
		{
			if (peek().type==type) return advance();
		}
		throw error(peek(), message);
	}

	private ParseError error(Token token, string message)
	{
		Compiler.error(token, message);
		return new ParseError();
	}

	private bool check(TokenType type)
	{
		if (isAtEnd()) return false;
		return peek().type == type;
	}

	private Token advance()
	{
		if (!isAtEnd()) current++;
		return previous();
	}

	private void Back()
	{
		if (current > 0) current--;
	}

	
	private bool isAtEnd()
	{
		return peek().type == TokenType.EOF;
	}

	private Token peek()
	{
		return tokens[current];
	}

	private Token previous()
	{
		return tokens[current - 1];
	}
 
 
 
 
 
}
