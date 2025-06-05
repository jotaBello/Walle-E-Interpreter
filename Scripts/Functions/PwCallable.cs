/*using System;
using System.Collections.Generic;
interface PwCallable
{
    int arity();
    Object call(Interpreter interpreter, List<Object> arguments);
}

public class Clock : PwCallable
{
    public int arity()
    {
        return 0;
    }
    public Object call(Interpreter interpreter, List<Object> arguments)
    {
        return null;
    }
}

public class PwFunction : PwCallable
{
    private Function declaration;
    private Environment closure;

    public PwFunction(Function declaration, Environment closure)
    {
        this.declaration = declaration;
        this.closure = closure;
    }

    public Object call(Interpreter interpreter, List<Object> arguments)
    {
        Environment environment = new Environment(closure);
        for (int i = 0; i < declaration.parameters.Count; i++)
        {
            environment.define(declaration.parameters[i].lexeme, arguments[i]);
        }
        try
        {
            interpreter.executeBlock(declaration.body, environment);
        }
        catch (Return returnValue)
        {
            return returnValue.value;
        }
        return null;
    }
    public int arity()
    {
    return declaration.parameters.Count;
    }

}
*/