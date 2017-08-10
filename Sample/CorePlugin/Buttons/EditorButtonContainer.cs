using EditorButtons.Editor;
using System.Collections.Generic;

namespace ButtonSample.Buttons
{
	public class EditorButtonContainer : IButtonContainer
	{
		public List<IButtonRow> Rows { get; set; }
		public HeaderSettings HeaderSettings { get; set; }
		public int Indent { get; set; }
		public bool Collapsible { get; set; }
		public bool Dirty { get; set; }

		public EditorButtonContainer(string Name = "", ButtonRowAlign Align = ButtonRowAlign.Left)
		{
			Indent = 0;
			HeaderSettings = new HeaderSettings
			{
				HeaderLabel = Name
			};

			Rows = new List<IButtonRow>();

			IButtonRow row0 = new EditorButtonRow(new List<IButtonValue>(), Align);
			Rows.Add(row0);
		}
	}
}