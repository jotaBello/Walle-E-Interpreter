using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
public class Parser
{
    private class ParseError : RuntimeBinderException {}
    private List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public Expr parse()
    {
        try
        {
            return expression();
        }
        catch (ParseError error)
        {
            return null;
        }
    }



    private Expr expression()
    {
        return equality();
    }

    private Expr equality()
    {
        Expr expr = comparison();
        while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
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

        throw error(peek(), "Expect expression.");
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

    private ParseError error(Token token, string message)
    {
        Interpreter.error(token, message);
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