using Microsoft.CSharp.RuntimeBinder;

public class RuntimeError : RuntimeBinderException
{
    public Token token;
    public string message;
    public RuntimeError(Token token, string message)
    {
        this.message = message;
        this.token = token;
    }
}