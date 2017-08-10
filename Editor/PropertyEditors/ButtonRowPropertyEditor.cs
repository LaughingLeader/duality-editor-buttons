using AdamsLair.WinForms.PropertyEditing;
using Duality;
using EditorButtons.Editor.Backgrounds;
using EditorButtons.Editor.Buttons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ButtonState = AdamsLair.WinForms.Drawing.ButtonState;

namespace EditorButtons.Editor.PropertyEditors
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
		public int TotalWidth { get => totalWidth; set => totalWidth = value; }
		public int SpacingX { get => spacingX; set => spacingX = value; }
		public ButtonRowAlign Align { get => align; set => align = value; }

		public PropertyEditor Editor { get => this; }

		public IButtonContainer Container { get => parentContainer?.Data; set { } }

		public IButtonBackground Background { get; set; }
		public IBrushSettings<Brush> BrushSettings { get; set; }

		public override object DisplayedValue
		{
			get { return this.GetValue(); }
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

		//For triggering painting from the container.
		internal void PaintRow(PaintEventArgs e)
		{
			this.OnPaint(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (BrushSettings == null && Background != null)
			{
				BrushSettings = ButtonPropertyMethods.PrepareBrush(Background);
			}

			if (BrushSettings != null && BrushSettings.Brush != null)
			{
				ButtonPropertyMethods.FillBackground(ButtonPanel, e, BrushSettings.Brush, Background.Outline);
			}
			else
			{
				ButtonPropertyMethods.PaintSolid(ButtonPanel, e, null, Background.Outline);
			}

			if (ButtonPanel != null)
			{
				var editorRect = ButtonPanel;
				editorRect.Y = Editor.ClientRectangle.Y;
				ButtonPanel = editorRect;
			}

			if (Buttons?.Count > 0)
			{
				if (TotalWidth == 0) TotalWidth = ButtonPropertyMethods.CalculateTotalWidth(this);

				int bX = ButtonPanel.X, bY = ButtonPanel.Y;

				if (Align == ButtonRowAlign.Center)
				{
					bX = ButtonPanel.X + ((ButtonPanel.Width - TotalWidth) / 2);

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

						bX += button.Rect.Width + SpacingX;
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
							bX += button.Rect.Width + SpacingX;
						}
						else if (Align == ButtonRowAlign.Right)
						{
							button.Rect.X = bX - button.Rect.Width;
							bX -= button.Rect.Width + SpacingX;
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
			this.RowData = rowData;
			this.Background = RowData.Background;
			this.parentContainer = ParentContainer;
			this.Buttons = rowButtons;
			this.SpacingX = rowData.ButtonSpacing;
			this.Align = RowData.Align;

			var mask = ~HintFlags.HasPropertyName;
			this.Hints = this.Hints & mask;

			this.PropertyName = ParentContainer.PropertyName;
		}
	}
}