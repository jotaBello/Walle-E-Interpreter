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

        tokens.Add(new Token(TokenType.EOL, "", null, line));
        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private bool isAtEnd()
    {
        return current >= source.Length;
    }

    private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        {"true",TokenType.TRUE},
        {"false",TokenType.FALSE},
        {"GoTo",TokenType.GOTO},
        {"while",TokenType.WHILE},
        {"for",TokenType.FOR},
        {"return",TokenType.RETURN},
        {"fun",TokenType.FUN},
        {"print",TokenType.PRINT},
        {"if",TokenType.IF},
        {"else",TokenType.ELSE}
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
            case '[': addToken(TokenType.LEFT_COR); break;
            case ']': addToken(TokenType.RIGHT_COR); break;
            case ',': addToken(TokenType.COMMA); break;
            case '.': addToken(TokenType.DOT); break;
            case '-': addToken(TokenType.MINUS); break;
            case '+': addToken(TokenType.PLUS); break;
            case ';': addToken(TokenType.SEMICOLON); break;
            case '/': addToken(TokenType.SLASH); break;
            case '%': addToken(TokenType.MOD); break;

            case '*':
                addToken(match('*') ? TokenType.POW : TokenType.STAR);
                break;
            case '!':
                addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                if (match('='))
                {
                    addToken(TokenType.LESS_EQUAL);
                }
                else if (match('-'))
                {
                    addToken(TokenType.VAR);
                }
                else
                {
                     addToken(TokenType.LESS);
                }
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
                case '&':
                if (match('&'))
                {
                    addToken(TokenType.AND);
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

            case '"': sstring(); break;

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
                    Compiler.Lexicalerror(c.ToString(),line, "Unexpected character.");
                }
                break;
        }
    }

    private void sstring() 
    {
        while (peek() != '"' && !isAtEnd())
        {
            if (peek() == '\n') line++;
            advance();
        }
        if (isAtEnd())
        {
            Compiler.Lexicalerror("end",line, "Unfinished string.");
            return;
        }

        advance();
        string value = source.Substring(start +1 , current - start-2);
        addToken(TokenType.STRING, value);
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