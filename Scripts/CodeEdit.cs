using Godot;
using System;

public partial class CodeEdit : Godot.CodeEdit
{
    [Export] CodeEdit codeEdit;

    public void _on_button_pressed()
    {
        Interpreter.run(codeEdit.Text);
    }
}
