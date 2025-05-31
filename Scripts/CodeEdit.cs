using Godot;
using System;

public partial class CodeEdit : Godot.CodeEdit
{
	[Export] CodeEdit codeEdit;

	public void _on_button_pressed()
	{
		Console.WriteLine("debug in vscode");
		Compiler.run(codeEdit.Text);
	}
}
