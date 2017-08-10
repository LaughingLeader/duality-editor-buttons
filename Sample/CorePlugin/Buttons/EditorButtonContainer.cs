using EditorButtons.Editor;
using System.Collections.Generic;

namespace ButtonSample.Buttons
{
	public class EditorButtonContainer : IButtonContainer
	{
		public ButtonRowAlign DefaultAlign { get; set; }
		public bool ShowPropertyName { get; set; } = false;
		public HeaderSettings HeaderSettings { get; set; }
		public List<IButtonRow> Rows { get; set; }
		public int ButtonSpacingX { get; set; }
		public bool Collapsible { get; set; }
		public bool Dirty { get; set; }

		public int Indent { get; set; }

		public EditorButtonContainer(string Name = "", ButtonRowAlign Align = ButtonRowAlign.Left, int SpacingX = 1)
		{
			Indent = 0;
			DefaultAlign = Align;
			HeaderSettings = new HeaderSettings
			{
				HeaderLabel = Name
			};

			Rows = new List<IButtonRow>();

			IButtonRow row0 = new EditorButtonRow(new List<IButtonValue>(), DefaultAlign);
			Rows.Add(row0);
			ButtonSpacingX = SpacingX;
		}
	}
}