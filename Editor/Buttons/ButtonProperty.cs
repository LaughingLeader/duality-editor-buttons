using System.Drawing;

namespace EditorButtons.Editor.Buttons
{
	public class ButtonProperty
	{
		private string label = "Button";
		private bool hovered = false;
		private bool pressed = false;
		private IButtonValue val;

		public Rectangle Rect = Rectangle.Empty;
		public string Label { get => label; set => label = value; }
		public bool Hovered { get => hovered; set => hovered = value; }
		public bool Pressed { get => pressed; set => pressed = value; }
		public IButtonValue Value { get => val; set => val = value; }

		public ButtonProperty()
		{
		}

		public ButtonProperty(string Label)
		{
			this.Label = Label;
		}
	}
}