using AdamsLair.WinForms.PropertyEditing;
using Duality;
using Duality.Drawing;
using Duality.Editor;
using EditorButtons.Buttons;
using EditorButtons.Buttons.Backgrounds;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EditorButtons.PropertyEditors
{
	[PropertyEditorAssignment(typeof(IButtonContainer))]
	public class ButtonContainerPropertyEditor : GroupedPropertyEditor
	{
		private IButtonContainer data;
		protected Rectangle backgroundPanel = Rectangle.Empty;
		protected List<List<ButtonProperty>> buttonPropertyRows;
		private List<ButtonRowPropertyEditor> rowEditors;
		private int originalHeaderHeight = 0;
		private bool propertyChanged = false;
		private bool initialized = false;
		private bool dataInitialized = false;

		public Rectangle BackgroundPanel { get => backgroundPanel; set => backgroundPanel = value; }
		public IButtonBackground Background { get; set; }
		public IBrushSettings<Brush> BrushSettings { get; set; }

		public bool Collapsible { get; set; } = true;

		public override object DisplayedValue
		{
			get { return this.GetValue(); }
		}

		public IButtonContainer Data { get => data; set => data = value; }

		protected void ApplyData(IButtonContainer value)
		{
			this.Indent = value.Indent;

			if (value.HeaderSettings != null)
			{
				if (value.HeaderSettings.HeaderLabel != null)
				{
					this.HeaderValueText = value.HeaderSettings.HeaderLabel;
				}

				if (value.HeaderSettings.PropertyLabel != null)
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
				else
				{
					this.HeaderIcon = null;
				}

				if (!value.HeaderSettings.ShowHeaderLabel)
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
		}

		protected void InitButtonRows(IButtonContainer value)
		{
			if (rowEditors == null) rowEditors = new List<ButtonRowPropertyEditor>();

			if (value != null)
			{
				ApplyData(value);

				//Log.Editor.Write($"Initializing button rows for [{this.PropertyName}|{this.HeaderValueText}]");

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

					if (row.Align == default(ButtonRowAlign))
					{
						row.Align = value.DefaultAlign;
					}

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

								buttonRow.Add(buttonEntry);
							}
						}
					}

					buttonPropertyRows.Add(buttonRow);

					if (value.Rows.Count > 0)
					{
						var rowEditor = new ButtonRowPropertyEditor(this, buttonRow, row);
						rowEditor.RowNum = rowNum;
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

			this.backgroundPanel = new Rectangle(
				this.ClientRectangle.X,
				this.ClientRectangle.Y,
				this.ClientRectangle.Width,
				this.ClientRectangle.Height);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (buttonPropertyRows.Count == 0)
			{
				if (BrushSettings == null && Background != null)
				{
					BrushSettings = ButtonPropertyMethods.PrepareBrush(Background);
				}

				var outline = Background != null ? Background.Outline : true;

				if (BrushSettings != null && BrushSettings.Brush != null)
				{
					ButtonPropertyMethods.FillBackground(BackgroundPanel, e, BrushSettings.Brush, outline);
				}
				else
				{
					ButtonPropertyMethods.PaintSolid(BackgroundPanel, e, null, outline);
				}
			}

			if (Data == null) Data = GetValue()?.Cast<IButtonContainer>().FirstOrDefault();

			if (Data != null && Data.Dirty || propertyChanged)
			{
				if (Data.Dirty) Data.Dirty = false;
				if (propertyChanged) propertyChanged = false;
				ButtonPropertyMethods.RefreshAffectedProperty(this);
			}
		}

		private void OnObjectPropertyChanged(object sender, ObjectPropertyChangedEventArgs e)
		{
			if (Data != null && Data.Dirty && !propertyChanged)
			{
				//Log.Editor.WriteWarning($"{this.HeaderValueText} is dirty. Updating.");
				InitButtonRows(Data);
				propertyChanged = true;
			}
		}

		public ButtonContainerPropertyEditor()
		{
			buttonPropertyRows = new List<List<ButtonProperty>>();
			Hints = HintFlags.None;
		}

		protected override void OnParentEditorChanged()
		{
			base.OnParentEditorChanged();

			if (!initialized)
			{
				//Log.Editor.Write($"ParentEditor changed for {this.PropertyName} Active {this.Active} | Added event.");
				DualityEditorApp.ObjectPropertyChanged += OnObjectPropertyChanged;
				initialized = true;
			}

			if (!dataInitialized && this.GetValue() != null)
			{
				Data = this.GetValue()?.Cast<IButtonContainer>().FirstOrDefault();

				if (Data != null)
				{
					InitButtonRows(Data);
					dataInitialized = true;
				}
			}
		}

		protected override void OnDisposing(bool manually)
		{
			if (initialized)
			{
				//Log.Editor.Write($"{this.PropertyName} is Disposing | Removed event.");
				DualityEditorApp.ObjectPropertyChanged -= OnObjectPropertyChanged;
				initialized = false;
				dataInitialized = false;
			}

			base.OnDisposing(manually);
		}
	}
}