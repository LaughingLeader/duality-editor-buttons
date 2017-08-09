using AdamsLair.WinForms.Drawing;
using Duality;
using Duality.Drawing;
using Duality.Editor;
using EditorButtons.Editor.Backgrounds;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ButtonState = AdamsLair.WinForms.Drawing.ButtonState;

namespace EditorButtons.Editor.Buttons
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

		private static int CalculateTotalWidth(IButtonPropertyEditor editor)
		{
			int width = 0;
			foreach (var button in editor.Buttons)
			{
				button.Rect.Width = editor.ButtonPanel.Width;
				button.Rect.Height = editor.ButtonPanel.Height;

				if (button.Value != null)
				{
					button.Rect.Width = MathF.RoundToInt(editor.ButtonPanel.Width * button.Value.WidthPercentage);
					button.Rect.Height = MathF.RoundToInt(editor.ButtonPanel.Height * button.Value.HeightPercentage);
				}

				width += button.Rect.Width + editor.SpacingX;
			}

			return width;
		}

		public static void RefreshAffectedProperty(IButtonPropertyEditor editor, IButtonValue button)
		{
			var parent = editor.Editor.ParentGrid;

			DualityEditorApp.NotifyObjPropChanged(parent, new ObjectSelection(parent.Selection));
			parent.MainEditor.PerformGetValue();

			var aboveControl = parent.ParentForm.GetContainerControl();

			if (button.AdditionalUpdates != null && button.AdditionalUpdates.Count > 0)
			{
				foreach (var gameObj in button.AdditionalUpdates)
				{
					DualityEditorApp.NotifyObjPropChanged(parent, new ObjectSelection(gameObj), false);
					DualityEditorApp.UpdateGameObject(gameObj);
				}
			}
		}

		private static void FillBackground<T>(IButtonPropertyEditor editor, PaintEventArgs e, T fillBrush) where T : Brush
		{
			e.Graphics.FillRectangle(
					fillBrush,
					editor.ButtonPanel.X,
					editor.ButtonPanel.Y,
					editor.ButtonPanel.Width,
					editor.ButtonPanel.Height);

			e.Graphics.DrawRectangle(SystemPens.ControlDark, editor.ButtonPanel);
		}

		private static void PaintSolid(IButtonPropertyEditor editor, PaintEventArgs e, Color? color = null)
		{
			if (color == null) color = Color.LightGray;
			FillBackground(editor, e, new SolidBrush(color.Value));
		}

		public static void OnPaint(IButtonPropertyEditor editor, PaintEventArgs e, ControlRenderer ControlRenderer)
		{
			if (editor.BrushSettings == null && editor.Background != null)
			{
				switch (editor.Background.BackgroundStyle)
				{
					case BackgroundStyle.Solid:
						{
							SolidBackground bg = (SolidBackground)editor.Background;
							SolidBrushSettings settings = new SolidBrushSettings()
							{
								Color = bg.Color.ToSysDrawColor()
							};
							settings.Brush = new SolidBrush(settings.Color);
							editor.BrushSettings = settings;
						}
						break;

					case BackgroundStyle.LinearGradient:
						{
							LinearBackground bg = (LinearBackground)editor.Background;

							RectangleF rect = RectangleF.Empty;
							if (bg.Rectangle != Rect.Empty) rect = new RectangleF(bg.Rectangle.X, bg.Rectangle.Y, bg.Rectangle.W, bg.Rectangle.H);

							var startPoint = new Point(bg.StartPoint.X, bg.StartPoint.Y);
							var endPoint = new Point(bg.EndPoint.X, bg.EndPoint.Y);

							LinearBrushSettings settings = new LinearBrushSettings
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

							editor.BrushSettings = settings;
						}
						break;

					case BackgroundStyle.PathGradient:
						{
							PathBackground bg = (PathBackground)editor.Background;

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

							PathBrushSettings settings = new PathBrushSettings
							{
								Path = path,
								Brush = brush,
								Mode = mode,
								CenterColor = bg.CenterColor.ToSysDrawColor(),
								SurroundColors = surroundColors
							};

							brush.CenterColor = settings.CenterColor;
							brush.SurroundColors = settings.SurroundColors;
							editor.BrushSettings = settings;
						}
						break;

					case BackgroundStyle.Hatch:
						{
							HatchBackground bg = (HatchBackground)editor.Background;
							HatchBrushSettings settings = new HatchBrushSettings()
							{
								Style = (HatchStyle)bg.Style,
								ForeColor = bg.ForeColor.ToSysDrawColor(),
								BackColor = bg.BackColor.ToSysDrawColor()
							};
							settings.Brush = new HatchBrush(settings.Style, settings.ForeColor, settings.BackColor);
							editor.BrushSettings = settings;
						}
						break;

					case BackgroundStyle.Texture:
						{
							TextureBackground bg = (TextureBackground)editor.Background;

							Bitmap tex = bg.Texture.Res.MainLayer.ToBitmap();
							Rectangle rect = Rectangle.Empty;
							if (bg.Rectangle != Rect.Empty) rect = new Rectangle(MathF.RoundToInt(bg.Rectangle.X), MathF.RoundToInt(bg.Rectangle.Y),
								MathF.RoundToInt(bg.Rectangle.W), MathF.RoundToInt(bg.Rectangle.H));

							TextureBrushSettings settings = new TextureBrushSettings()
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
							editor.BrushSettings = settings;
						}
						break;

					default:
						{
							PaintSolid(editor, e);
						}
						break;
				}
			}

			if (editor.BrushSettings != null && editor.BrushSettings.Brush != null)
			{
				FillBackground(editor, e, editor.BrushSettings.Brush);
			}
			else
			{
				PaintSolid(editor, e);
			}

			if (editor.ButtonPanel != null)
			{
				var editorRect = editor.ButtonPanel;
				editorRect.Y = editor.Editor.ClientRectangle.Y;
				editor.ButtonPanel = editorRect;
			}

			if (editor.Buttons.Count > 0)
			{
				if (editor.TotalWidth == 0) editor.TotalWidth = CalculateTotalWidth(editor);

				int bX = editor.ButtonPanel.X, bY = editor.ButtonPanel.Y;

				if (editor.Align == ButtonRowAlign.Center)
				{
					bX = editor.ButtonPanel.X + ((editor.ButtonPanel.Width - editor.TotalWidth) / 2);

					foreach (var button in editor.Buttons)
					{
						button.Rect.X = bX;
						button.Rect.Y = bY;

						ButtonState bstate = ButtonState.Disabled;
						if (editor.Editor.Enabled)
						{
							if (button.Pressed) bstate = ButtonState.Pressed;
							else if (button.Hovered) bstate = ButtonState.Hot;
							else bstate = ButtonState.Normal;
						}

						ApplyColors(button, ControlRenderer);
						ControlRenderer.DrawButton(e.Graphics, button.Rect, bstate, button.Label);
						ResetColors(ControlRenderer);

						bX += button.Rect.Width + editor.SpacingX;
					}
				}
				else
				{
					bX = editor.Align == ButtonRowAlign.Left ? editor.ButtonPanel.X : editor.ButtonPanel.Right;

					foreach (var button in editor.Buttons)
					{
						if (editor.Align == ButtonRowAlign.Left)
						{
							button.Rect.X = bX;
							bX += button.Rect.Width + editor.SpacingX;
						}
						else if (editor.Align == ButtonRowAlign.Right)
						{
							button.Rect.X = bX - button.Rect.Width;
							bX -= button.Rect.Width + editor.SpacingX;
						}

						button.Rect.Y = bY;

						ButtonState bstate = ButtonState.Disabled;
						if (editor.Editor.Enabled)
						{
							if (button.Pressed) bstate = ButtonState.Pressed;
							else if (button.Hovered) bstate = ButtonState.Hot;
							else bstate = ButtonState.Normal;
						}

						ApplyColors(button, ControlRenderer);
						ControlRenderer.DrawButton(e.Graphics, button.Rect, bstate, button.Label);
						ResetColors(ControlRenderer);
					}
				}
			}
		}

		public static void OnMouseMove(IButtonPropertyEditor editor, MouseEventArgs e)
		{
			if (editor.Buttons.Count > 0)
			{
				foreach (var button in editor.Buttons)
				{
					var lastHovered = button.Hovered;
					button.Hovered = button.Rect.Contains(e.Location);
				}
			}
		}

		public static void OnMouseLeave(IButtonPropertyEditor editor, EventArgs e)
		{
			if (editor.Buttons.Count > 0)
			{
				foreach (var button in editor.Buttons)
				{
					button.Hovered = false;
				}
			}
		}

		public static void OnMouseDown(IButtonPropertyEditor editor, MouseEventArgs e)
		{
			if (editor.Buttons.Count > 0)
			{
				foreach (var button in editor.Buttons)
				{
					if (button.Hovered && (e.Button & MouseButtons.Left) != MouseButtons.None)
					{
						button.Pressed = true;
					}
				}
			}
		}

		public static void OnClick(IButtonPropertyEditor editor, MouseEventArgs e)
		{
			if (editor.Buttons.Count > 0)
			{
				foreach (var button in editor.Buttons)
				{
					if (button.Hovered && (e.Button & MouseButtons.Left) != MouseButtons.None)
					{
						if (button.Pressed && button.Hovered)
						{
							if (button.Value != null)
							{
								button.Value.OnClick?.Invoke();
								RefreshAffectedProperty(editor, button.Value);
							}
						}

						button.Pressed = false;
					}
				}
			}
		}
	}
}