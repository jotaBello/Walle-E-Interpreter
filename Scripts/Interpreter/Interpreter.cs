using System.Collections.Generic;
using Godot;

public static class Interpreter
{
    static bool hadError = false;


    public static void run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();
        Parser parser = new Parser(tokens);

        Expr expression = parser.parse();
        if (hadError) return;
        GD.Print("Parseado");
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
        hadError = true;
    }
}