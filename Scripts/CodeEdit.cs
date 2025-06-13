using Godot;
using System;
using System.IO;

public partial class CodeEdit : Godot.CodeEdit
{
	[Export] CodeEdit codeEdit;
	[Export] TextEdit ConsoleLog;
	[Export] FileDialog SaveFileDialog;
	[Export] FileDialog ImportFileDialog;
	
	


	public override void _Ready()
	{
		CodeCompletionEnabled = true;
		SyntaxHighlight();
		ConsoleLogSyntaxHighlight();
	}


	public void _on_run_button_pressed()
	{
		Paint.ClearCanvas();
		Compiler.hadRuntimeError = false;
		Compiler.hadError = false;
		Compiler.run(codeEdit.Text);
	}

	public void _on_text_changed()
	{
		Compiler.hadError = false;
		Compiler.resolve(codeEdit.Text);
	}



	public void _on_save_button_pressed()
	{
		SaveFileDialog.Popup();
	}
	public void _on_import_button_pressed()
	{
		ImportFileDialog.Popup();
	}
	public void _on_save_file_dialog_file_selected(string path)
	{
		try
		{
			File.WriteAllText(path, codeEdit.Text);
			GD.Print($"Archivo guardado: {path}");
		}
		catch (Exception ex)
		{
			GD.Print("Error al guardar" + ex.Message);
		}

	}
	public void _on_import_file_dialog_file_selected(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				codeEdit.Text = File.ReadAllText(path);
				GD.Print($"Archivo cargado: {path}");
			}
			else
			{
				GD.Print("Archivo no encontrado");
			}
		}
		catch (Exception ex)
		{
			GD.Print($"Error al cargar: {ex.Message}");
		}
	}
	void ConsoleLogSyntaxHighlight()
	{
		CodeHighlighter codeHighlighter = new CodeHighlighter();
		ConsoleLog.SyntaxHighlighter = codeHighlighter;

		codeHighlighter.SymbolColor = Colors.Gray;
		codeHighlighter.NumberColor = Colors.LightGray;
		codeHighlighter.FunctionColor = Colors.Cyan;
	}

	void SyntaxHighlight()
	{
		CodeHighlighter codeHighlighter = new CodeHighlighter();
		codeEdit.SyntaxHighlighter = codeHighlighter;

		codeHighlighter.SymbolColor = Colors.Gray;
		codeHighlighter.NumberColor = Colors.LightGray;
		codeHighlighter.FunctionColor = Colors.Cyan;

		foreach (string cmd in commands)
		{
			codeHighlighter.AddKeywordColor(cmd, keywordColor);
		}


		foreach (string func in functions)
		{
			codeHighlighter.AddKeywordColor(func, functionColor);
		}


		foreach (string flow in flowKeywords)
		{
			codeHighlighter.AddKeywordColor(flow, flowColor);
		}


		foreach (string op in operators)
		{
			codeHighlighter.AddKeywordColor(op, operatorColor);
		}


		codeHighlighter.AddColorRegion("\"", "\"", stringColor, false); // Strings
		codeHighlighter.AddColorRegion("//", "", commentColor, true);   //LineComment
	codeHighlighter.AddColorRegion("/*", "*/", commentColor);   //blockComment
	}

	private readonly Color keywordColor = new Color("#569cd6");      // Blue
	private readonly Color functionColor = new Color("#dcdcaa");     // LightYellow
	private readonly Color typeColor = new Color("#4ec9b0");         // Cyan
	private readonly Color stringColor = new Color("#ce9178");       // LightBrown
	private readonly Color numberColor = new Color("#b5cea8");       // LightGreen
	private readonly Color commentColor = new Color("#6a9955");      // Green
	private readonly Color flowColor = new Color("#c586c0");         // Purple
	private readonly Color operatorColor = new Color("#d4d4d4");     // LightGray


	private readonly string[] commands = {
		"Spawn", "Color", "Size", "DrawLine", "DrawCircle",
		"DrawRectangle", "Fill","Move",
	};

	private readonly string[] functions = {
		"GetActualX", "GetActualY", "GetCanvasSize", "GetColorCount",
		"IsBrushColor", "IsBrushSize", "IsCanvasColor"
	};

	private readonly string[] flowKeywords = {
		"GoTo", "if", "else", "while", "for", "break", "continue", "return"
	};

	private readonly string[] operators = {
		"<-", "\\+", "-", "\\*", "/", "\\*\\*", "%", 
		"==", ">=", "<=", ">", "<", "&&", "\\|\\|"
	};







	//AUTOCOMPLETION
	/*
	private static readonly string[] Keywords = {
		"Spawn", "Color", "Size", "DrawLine", "DrawCircle",
		"DrawRectangle", "Fill", "GetActualX", "GetActualY",
		"GetCanvasSize", "GetColorCount", "IsBrushColor",
		"IsBrushSize", "IsCanvasColor", "GoTo"
	};
	private static readonly string[] Colors = {
		"Red", "Blue", "Green", "Yellow", "Orange",
		"Purple", "Black", "White", "Transparent"
	};
	private readonly HashSet<string> _userDefined = new();
	private readonly Regex _definitionRegex = new(@"(?<!\S)([a-zA-Z][\w-]*)(?!\S)\s*(?:$|<-)");

	private void OnTextChanged()
	{
		UpdateUserDefinitions();
	}
	private void UpdateUserDefinitions()
	{
		_userDefined.Clear();

		foreach (var line in Text.Split('\n'))
		{
			var match = _definitionRegex.Match(line);
			if (match.Success && !Keywords.Contains(match.Value))
			{
				_userDefined.Add(match.Value);
			}
		}
	}

	private void OnCompletionRequested()
	{
		var lineIdx = GetCaretLine();
		var line = GetLine(lineIdx);
		var caretPos = GetCaretColumn();

		// Determinar contexto actual
		var context = GetCompletionContext(line, caretPos);

		// Generar sugerencias según el contexto
		switch (context)
		{
			case "color":
				AddColorSuggestions();
				break;

			case "command":
				AddCommandSuggestions();
				break;

			case "variable":
				AddVariableSuggestions();
				break;

			default:
				AddGeneralSuggestions();
				break;
		}
	}
	private string GetCompletionContext(string line, int caretPos)
	{
		// Analizar texto antes del cursor
		var preText = line.Substring(0, caretPos);

		// Si estamos dentro de un Color( ... )
		if (Regex.IsMatch(preText, @"Color\s*\(\s*[\""\']?[^\""\']*$"))
			return "color";

		// Si estamos al inicio de línea o después de salto
		if (string.IsNullOrWhiteSpace(preText))
			return "command";

		// Si estamos después de una asignación o parámetro
		if (preText.EndsWith(" ") || preText.EndsWith(","))
			return "variable";

		// Si estamos en un identificador
		return "general";
	}

	private void AddColorSuggestions()
	{
		foreach (var color in Colors)
		{
			AddCodeCompletionOption(CodeCompletionKind.PlainText, color, color);
		}
	}

	private void AddCommandSuggestions()
	{
		foreach (var keyword in Keywords)
		{
			AddCodeCompletionOption(CodeCompletionKind.PlainText, keyword, keyword);
		}
	}

	private void AddVariableSuggestions()
	{
		foreach (var item in _userDefined)
		{
			AddCodeCompletionOption(CodeCompletionKind.PlainText, item, item);
		}
	}

	private void AddGeneralSuggestions()
	{
		AddCommandSuggestions();
		AddColorSuggestions();
		AddVariableSuggestions();
	}
	
	*/

}
	

	

	
