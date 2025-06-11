
using System;
using System.Collections.Generic;
public class Environment
{
	Environment enclosing;
	private Dictionary<string, object> variables = new Dictionary<string, object>();
	private Dictionary<string, int> labels = new Dictionary<string, int>();
	private Dictionary<string, Dictionary<int, Function>> funGlobal = new Dictionary<string, Dictionary<int, Function>>();
	private Dictionary<string, int> builtins = new Dictionary<string, int>()
	{
		{"Spawn", 2},
		{"Color", 1 },
		{"Size", 1 },
		{"DrawLine", 3 },
		{"DrawCircle", 3 },
		{"DrawRectangle", 5 },
		{"Fill", 0},
		{"rand",2},
		{"GetColor",2},
		{"GetActualX",0},
		{"GetActualY",0},
		{"GetCanvasSize",0},
		{"GetColorCount",5 },
		{"IsBrushColor",1},
		{"IsBrushSize",1},	
		{"IsCanvasColor",3},	
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
			throw new RuntimeError(fun.name, "is a built-in function.");
		}

		if (funGlobal.ContainsKey(name))
		{
			Dictionary<int, Function> table = funGlobal[name];
			if (table.ContainsKey(arity))
			{
				throw new RuntimeError(fun.name, "already exists.");
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
		if (variables.ContainsKey(name))
		{
			variables[name] = value;
		}
		else
		{
			variables.Add(name, value);
		}

		if (enclosing != null)
		{
			enclosing.define(name, value);
			return;
		}
	}
	public void labeldefine(Token name,int index)
	{
		if (labels.ContainsKey(name.lexeme))
		{
			throw new RuntimeError(name, " label already exists.");
		}
		else
		{
			labels.Add(name.lexeme,index);
		}
	}
	public int labelget(Token name)
	{
		if (!labels.ContainsKey(name.lexeme))
		{
			throw new RuntimeError(name, " label does'nt exists.");
		}

		return labels[name.lexeme];
	}
	public bool islabel (Token name)
	{
		return labels.ContainsKey(name.lexeme);
	}

	public Object get(Token name)
	{
		if (variables.ContainsKey(name.lexeme))
		{
			return variables[name.lexeme];
		}
		else
		{
			if (enclosing != null) return enclosing.get(name);
		}

		throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
	}
	public bool isVariable(Token name)
	{
		return variables.ContainsKey(name.lexeme);
	}

	public bool IsFunction(Token name)
	{
		if (funGlobal.ContainsKey(name.lexeme) || builtins.ContainsKey(name.lexeme))
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
		else if(builtins.ContainsKey(name.lexeme))
		{
			return builtins[name.lexeme] == arity;
		}
		else if (enclosing != null)
		{
			return enclosing.IsFunction(name, arity);
		}
		return false;
	}

	public bool IsBuiltin(string name, int arity)
	{
		return builtins.ContainsKey(name) && builtins[name]==arity ;
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
