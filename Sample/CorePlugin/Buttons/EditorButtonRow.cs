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

		public EditorButtonRow(ButtonRowAlign align = ButtonRowAlign.Left)
		{
			Align = align;
			Buttons = new List<IButtonValue>();
		}
	}
}