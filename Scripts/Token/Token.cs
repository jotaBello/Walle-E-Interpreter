using System;

public class Token
{
    public TokenType type { get;}
    public string lexeme;
    public Object literal;
    public int line;

    public Token(TokenType type, string lexeme, Object literal, int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }

    public string toString()
    {
        return type + " " + lexeme + " " + literal;
    }

}

public enum TokenType
{
    //Single Character Tokens
    LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE,
    RIGHT_BRACE,LEFT_COR,RIGHT_COR, COMMA, DOT, MINUS, PLUS,
    SEMICOLON, SLASH, STAR,MOD,POW,

    //One or Two Character Tokens
    BANG, BANG_EQUAL,
    EQUAL, EQUAL_EQUAL, GREATER,
    GREATER_EQUAL, LESS,
    LESS_EQUAL,

    //Literals
    IDENTIFIER, NUMBER,STRING,

    //Keywords
    AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
    PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE, GOTO,

    EOL,
    EOF
}