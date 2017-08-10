using Duality;
using Duality.Drawing;
using Duality.Resources;

namespace EditorButtons.Editor.Backgrounds
{
	public class BackgroundBase
	{
		public bool Outline { get; set; } = true;
	}

	public class SolidBackground : BackgroundBase, IButtonBackground
	{
		public BackgroundStyle BackgroundStyle { get => BackgroundStyle.Solid; }
		public ColorRgba Color { get; set; }

		public SolidBackground(ColorRgba BGColor)
		{
			Color = BGColor;
		}
	}

	public class LinearBackground : BackgroundBase, IButtonBackground
	{
		public BackgroundStyle BackgroundStyle { get => BackgroundStyle.LinearGradient; }

		public Point2 StartPoint { get; set; }
		public Point2 EndPoint { get; set; }

		public ColorRgba Color1 { get; set; }
		public ColorRgba Color2 { get; set; }

		public Rect Rectangle { get; set; }

		public BgGradientMode GradientMode { get; set; }

		public LinearBackground(ColorRgba c1, ColorRgba c2, BgGradientMode gradientMode, Rect rect)
		{
			Color1 = c1;
			Color2 = c2;
			Rectangle = rect;
			GradientMode = gradientMode;
		}

		public LinearBackground(ColorRgba c1, ColorRgba c2, BgGradientMode gradientMode, Point2 Start, Point2 End)
		{
			Color1 = c1;
			Color2 = c2;
			Rectangle = Rect.Empty;
			GradientMode = gradientMode;
			StartPoint = Start;
			EndPoint = End;
		}
	}

	public class HatchBackground : BackgroundBase, IButtonBackground
	{
		public BackgroundStyle BackgroundStyle { get => BackgroundStyle.Hatch; }
		public BgHatchStyle Style { get; set; }
		public ColorRgba ForeColor { get; set; }
		public ColorRgba BackColor { get; set; } = ColorRgba.TransparentWhite;

		public HatchBackground(BgHatchStyle style, ColorRgba foreColor, ColorRgba? backColor = null)
		{
			Style = style;
			ForeColor = foreColor;
			if (backColor != null) BackColor = backColor.Value;
		}
	}

	public class PathBackground : BackgroundBase, IButtonBackground
	{
		public BackgroundStyle BackgroundStyle { get => BackgroundStyle.PathGradient; }
		public BgWrapMode Mode { get; set; }
		public Point2[] Path { get; set; }
		public ColorRgba CenterColor { get; set; }
		public ColorRgba[] SurroundColors { get; set; }

		public PathBackground(Point2[] path, ColorRgba centerColor, ColorRgba[] surroundColors, BgWrapMode mode = 0)
		{
			Path = path;
			CenterColor = centerColor;
			SurroundColors = surroundColors;
			Mode = mode;
		}
	}

	public class TextureBackground : BackgroundBase, IButtonBackground
	{
		public BackgroundStyle BackgroundStyle { get => BackgroundStyle.Texture; }

		public ContentRef<Pixmap> Texture { get; set; }
		public Rect Rectangle { get; set; }

		public BgWrapMode Mode { get; set; }

		public TextureBackground(ContentRef<Pixmap> texture, BgWrapMode mode, Rect rect)
		{
			Texture = texture;
			Rectangle = rect;
			Mode = mode;
		}

		public TextureBackground(ContentRef<Pixmap> texture, BgWrapMode mode)
		{
			Texture = texture;
			Rectangle = Rect.Empty;
			Mode = mode;
		}
	}
}