using Godot;
public partial class PwCanvas : TextureRect
{
	[Export] public int CanvasWidth = 31;
	[Export] public int CanvasHeight = 31;
	private Image image;
	private ImageTexture texture;

	public override void _Ready()
	{
		SetupCanvas(CanvasWidth,CanvasHeight);
	}

	public void SetupCanvas(int x,int y)
	{
		Paint.pwCanvas = this;
		Paint.canvas = new PwColor[x, y];

		CanvasWidth = x;
		CanvasHeight = y;

		image = Image.CreateEmpty(x, y, false, Image.Format.Rgba8);
		image.Fill(Colors.White);

		texture = ImageTexture.CreateFromImage(image);
		Texture = texture;

		Scale = new Vector2(512/x, 512/y);
		ExpandMode = ExpandModeEnum.IgnoreSize;
		StretchMode = StretchModeEnum.Keep;
		TextureFilter = TextureFilterEnum.Nearest;
	}
	public void SetPixel(int x, int y, Color color)
	{
		image.SetPixel(x, y, color);
	}
	public Color GetPixel(int x, int y)
	{
		return image.GetPixel(x, y);
	}
	public void ClearCanvas(Color? fillColor)
	{
		image.Fill(fillColor ?? Colors.White);
	}
	public void UpdateTexture()
	{
		texture.Update(image);
	}
}
