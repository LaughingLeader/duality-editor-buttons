using AdamsLair.WinForms.Drawing;
using AdamsLair.WinForms.PropertyEditing;
using Duality;
using Duality.Drawing;
using Duality.Editor;
using EditorButtons.Buttons.Backgrounds;
using EditorButtons.PropertyEditors;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EditorButtons.Buttons
{
	/// <summary>
	/// Common methods shared between Button PropertyEditor classes.
	/// ButtonContainerPropertyEditor uses this when it only has one row of buttons (to display it inline).
	/// </summary>
	internal static class ButtonPropertyMethods
	{
		private static bool colorsInitialized = false;
		private static Color defaultColorInner;
		private static Color defaultColorBorder;
		private static Color defaultColorHighlight;
		private static Color defaultColorText;
		private static Color defaultColorGrayText;

		public static Color DefaultColorInner { get => defaultColorInner; set => defaultColorInner = value; }
		public static Color DefaultColorBorder { get => defaultColorBorder; set => defaultColorBorder = value; }
		public static Color DefaultColorHighlight { get => defaultColorHighlight; set => defaultColorHighlight = value; }
		public static Color DefaultColorText { get => defaultColorText; set => defaultColorText = value; }
		public static Color DefaultColorGrayText { get => defaultColorGrayText; set => defaultColorGrayText = value; }

		public static bool DataChanged { get; set; } = false;

		public static void InitColors()
		{
			//DefaultColorInner = ControlRenderer.ColorVeryLightBackground;
			//DefaultColorBorder = ControlRenderer.ColorVeryDarkBackground;
			//DefaultColorHighlight = ControlRenderer.ColorHightlight;
			//DefaultColorText = ControlRenderer.ColorText;
			//DefaultColorGrayText = ControlRenderer.ColorGrayText;

			if (!colorsInitialized)
			{
				DefaultColorInner = SystemColors.ControlLightLight;
				DefaultColorBorder = SystemColors.ControlDarkDark;
				DefaultColorHighlight = SystemColors.Highlight;
				DefaultColorText = SystemColors.ControlText;
				DefaultColorGrayText = SystemColors.GrayText;
				colorsInitialized = true;
			}
		}

		public static void ApplyColors(ButtonProperty button, ControlRenderer ControlRenderer)
		{
			if (button.Value != null && button.Value.Colors != null)
			{
				var defaultColor = default(ColorRgba);

				var bColors = button.Value.Colors;

				//Check if the color has been set.
				if (bColors.ColorInner != defaultColor) ControlRenderer.ColorVeryLightBackground = bColors.ColorInner.ToSysDrawColor();
				if (bColors.ColorBorder != defaultColor) ControlRenderer.ColorVeryDarkBackground = bColors.ColorBorder.ToSysDrawColor();
				if (bColors.ColorHighlight != defaultColor) ControlRenderer.ColorHightlight = bColors.ColorHighlight.ToSysDrawColor();
				if (bColors.ColorText != defaultColor) ControlRenderer.ColorText = bColors.ColorText.ToSysDrawColor();
				if (bColors.ColorGrayText != defaultColor) ControlRenderer.ColorGrayText = bColors.ColorGrayText.ToSysDrawColor();
			}
		}

		public static void ResetColors(ControlRenderer ControlRenderer)
		{
			ControlRenderer.ColorVeryLightBackground = DefaultColorInner;
			ControlRenderer.ColorVeryDarkBackground = DefaultColorBorder;
			ControlRenderer.ColorHightlight = DefaultColorHighlight;
			ControlRenderer.ColorText = DefaultColorText;
			ControlRenderer.ColorGrayText = DefaultColorGrayText;
		}

		public static void RefreshAffectedProperty(PropertyEditor editor, IButtonValue button = null)
		{
			var parent = editor.ParentGrid;

			DualityEditorApp.NotifyObjPropChanged(parent, new ObjectSelection(parent.Selection));
			parent.MainEditor.PerformGetValue();

			var aboveControl = parent.ParentForm.GetContainerControl();

			if (button != null && button.AdditionalUpdates != null && button.AdditionalUpdates.Count > 0)
			{
				foreach (var gameObj in button.AdditionalUpdates)
				{
					DualityEditorApp.NotifyObjPropChanged(parent, new ObjectSelection(gameObj), false);
					DualityEditorApp.UpdateGameObject(gameObj);
				}
			}
		}

		public static void FillBackground<T>(Rectangle Panel, PaintEventArgs e, T fillBrush, bool Outline = true) where T : Brush
		{
			e.Graphics.FillRectangle(
					fillBrush,
					Panel.X,
					Panel.Y,
					Panel.Width,
					Panel.Height);

			if (Outline) e.Graphics.DrawRectangle(SystemPens.ControlDark, Panel);
		}

		public static void PaintSolid(Rectangle Panel, PaintEventArgs e, Color? color = null, bool Outline = true)
		{
			if (color == null) color = Color.LightGray;
			FillBackground(Panel, e, new SolidBrush(color.Value), Outline);
		}

		public static IBrushSettings<Brush> PrepareBrush(IButtonBackground Background)
		{
			if (Background != null)
			{
				switch (Background.BackgroundStyle)
				{
					case BackgroundStyle.Solid:
						{
							SolidBackground bg = (SolidBackground)Background;
							var settings = new SolidBrushSettings()
							{
								Color = bg.Color.ToSysDrawColor()
							};
							settings.Brush = new SolidBrush(settings.Color);
							return settings;
						}
					case BackgroundStyle.LinearGradient:
						{
							LinearBackground bg = (LinearBackground)Background;

							RectangleF rect = RectangleF.Empty;
							if (bg.Rectangle != Rect.Empty) rect = new RectangleF(bg.Rectangle.X, bg.Rectangle.Y, bg.Rectangle.W, bg.Rectangle.H);

							var startPoint = new Point(bg.StartPoint.X, bg.StartPoint.Y);
							var endPoint = new Point(bg.EndPoint.X, bg.EndPoint.Y);

							var settings = new LinearBrushSettings
							{
								Rect = rect,
								Mode = (LinearGradientMode)bg.GradientMode,
								Color1 = bg.Color1.ToSysDrawColor(),
								Color2 = bg.Color2.ToSysDrawColor(),
								Start = startPoint,
								End = endPoint
							};

							if (settings.Rect != RectangleF.Empty)
							{
								settings.Brush = new LinearGradientBrush(settings.Rect, settings.Color1, settings.Color2, settings.Mode);
							}
							else
							{
								settings.Brush = new LinearGradientBrush(settings.Start, settings.End, settings.Color1, settings.Color2);
							}

							return settings;
						}
					case BackgroundStyle.PathGradient:
						{
							PathBackground bg = (PathBackground)Background;

							Point[] path = new Point[bg.Path.Length];
							for (var i = 0; i < bg.Path.Length; i++)
							{
								var point = bg.Path[i];
								path[i] = new Point(point.X, point.Y);
							}

							var mode = (WrapMode)bg.Mode;
							var brush = new PathGradientBrush(path, mode);

							Color[] surroundColors = new Color[bg.SurroundColors.Length];
							for (var i = 0; i < surroundColors.Length; i++)
							{
								surroundColors[i] = bg.SurroundColors[i].ToSysDrawColor();
							}

							var settings = new PathBrushSettings
							{
								Path = path,
								Brush = brush,
								Mode = mode,
								CenterColor = bg.CenterColor.ToSysDrawColor(),
								SurroundColors = surroundColors
							};

							brush.CenterColor = settings.CenterColor;
							brush.SurroundColors = settings.SurroundColors;
							return settings;
						}
					case BackgroundStyle.Hatch:
						{
							HatchBackground bg = (HatchBackground)Background;
							var settings = new HatchBrushSettings()
							{
								Style = (HatchStyle)bg.Style,
								ForeColor = bg.ForeColor.ToSysDrawColor(),
								BackColor = bg.BackColor.ToSysDrawColor()
							};
							settings.Brush = new HatchBrush(settings.Style, settings.ForeColor, settings.BackColor);
							return settings;
						}
					case BackgroundStyle.Texture:
						{
							TextureBackground bg = (TextureBackground)Background;

							Bitmap tex = bg.Texture.Res.MainLayer.ToBitmap();
							Rectangle rect = Rectangle.Empty;
							if (bg.Rectangle != Rect.Empty) rect = new Rectangle(MathF.RoundToInt(bg.Rectangle.X), MathF.RoundToInt(bg.Rectangle.Y),
								MathF.RoundToInt(bg.Rectangle.W), MathF.RoundToInt(bg.Rectangle.H));

							var settings = new TextureBrushSettings()
							{
								Mode = (WrapMode)bg.Mode,
								Texture = tex,
								Rect = rect
							};
							if (settings.Rect != Rectangle.Empty)
							{
								settings.Brush = new TextureBrush(settings.Texture, settings.Mode, settings.Rect);
							}
							else
							{
								settings.Brush = new TextureBrush(settings.Texture, settings.Mode);
							}
							return settings;
						}
				}
			}

			var defaultSettings = new SolidBrushSettings()
			{
				Color = Color.Transparent
			};
			defaultSettings.Brush = new SolidBrush(defaultSettings.Color);
			return defaultSettings;
		}
	}
}