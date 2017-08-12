using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorButtons.Buttons
{
	public interface IBrushSettings<out T> where T : Brush
	{
		T Brush { get; }
	}

	public class BrushSettings<T> : IBrushSettings<T> where T : Brush
	{
		private T brush;
		public T Brush { get => brush; set => brush = value; }
	}

	public class SolidBrushSettings : BrushSettings<SolidBrush>
	{
		public Color Color { get; set; }
	}

	public class LinearBrushSettings : BrushSettings<LinearGradientBrush>
	{
		public RectangleF Rect { get; set; }
		public LinearGradientMode Mode { get; set; }

		public Point Start { get; set; }
		public Point End { get; set; }

		public Color Color1 { get; set; }
		public Color Color2 { get; set; }
	}

	public class PathBrushSettings : BrushSettings<PathGradientBrush>
	{
		public Point[] Path { get; set; }
		public WrapMode Mode { get; set; }

		public Color CenterColor { get; set; }
		public Color[] SurroundColors { get; set; }
	}

	public class HatchBrushSettings : BrushSettings<HatchBrush>
	{
		public HatchStyle Style { get; set; }
		public Color ForeColor { get; set; }
		public Color BackColor { get; set; }
	}

	public class TextureBrushSettings : BrushSettings<TextureBrush>
	{
		public Bitmap Texture { get; set; }
		public Rectangle Rect { get; set; }

		public WrapMode Mode { get; set; }
	}
}