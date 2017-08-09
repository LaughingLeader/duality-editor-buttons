using AdamsLair.WinForms.PropertyEditing;
using Duality;
using Duality.Editor;
using EditorButtons.Editor.Backgrounds;
using EditorButtons.Editor.Buttons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EditorButtons.Editor.PropertyEditors
{
	[PropertyEditorAssignment(typeof(IButtonContainer))]
	public class ButtonContainerPropertyEditor : GroupedPropertyEditor, IButtonPropertyEditor
	{
		protected Rectangle buttonPanel = Rectangle.Empty;
		protected List<List<ButtonProperty>> buttonPropertyRows;
		private List<ButtonRowPropertyEditor> rowEditors;
		private ButtonRowAlign align;
		private int totalWidth = 0;
		private int spacingX;

		private IButtonContainer data;

		#region IButtonPropertyEditor

		public Rectangle ButtonPanel { get => buttonPanel; set => buttonPanel = value; }
		public List<ButtonProperty> Buttons { get => buttonPropertyRows?.FirstOrDefault(); }
		public int TotalWidth { get => totalWidth; set => totalWidth = value; }
		public int SpacingX { get => spacingX; set => spacingX = value; }
		public ButtonRowAlign Align { get => align; set => align = value; }

		public PropertyEditor Editor { get => this; }

		public IButtonContainer Container { get; set; }

		public IButtonBackground Background { get; set; }
		public IBrushSettings<Brush> BrushSettings { get; set; }

		#endregion IButtonPropertyEditor

		public bool Collapsible { get; set; } = true;

		public override object DisplayedValue
		{
			get { return this.GetValue(); }
		}

		protected void ApplyData(IButtonContainer value)
		{
			this.Container = value;
			this.PropertyName = value.HeaderName;
			this.Collapsible = value.Collapsible;

			align = value.DefaultAlign;
			spacingX = value.ButtonSpacingX;

			if (value.ShowPropertyName)
			{
				Hints |= HintFlags.HasPropertyName;
			}
			else
			{
				Hints = Hints & ~HintFlags.HasPropertyName;
			}
		}

		protected void InitButtonRows(IButtonContainer value)
		{
			if (rowEditors == null) rowEditors = new List<ButtonRowPropertyEditor>();

			if (value != null)
			{
				ApplyData(value);

				if (rowEditors.Count > 0)
				{
					ClearPropertyEditors();
					rowEditors.Clear();
				}

				if (buttonPropertyRows.Count > 0) buttonPropertyRows.Clear();

				var rowNum = 0;
				foreach (var row in value.Rows)
				{
					List<ButtonProperty> buttonRow = new List<ButtonProperty>();

					if (row.Buttons != null && row.Buttons.Count > 0)
					{
						foreach (var button in row.Buttons)
						{
							if (!string.IsNullOrEmpty(button.ButtonLabel))
							{
								var buttonEntry = new ButtonProperty()
								{
									Label = button.ButtonLabel,
									Value = button
								};
								buttonEntry.Rect.Width = MathF.RoundToInt(buttonPanel.Width * button.WidthPercentage);
								buttonEntry.Rect.Height = MathF.RoundToInt(buttonPanel.Height * button.HeightPercentage);

								buttonRow.Add(buttonEntry);
							}
						}
					}

					buttonPropertyRows.Add(buttonRow);

					if (value.Rows.Count > 1)
					{
						var rowEditor = new ButtonRowPropertyEditor(this, buttonRow, row, value.ButtonSpacingX);
						if (!Collapsible)
						{
							rowEditor.Hints = Hints & ~HintFlags.HasExpandCheck;
							rowEditor.Hints |= HintFlags.ExpandEnabled;
						}

						this.AddPropertyEditor(rowEditor);
						this.ParentGrid.ConfigureEditor(rowEditor);
						rowEditors.Add(rowEditor);
					}
					else
					{
						this.Background = row.Background;
					}

					rowNum++;
				}

				if (buttonPropertyRows.Count > 1)
				{
					if (Collapsible)
					{
						this.Hints |= HintFlags.HasExpandCheck;
					}
					else
					{
						this.Hints = Hints & ~HintFlags.HasExpandCheck;
					}
					this.Hints |= HintFlags.ExpandEnabled;
					//Flip the bool a bit to render the buttons already expanded.
					this.Expanded = true;
					this.Expanded = false;
					this.Expanded = true;
				}
				else
				{
					this.Expanded = false;
					Hints = Hints & ~HintFlags.HasExpandCheck;
					Hints = Hints & ~HintFlags.ExpandEnabled;
				}

				if (value.Dirty) value.Dirty = false;
			}
		}

		protected override void UpdateGeometry()
		{
			base.UpdateGeometry();

			this.buttonPanel = new Rectangle(
				this.ClientRectangle.X,
				this.ClientRectangle.Y,
				this.ClientRectangle.Width,
				this.ClientRectangle.Height);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (data == null) data = this.GetValue().Cast<IButtonContainer>().FirstOrDefault();

			if (data != null && (data.Dirty || buttonPropertyRows.Count == 0))
			{
				InitButtonRows(data);
			}

			base.OnPaint(e);

			if (buttonPropertyRows.Count == 1) ButtonPropertyMethods.OnPaint(this, e, this.ControlRenderer);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (buttonPropertyRows.Count == 1) ButtonPropertyMethods.OnMouseMove(this, e);

			this.Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			if (buttonPropertyRows.Count == 1) ButtonPropertyMethods.OnMouseLeave(this, e);

			this.Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (buttonPropertyRows.Count == 1) ButtonPropertyMethods.OnMouseDown(this, e);

			this.Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (buttonPropertyRows.Count == 1) ButtonPropertyMethods.OnClick(this, e);

			this.Invalidate();
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			ButtonPropertyMethods.OnClick(this, e);

			this.Invalidate();
		}

		public ButtonContainerPropertyEditor()
		{
			buttonPropertyRows = new List<List<ButtonProperty>>();
			this.Hints = HintFlags.None;
			this.Indent = 0;
		}

		public override void InitContent()
		{
			base.InitContent();
		}
	}
}