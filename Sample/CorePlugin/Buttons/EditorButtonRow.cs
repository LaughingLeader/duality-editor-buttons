using EditorButtons.Editor;
using EditorButtons.Editor.Backgrounds;
using System.Collections.Generic;

namespace ButtonSample.Buttons
{
	public class EditorButtonRow : IButtonRow
	{
		public float HeightPercentage { get; set; } = 1;
		public IButtonBackground Background { get; set; }
		public ButtonRowAlign Align { get; set; }
		public List<IButtonValue> Buttons { get; set; }

		public void Add(IButtonValue button)
		{
			Buttons.Add(button);
		}

		public EditorButtonRow() { Buttons = new List<IButtonValue>(); }

		public EditorButtonRow(ButtonRowAlign align)
		{
			Buttons = new List<IButtonValue>();
			Align = align;
		}


		public EditorButtonRow(List<IButtonValue> buttons, ButtonRowAlign align)
		{
			Buttons = buttons;
			Align = align;
		}
	}
}