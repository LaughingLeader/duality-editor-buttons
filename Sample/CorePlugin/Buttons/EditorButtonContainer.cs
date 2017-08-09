using EditorButtons.Editor;
using System.Collections.Generic;

namespace ButtonSample.Buttons
{
	public class EditorButtonContainer : IButtonContainer
	{
		public ButtonRowAlign DefaultAlign { get; set; }
		public bool ShowPropertyName { get; set; } = false;
		public bool Collapsible { get; set; } = true;
		public string HeaderName { get; set; }
		public List<IButtonRow> Rows { get; set; }
		public int ButtonSpacingX { get; set; }

		public bool Dirty { get; set; }

		public EditorButtonContainer(string Name = "", ButtonRowAlign Align = ButtonRowAlign.Left, int SpacingX = 1)
		{
			DefaultAlign = Align;
			HeaderName = Name;
			Rows = new List<IButtonRow>();

			IButtonRow row0 = new EditorButtonRow(DefaultAlign);
			Rows.Add(row0);
			ButtonSpacingX = SpacingX;
		}
	}
}