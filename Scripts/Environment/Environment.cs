
using System;
using System.Collections.Generic;
public class Environment
{
	Environment enclosing;
	private Dictionary<string, Object> values = new Dictionary<string, Object>();
	private Dictionary<string, Dictionary<int, Function>> funGlobal = new Dictionary<string, Dictionary<int, Function>>();
	private HashSet<(string, int)> builtins = new HashSet<(string, int)>()
  {
	("rand", 0),
	("cos", 1),
	("exp", 1),
	("print", 1),
	("sin", 1),
	("sqrt", 1),
	("log", 2)
  };

	public Environment()
	{
		enclosing = null;
	}
	public Environment(Environment enclosing)
	{
		this.enclosing = enclosing;
	}

	public void FunDeclare(Function fun)
	{
		fun = new Function(fun.name, fun.parameters, fun.body, this);

		string name = fun.name.lexeme;
		int arity = fun.Arity;
		if (IsBuiltin(name, arity))
		{
			Compiler.runtimeError(new RuntimeError(fun.name, "is a built-in function."));
			return;
		}

		if (funGlobal.ContainsKey(name))
		{
			Dictionary<int, Function> table = funGlobal[name];
			if (table.ContainsKey(arity))
			{
				Compiler.runtimeError(new RuntimeError(fun.name, "already exists."));
				return;
			}
			else
			{
				funGlobal[name].Add(arity, fun);
			}
		}
		else
		{
			Dictionary<int, Function> table = new Dictionary<int, Function>
			{
				{ arity, fun }
			};
			funGlobal.Add(name, table);
		}
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

		if (enclosing != null)
		{
			enclosing.define(name, value);
			return;
		}
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

		throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
	}

	public bool IsFunction(Token name)
	{
		if (funGlobal.ContainsKey(name.lexeme))
		{
			return true;
		}
		else if (enclosing != null)
		{
			return enclosing.IsFunction(name);
		}
		return false;
	}
	public bool IsFunction(Token name, int arity)
	{
		if (funGlobal.ContainsKey(name.lexeme))
		{
			return funGlobal[name.lexeme].ContainsKey(arity);
		}
		else if (enclosing != null)
		{
			return enclosing.IsFunction(name,arity);
		}
		return false;
	}

	public bool IsBuiltin(string name, int arity)
	{
		return builtins.Contains((name, arity));
	}
	public List<Stmt> GetBody(string name, int arity)
	{
		if (funGlobal.ContainsKey(name) && funGlobal[name].ContainsKey(arity))
			return funGlobal[name][arity].body;
		else
			return enclosing.GetBody(name, arity);
	}
	public List<Token> GetParameters(string name, int arity)
	{
		if (funGlobal.ContainsKey(name) && funGlobal[name].ContainsKey(arity))
			return funGlobal[name][arity].parameters;
		else
			return enclosing.GetParameters(name, arity);
	}
	public Environment GetClosure(string name, int arity)
	{
		if (funGlobal.ContainsKey(name) && funGlobal[name].ContainsKey(arity))
			return funGlobal[name][arity].closure;
		else
			return enclosing.GetClosure(name, arity);
	}
}
