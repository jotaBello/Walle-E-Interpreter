using System.Collections.Generic;
using Godot;

public partial class Log : TextEdit
{
	[Export] TextEdit textEdit;

	public override void _Ready()
	{
		LogReporter.log = this;
	}

	public void LogMessage(string message)
	{
		textEdit.Text += message + "\n";
	}
	public void LogMessages(List<string> messages)
	{
		CleanLog();
		foreach (var message in messages)
		{
			LogMessage(message);
		}
	}
	public void CleanLog()
	{
		textEdit.Text = "";
	}
}
public static class LogReporter
{
	[Export] public static Log log;
	public static void LogMessage(string message)
	{
		log.LogMessage(message);
	}
	public static void LogMessages(List<string> messages)
	{
		log.LogMessages(messages);
	}
	 public static void CleanLog()
	{
		log.CleanLog();
	}
	

}
