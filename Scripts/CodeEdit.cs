using Godot;
using System;
using System.IO;
using System.Linq;

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
		CodeComplet();
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
		RequestCompletion();
	}
	public void _on_spin_box_value_changed(float value)
	{
		Paint.pwCanvas.SetupCanvas((int)value, (int)value);
	}

	public void _on_home_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
	public void _on_trash_button_button_pressed()
	{
		Paint.ClearCanvas();
		ConsoleLog.Text = "";
		codeEdit.Text = "";
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

			
	private string[] _completionWords =
	{
		"Spawn", "Color", "Size", "DrawLine", "DrawCircle","Red","Blue","Green","Yellow","Purple","Black","Orange","White","Transparent",
		"DrawRectangle", "Fill","Move","GetActualX", "GetActualY", "GetCanvasSize", "GetColorCount",
		"IsBrushColor", "IsBrushSize", "IsCanvasColor","GoTo", "if", "else", "while", "for","fun", "break", "continue", "return",
	};
	
	public void CodeComplet()
	{
		CodeCompletionEnabled = true;
		
		string[] prefixes = _completionWords
			.Select(word => word[0].ToString())
			.Distinct()
			.ToArray();

		Godot.Collections.Array<string> pr = new Godot.Collections.Array<string>();

		foreach (var p in prefixes)
		{
			pr.Add(p);
		}
		
		CodeCompletionPrefixes = pr;
		
		
		CodeCompletionRequested    += RequestCompletion;
	}

	private void RequestCompletion()
	{
		string prefix = GetCurrentPrefix();
		if (string.IsNullOrEmpty(prefix)) return;
		
		
		var suggestions = _completionWords
			.Where(word => word.StartsWith(prefix))
			.ToList();

		
		
		foreach (string word in suggestions)
		{
			AddCodeCompletionOption(
				CodeCompletionKind.PlainText,
				displayText: word,
				insertText: word
			);
		}
		
	
		UpdateCodeCompletionOptions(true);
	}

	private string GetCurrentPrefix()
	{
		int caretLine = GetCaretLine();
		int caretColumn = GetCaretColumn();
		string lineText = GetLine(caretLine);
		
		
		if (caretColumn > lineText.Length)
		{
			return "";
		}
		
		int start = caretColumn - 1;
		while (start >= 0)
		{
			char c = lineText[start];
			if (!(char.IsLetterOrDigit(c) || c == '_'))
			{
				break;
			}
			start--;
		}
		
		int startIndex = start + 1;
		int length = caretColumn - startIndex;
		
		return lineText.Substring(startIndex, length);
	}
}

	

	

	
