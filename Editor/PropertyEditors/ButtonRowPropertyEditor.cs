using AdamsLair.WinForms.PropertyEditing;
using Duality;
using EditorButtons.Buttons;
using EditorButtons.Buttons.Backgrounds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ButtonState = AdamsLair.WinForms.Drawing.ButtonState;

namespace EditorButtons.PropertyEditors
{
	public class ButtonRowPropertyEditor : PropertyEditor
	{
		private ButtonContainerPropertyEditor parentContainer;
		private Rectangle buttonPanel = Rectangle.Empty;
		private List<ButtonProperty> buttons;
		private int totalWidth = 0;
		private int spacingX;
		private ButtonRowAlign align;
		private int originalHeight = -1;
		private IButtonRow rowData;

		public IButtonRow RowData { get => rowData; set => rowData = value; }

		public Rectangle ButtonPanel { get => buttonPanel; set => buttonPanel = value; }
		public List<ButtonProperty> Buttons { get => buttons; set => buttons = value; }
		public ButtonRowAlign Align { get => align; set => align = value; }

		public PropertyEditor Editor { get => this; }

		public IButtonContainer Container { get => parentContainer?.Data; set { } }

		public IButtonBackground Background { get; set; }
		public IBrushSettings<Brush> BrushSettings { get; set; }

		public int RowNum { get; set; }

		public override object DisplayedValue
		{
			get { return this.GetValue(); }
		}

		public void SetButtonSizes()
		{
			totalWidth = 0;
			var totalSpacing = RowData.Buttons.Count * RowData.ButtonSpacing;
			var rowWidth = (buttonPanel.Width * RowData.WidthPercentage);

			foreach (var buttonEntry in buttons)
			{
				if (buttonEntry.Value.WidthPercentage > 1) buttonEntry.Value.WidthPercentage = 1;
				if (buttonEntry.Value.HeightPercentage > 1) buttonEntry.Value.HeightPercentage = 1;
				if (buttonEntry.Value.WidthPercentage < 0) buttonEntry.Value.WidthPercentage = 0;
				if (buttonEntry.Value.HeightPercentage < 0) buttonEntry.Value.HeightPercentage = 0;

				if (RowData.ScaleMode == ButtonRowScaleMode.Manual)
				{
					buttonEntry.Rect.Width = MathF.RoundToInt(rowWidth * buttonEntry.Value.WidthPercentage);
					buttonEntry.Rect.Height = MathF.RoundToInt((buttonPanel.Height * RowData.HeightPercentage) * buttonEntry.Value.HeightPercentage);
				}
				else
				{
					var maxButtonWidth = (rowWidth + totalSpacing) / RowData.Buttons.Count;
					buttonEntry.Rect.Width = MathF.RoundToInt(maxButtonWidth * buttonEntry.Value.WidthPercentage);
					buttonEntry.Rect.Height = MathF.RoundToInt((buttonPanel.Height * RowData.HeightPercentage) * buttonEntry.Value.HeightPercentage);

					//Log.Editor.Write($"Row[{RowNum}:{buttons.Count}:{i}] Set width: {buttonEntry.Rect.Width} | max[{maxButtonWidth}] " +
					//	$"buttonPanel[{buttonPanel.Width}] totalSpacing[{totalSpacing}] buttonWidth%[{buttonEntry.Value.WidthPercentage}]");
				}

				totalWidth += buttonEntry.Rect.Width;
			}
		}

		public void UpdateRectangles()
		{
			if (RowData != null && RowData.HeightPercentage < 1)
			{
				if (originalHeight < 0) originalHeight = this.ClientRectangle.Height;
				this.ClientRectangle = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y,
					this.ClientRectangle.Width, MathF.RoundToInt(originalHeight * RowData.HeightPercentage));
			}

			this.buttonPanel = new Rectangle(
				this.ClientRectangle.X,
				this.ClientRectangle.Y,
				this.ClientRectangle.Width,
				this.ClientRectangle.Height);
		}

		protected override void UpdateGeometry()
		{
			base.UpdateGeometry();
			UpdateRectangles();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (BrushSettings == null && Background != null)
			{
				BrushSettings = ButtonPropertyMethods.PrepareBrush(Background);
			}

			var outline = Background != null ? Background.Outline : true;

			if (BrushSettings != null && BrushSettings.Brush != null)
			{
				ButtonPropertyMethods.FillBackground(ButtonPanel, e, BrushSettings.Brush, outline);
			}
			else
			{
				ButtonPropertyMethods.PaintSolid(ButtonPanel, e, null, outline);
			}

			if (ButtonPanel != null)
			{
				var editorRect = ButtonPanel;
				editorRect.Y = Editor.ClientRectangle.Y;
				ButtonPanel = editorRect;
			}

			if (Buttons?.Count > 0)
			{
				if (totalWidth == 0)
				{
					SetButtonSizes();
				}

				int bX = ButtonPanel.X, bY = ButtonPanel.Y;

				if (Align == ButtonRowAlign.Center)
				{
					bX = ButtonPanel.X + ((ButtonPanel.Width - totalWidth) / 2);

					foreach (var button in Buttons)
					{
						button.Rect.X = bX;
						button.Rect.Y = bY;

						ButtonState bstate = ButtonState.Disabled;
						if (Editor.Enabled)
						{
							if (button.Pressed) bstate = ButtonState.Pressed;
							else if (button.Hovered) bstate = ButtonState.Hot;
							else bstate = ButtonState.Normal;
						}

						ButtonPropertyMethods.ApplyColors(button, ControlRenderer);
						ControlRenderer.DrawButton(e.Graphics, button.Rect, bstate, button.Label);
						ButtonPropertyMethods.ResetColors(ControlRenderer);

						bX += button.Rect.Width + spacingX;
					}
				}
				else
				{
					bX = Align == ButtonRowAlign.Left ? ButtonPanel.X : ButtonPanel.Right;

					foreach (var button in Buttons)
					{
						if (Align == ButtonRowAlign.Left)
						{
							button.Rect.X = bX;
							bX += button.Rect.Width + spacingX;
						}
						else if (Align == ButtonRowAlign.Right)
						{
							button.Rect.X = bX - button.Rect.Width;
							bX -= button.Rect.Width + spacingX;
						}

						button.Rect.Y = bY;

						ButtonState bstate = ButtonState.Disabled;
						if (Editor.Enabled)
						{
							if (button.Pressed) bstate = ButtonState.Pressed;
							else if (button.Hovered) bstate = ButtonState.Hot;
							else bstate = ButtonState.Normal;
						}

						ButtonPropertyMethods.ApplyColors(button, ControlRenderer);
						ControlRenderer.DrawButton(e.Graphics, button.Rect, bstate, button.Label);
						ButtonPropertyMethods.ResetColors(ControlRenderer);
					}
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (Buttons?.Count > 0)
			{
				foreach (var button in Buttons)
				{
					var lastHovered = button.Hovered;
					button.Hovered = button.Rect.Contains(e.Location);
				}
			}

			this.Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			if (Buttons?.Count > 0)
			{
				foreach (var button in Buttons)
				{
					button.Hovered = false;
				}
			}

			this.Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (Buttons?.Count > 0)
			{
				foreach (var button in Buttons)
				{
					if (button.Hovered && (e.Button & MouseButtons.Left) != MouseButtons.None)
					{
						button.Pressed = true;
					}
				}
			}

			this.Invalidate();
		}

		private void OnClick(MouseEventArgs e)
		{
			if (Buttons?.Count > 0)
			{
				foreach (var button in Buttons)
				{
					if (button.Hovered && (e.Button & MouseButtons.Left) != MouseButtons.None)
					{
						if (button.Pressed && button.Hovered)
						{
							if (button.Value != null)
							{
								button.Value.OnClick?.Invoke();
								ButtonPropertyMethods.RefreshAffectedProperty(this, button.Value);
							}
						}

						button.Pressed = false;
					}
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			OnClick(e);

			this.Invalidate();
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			OnClick(e);

			this.Invalidate();
		}

		public ButtonRowPropertyEditor(ButtonContainerPropertyEditor ParentContainer, List<ButtonProperty> rowButtons, IButtonRow rowData)
		{
			this.rowData = rowData;
			this.Background = RowData.Background;
			this.parentContainer = ParentContainer;
			this.buttons = rowButtons;
			this.spacingX = rowData.ButtonSpacing;
			this.align = RowData.Align;

			var mask = ~HintFlags.HasPropertyName;
			this.Hints = this.Hints & mask;

			this.PropertyName = ParentContainer.PropertyName;
		}
	}
}