using System;
using System.Collections.Generic;
using System.IO;

public class Scanner
{
    private int start = 0;
    private int current = 0;
    private int line = 1;



    private string source;
    private List<Token> tokens = new List<Token>();

    public Scanner(string source)
    {
        this.source = source;
    }

    public List<Token> scanTokens()
    {
        while (!isAtEnd())
        {
            start = current;
            scanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private bool isAtEnd()
    {
        return current >= source.Length;
    }

    private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        {"true",TokenType.AND},
        {"false",TokenType.AND},
        {"GoTo",TokenType.AND},
    };
    

    private void scanToken()
    {
        char c = advance();

        switch (c)
        {
            case '(': addToken(TokenType.LEFT_PAREN); break;
            case ')': addToken(TokenType.RIGHT_PAREN); break;
            case '{': addToken(TokenType.LEFT_BRACE); break;
            case '}': addToken(TokenType.RIGHT_BRACE); break;
            case ',': addToken(TokenType.COMMA); break;
            case '.': addToken(TokenType.DOT); break;
            case '-': addToken(TokenType.MINUS); break;
            case '+': addToken(TokenType.PLUS); break;
            case ';': addToken(TokenType.SEMICOLON); break;
            case '*': addToken(TokenType.STAR); break;
            case '/': addToken(TokenType.SLASH); break;

            case '!':
                addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '|':
                if (match('|'))
                {
                    addToken(TokenType.OR);
                }
                break;

            case ' ':
            case '\t':
            case '\r':
                break;

            case '\n':
                addToken(TokenType.EOL);
                line++;
                break;



            default:
                if (isDigit(c))
                {
                    number();
                }
                else if (isAlpha(c))
                {
                    identifier();
                }
                else
                {
                    //Interpreter.error(line, "Unexpected character.");
                }
                break;
        }
    }

    private char advance()
    {
        current++;
        return source[current - 1];
    }

    private void addToken(TokenType type)
    {
        addToken(type, null);
    }

    private void addToken(TokenType type, Object literal)
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }

    private bool match(char expected)
    {
        if (isAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    private bool isDigit(char c)
    {
        return c >= '0' && c <= '9';
    }
    private void number()
    {
        while (isDigit(peek())) advance();

        addToken(TokenType.NUMBER, Convert.ToInt32(source.Substring(start, current - start)));
    }

    private char peek()
    {
        if (current >= source.Length) return '\0';
        return source[current];
    }

    private void identifier()
    {
        while (isAlphaNumeric(peek())) advance();
        string text = source.Substring(start, current-start);
        TokenType type;
        if (keywords.ContainsKey(text))
        {
            type = keywords[text];
        }
        
        //TEMPORAL
        else if (text == "print")
        {
            type=TokenType.PRINT;
        }

        else
        {
            type = TokenType.IDENTIFIER;
        }
        addToken(type);
    }

    private bool isAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
        (c >= 'A' && c <= 'Z') ||
        c == '_';
    }

    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }
 
}