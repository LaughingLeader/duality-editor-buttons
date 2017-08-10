using AdamsLair.WinForms.PropertyEditing;
using Duality;
using Duality.Drawing;
using Duality.Editor;
using EditorButtons.Editor.Backgrounds;
using EditorButtons.Editor.Buttons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
		private int originalHeaderHeight = 0;

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

			if (value.HeaderSettings != null)
			{
				if (value.HeaderSettings.HeaderLabel != null)
				{
					
					this.HeaderValueText = value.HeaderSettings.HeaderLabel;
				}

				if(value.HeaderSettings.PropertyLabel != null)
				{
					this.PropertyName = value.HeaderSettings.PropertyLabel;
				}

				if (value.HeaderSettings.Color != default(ColorRgba))
				{
					this.HeaderColor = value.HeaderSettings.Color.ToSysDrawColor();
				}

				this.HeaderStyle = (GroupHeaderStyle)value.HeaderSettings.Style;

				if (value.HeaderSettings.Icon != null && value.HeaderSettings.Icon.IsAvailable)
				{
					this.HeaderIcon = value.HeaderSettings.Icon.Res.MainLayer.ToBitmap();
				}

				if(!value.HeaderSettings.ShowHeaderLabel)
				{
					this.HeaderHeight = 0;
				}
				else if (value.HeaderSettings.Height > -1)
				{
					this.HeaderHeight = value.HeaderSettings.Height;
				}
				else
				{
					if (this.HeaderHeight <= 0) this.HeaderHeight = originalHeaderHeight;
				}


				if (value.HeaderSettings.ShowPropertyLabel)
				{
					Hints |= HintFlags.HasPropertyName;
				}
				else
				{
					Hints = Hints & ~HintFlags.HasPropertyName;
				}
			}

			this.Collapsible = value.Collapsible;

			align = value.DefaultAlign;
			spacingX = value.ButtonSpacingX;
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

					if (value.Rows.Count > 0)
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

				if (buttonPropertyRows.Count > 0)
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

				if (originalHeaderHeight <= 0) originalHeaderHeight = this.HeaderHeight;
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

			if (buttonPropertyRows.Count == 0)
			{
				ButtonPropertyMethods.OnPaint(this, e, this.ControlRenderer);
			}

			if (data != null && data.Dirty)
			{
				data.Dirty = false;
				ButtonPropertyMethods.RefreshAffectedProperty(this);
			}
		}

		private void OnObjectPropertyChanged(object sender, ObjectPropertyChangedEventArgs e)
		{
			if (data != null && data.Dirty)
			{
				InitButtonRows(data);
				data.Dirty = false;
				ButtonPropertyMethods.RefreshAffectedProperty(this);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (buttonPropertyRows.Count == 0) ButtonPropertyMethods.OnMouseMove(this, e);

			this.Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			if (buttonPropertyRows.Count == 0) ButtonPropertyMethods.OnMouseLeave(this, e);

			this.Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (buttonPropertyRows.Count == 0) ButtonPropertyMethods.OnMouseDown(this, e);

			this.Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (buttonPropertyRows.Count == 0) ButtonPropertyMethods.OnClick(this, e);

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

			DualityEditorApp.ObjectPropertyChanged += OnObjectPropertyChanged;
		}

		public override void InitContent()
		{
			base.InitContent();
		}
	}
}