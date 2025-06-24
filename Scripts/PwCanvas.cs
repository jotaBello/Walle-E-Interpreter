using Godot;
public partial class PwCanvas : TextureRect
{
	[Export] public int CanvasWidth = 31;
	[Export] public int CanvasHeight = 31;
	[Export] public int scale = 720;
	private Image image;
	private ImageTexture texture;

	public override void _Ready()
	{
		SetupCanvas(CanvasWidth,CanvasHeight);
	}

	public void SetupCanvas(int x, int y)
	{
		Paint.pwCanvas = this;
		Paint.canvas = new Color[x, y];
		Paint.FillWhite();

		CanvasWidth = x;
		CanvasHeight = y;

		image = Image.CreateEmpty(x, y, false, Image.Format.Rgba8);
		image.Fill(Colors.White);
		texture?.Dispose();
		texture = ImageTexture.CreateFromImage(image);
		Texture = texture;

		Scale = new Vector2(1, 1);
		
		ExpandMode = ExpandModeEnum.FitWidthProportional;
		StretchMode = StretchModeEnum.KeepAspectCentered;
		TextureFilter = TextureFilterEnum.Nearest;
	}
	public void SetPixel(int x, int y, Color color)
	{
		color.Clamp();
		image.SetPixel(x, y, color);
	}
	public Color GetPixel(int x, int y)
	{
		return image.GetPixel(x, y);
	}
	public void ClearCanvas()
	{
		image.Fill(Colors.White);
	}
	public void UpdateTexture()
	{
		texture.Update(image);
	}
}
