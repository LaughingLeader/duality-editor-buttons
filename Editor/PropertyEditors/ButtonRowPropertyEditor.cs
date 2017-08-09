using AdamsLair.WinForms.PropertyEditing;
using Duality;
using EditorButtons.Editor.Backgrounds;
using EditorButtons.Editor.Buttons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EditorButtons.Editor.PropertyEditors
{
	public class ButtonRowPropertyEditor : PropertyEditor, IButtonPropertyEditor
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

		#region IButtonPropertyEditor

		public Rectangle ButtonPanel { get => buttonPanel; set => buttonPanel = value; }
		public List<ButtonProperty> Buttons { get => buttons; set => buttons = value; }
		public int TotalWidth { get => totalWidth; set => totalWidth = value; }
		public int SpacingX { get => spacingX; set => spacingX = value; }
		public ButtonRowAlign Align { get => align; set => align = value; }

		public PropertyEditor Editor { get => this; }

		public IButtonContainer Container { get => parentContainer.Container; set { } }

		public IButtonBackground Background { get; set; }
		public IBrushSettings<Brush> BrushSettings { get; set; }

		#endregion IButtonPropertyEditor

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

			ButtonPropertyMethods.OnPaint(this, e, this.ControlRenderer);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			ButtonPropertyMethods.OnMouseMove(this, e);

			this.Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			ButtonPropertyMethods.OnMouseLeave(this, e);

			this.Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			ButtonPropertyMethods.OnMouseDown(this, e);

			this.Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			ButtonPropertyMethods.OnClick(this, e);

			this.Invalidate();
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			ButtonPropertyMethods.OnClick(this, e);

			this.Invalidate();
		}

		public ButtonRowPropertyEditor(ButtonContainerPropertyEditor ParentContainer, List<ButtonProperty> rowButtons, IButtonRow rowData, int bSpacingX = 1)
		{
			this.RowData = rowData;
			this.Background = RowData.Background;
			this.parentContainer = ParentContainer;
			this.Buttons = rowButtons;
			this.SpacingX = bSpacingX;
			this.Align = RowData.Align;

			var mask = ~HintFlags.HasPropertyName;
			this.Hints = this.Hints & mask;

			this.PropertyName = ParentContainer.PropertyName;
		}
	}
}