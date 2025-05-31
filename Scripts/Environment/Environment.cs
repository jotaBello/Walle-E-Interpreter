using Godot;
using System;
using System.Collections.Generic;
public class Environment
{
    Environment enclosing;
    private Dictionary<string, Object> values = new Dictionary<string, Object>();

    public Environment()
    {
        enclosing = null;
    }
    public Environment(Environment enclosing)
    {
        this.enclosing=enclosing;
    }

    public void define(String name, Object value)
    {
        if (values.ContainsKey(name))
        {
            values[name] = value;
        }
        else
        {
            values.Add(name, value);
        }

        /*if (enclosing != null)
        {
            enclosing.define(name, value);
            return;
        }*/
    }

    public void assign()
    {
        
    }

    public Object get(Token name)
    {
        if (values.ContainsKey(name.lexeme))
        {
            return values[name.lexeme];
        }
        else
        {
            if (enclosing != null) return enclosing.get(name);
        }

        throw new RuntimeError(name, "UUndefined variable '" + name.lexeme + "'.");
    }  
}