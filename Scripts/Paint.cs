using Godot;
using System;
using System.Collections.Generic;
public enum PwColor
{	
	White,
	Transparent,
	Red,
	Blue,
	Green,
	Yellow,
	Orange,
	Purple,
	Black,
	
}

public static class Paint
{
	static bool wasSpawned = false;
	static Tuple<int, int> wall_ePosition = new Tuple<int, int>(0, 0);
	static PwColor colorBrush = PwColor.Transparent;
	static Godot.Color pwcolorBrush = Colors.Transparent;
	static int sizeBrush = 1;
	static public PwColor[,] canvas;
	static public PwCanvas pwCanvas;



	public static void ChangeCanvasSize(int width, int height)
	{
		pwCanvas.CanvasWidth = width;
		pwCanvas.CanvasHeight = height;
		pwCanvas.ClearCanvas(null);
	}

	private static void MoveWalle(int x, int y)
	{
		wall_ePosition = new Tuple<int, int>(x, y);
	}

	public static void Spawn(int x, int y)
	{
		if (!wasSpawned)
		{
			if (isOnTheBounds(x, y))
			{
				wall_ePosition = new Tuple<int, int>(x, y);
				wasSpawned = true;
			}
			else
			{
				throw new RuntimeError(null, "wall_e must be on the bounds");
			}
		}
		else
		{
			throw new RuntimeError(null, "wall_e already was spawned");
		}
	}
	private static bool isOnTheBounds(int x, int y)
	{
		return x >= 0 && y >= 0 && x < canvas.GetLength(0) && y < canvas.GetLength(1);
	}
	public static void Color(string color)
	{
		switch (color)
		{
			case "Red":
				colorBrush = PwColor.Red;
				pwcolorBrush = Colors.Red;
				break;
			case "Blue":
				colorBrush = PwColor.Blue;
				pwcolorBrush = Colors.Blue;
				break;
			case "Green":
				colorBrush = PwColor.Green;
				pwcolorBrush = Colors.Green;
				break;
			case "Yellow":
				colorBrush = PwColor.Yellow;
				pwcolorBrush = Colors.Yellow;
				break;
			case "Purple":
				colorBrush = PwColor.Purple;
				pwcolorBrush = Colors.Purple;
				break;
			case "Black":
				colorBrush = PwColor.Black;
				pwcolorBrush = Colors.Black;
				break;
			case "Orange":
				colorBrush = PwColor.Orange;
				pwcolorBrush = Colors.Orange;
				break;
			case "White":
				colorBrush = PwColor.White;
				pwcolorBrush = Colors.White;
				break;
			case "Transparent":
				colorBrush = PwColor.Transparent;
				pwcolorBrush = Colors.Transparent;
				break;
			default:
				throw new RuntimeError(null, "Unexpected Color");
		}
	}
	public static void Size(int k)
	{
		if (k >= 1)
			sizeBrush = k + k % 2 - 1;
		else throw new RuntimeError(null, "The brush size must be greater than 1");
	}
	public static void DrawLine(int dirX, int dirY, int distance)
	{
		if (isAValidDirection(dirX, dirY))
		{
			int newX = wall_ePosition.Item1;
			int newy = wall_ePosition.Item2;

			while (isOnTheBounds(newX, newy) && distance > 0)
			{
				MoveWalle(newX, newy);
				SetPixel(newX, newy, pwcolorBrush);
				newX = newX + dirX;
				newy = newy + dirY;
				distance--;
			}
			if (isOnTheBounds(newX, newy)) MoveWalle(newX, newy);
			UpdateTexture();
		}
		else
		{
			throw new RuntimeError(null, "Is an invalid direction");
		}
	}

	static bool isAValidDirection(int dirX, int dirY)
	{
		return (dirX == 1 || dirX == 0 || dirX == -1) && (dirY == 1 || dirY == 0 || dirY == -1);
	}

	public static void DrawCircle(int dirX, int dirY, int radius)
	{
		int h = wall_ePosition.Item1 + dirX * radius;
		int k = wall_ePosition.Item2 + dirY * radius;

		if (isOnTheBounds(h, k))
		{
			for (int i = 0; i < canvas.GetLength(0); i++)
			{
				for (int j = 0; j < canvas.GetLength(1); j++)
				{
					if (isOnCircle(i, j))
					{
						SetPixel(i, j, pwcolorBrush);
					}
				}
			}


			MoveWalle(h, k);
			UpdateTexture();
		}

		else
		{
			throw new RuntimeError(null, "Is an invalid center of the circle");
		}

		bool isOnCircle(int i, int j)
		{
			return Math.Abs((i - h) * (i - h) + (j - k) * (j - k) - radius * radius) <= radius - 1;
		}
	}

	public static void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
	{
		int h = wall_ePosition.Item1 + dirX * distance;
		int k = wall_ePosition.Item2 + dirY * distance;

		if (isOnTheBounds(h, k))
		{
			for (int i = 0; i < canvas.GetLength(0); i++)
			{
				for (int j = 0; j < canvas.GetLength(1); j++)
				{
					if (isOnRectangle(i, j))
					{
						SetPixel(i, j, pwcolorBrush);
					}
				}
			}
			MoveWalle(h, k);
			UpdateTexture();
		}
		else
		{
			throw new RuntimeError(null, "Is an invalid center of the rectangle");
		}
		bool isOnRectangle(int i, int j)
		{
			return Math.Abs(i - h) == height / 2 && Math.Abs(j - k) <= width / 2 || Math.Abs(j - k) == width / 2 && Math.Abs(i - h) <= height / 2;
		}
	}
	public static void Fill()
	{
		Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>() { };
		queue.Enqueue(wall_ePosition);
		PwColor color = canvas[wall_ePosition.Item1, wall_ePosition.Item2];

		bool[,] mask = new bool[canvas.GetLength(0), canvas.GetLength(1)];
		mask[wall_ePosition.Item1, wall_ePosition.Item2] = true;

		int[] dirX = { 1, -1, 0, 0 };
		int[] dirY = { 0, 0, -1, 1 };

		while (queue.Count > 0)
		{
			Tuple<int, int> current = queue.Dequeue();

			for (int i = 0; i < dirX.Length; i++)
			{
				int h = current.Item1 + dirX[i]; int k = current.Item2 + dirY[i];

				if (isOnTheBounds(h, k) && canvas[h, k] == color && !mask[h, k])
				{
					mask[h, k] = true;
					queue.Enqueue(new Tuple<int, int>(h, k));
				}
			}
			SetOnePixel(current.Item1, current.Item2, pwcolorBrush);
		}
		UpdateTexture();
	}
	public static void SetOnePixel(int x, int y, Godot.Color color)
	{
		if (colorBrush != PwColor.Transparent)
		{
			canvas[x, y] = colorBrush;
			pwCanvas.SetPixel(x, y, color);
		}
	}

	public static void SetPixel(int x, int y, Godot.Color color)
	{
		if (colorBrush != PwColor.Transparent)
		{
			for (int i = 0; i < canvas.GetLength(0); i++)
			{
				for (int j = 0; j < canvas.GetLength(1); j++)
				{
					if (isOnTheSquare(i, j))
					{
						canvas[i, j] = colorBrush;
						pwCanvas.SetPixel(i, j, color);
					}
				}
			}

		}
		bool isOnTheSquare(int i, int j)
		{
			return Math.Max(Math.Abs(i - x), Math.Abs(j - y)) <= sizeBrush / 2;
		}
	}
	public static void UpdateTexture()
	{
		pwCanvas.UpdateTexture();
	}
	public static void ClearCanvas()
	{
		pwCanvas.ClearCanvas(null);
		UpdateTexture();

		wasSpawned = false;
		wall_ePosition = new Tuple<int, int>(0, 0);
		colorBrush = PwColor.Transparent;
		pwcolorBrush = Colors.Transparent;
		sizeBrush = 1;
		canvas = new PwColor[pwCanvas.CanvasHeight, pwCanvas.CanvasWidth];
	}
	public static string GetColor(int x, int y)
	{
		if (!isOnTheBounds(x, y)) throw new RuntimeError(null, "Is an invalid pixel");
		return canvas[x, y].ToString();
	}
	public static int GetActualX()
	{
		return wall_ePosition.Item1;
	}
	public static int GetActualY()
	{
		return wall_ePosition.Item2;
	}
	public static int GetCanvasSize()
	{
		return canvas.GetLength(0);
	}
	public static int GetColorCount(string color, int x1, int y1, int x2, int y2)
	{
		if (!isOnTheBounds(x1, y1) || !isOnTheBounds(x2, y2)) return 0;

		int count = 0;

		for (int i = x1; i <= x2; i++)
		{
			for (int j = y1; j <= y2; j++)
			{
				if (canvas[i, j].ToString() == color)
				{
					count++;
				}
			}
		}

		return count;
	}
	public static int IsBrushColor(string color)
	{
		return color == colorBrush.ToString() ? 1 : 0;
	}
	public static int IsBrushSize(int size)
	{
		return size == sizeBrush ? 1 : 0;
	}
	public static int IsCanvasColor(string color, int vertical, int horizontal)
	{
		int h = wall_ePosition.Item1 + vertical;
		int k = wall_ePosition.Item2 + horizontal;
		if (!isOnTheBounds(h, k)) return 0;
		return canvas[h, k].ToString() == color ? 1 : 0;
	}
	public static void Move(int x, int y)
	{
		if (isOnTheBounds(x, y))
		{
			MoveWalle(x, y);
		}
	}


}
