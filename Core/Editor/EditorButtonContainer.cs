using EditorButtons.Buttons;
using EditorButtons.Buttons.Backgrounds;
using System.Collections.Generic;
using System.Linq;

namespace EditorButtons.Editor
{
	public class EditorButtonContainer : IButtonContainer
	{
		public List<IButtonRow> Rows { get; set; }
		public HeaderSettings HeaderSettings { get; set; }
		public int Indent { get; set; }
		public bool Collapsible { get; set; }
		public bool Dirty { get; set; }

		public EditorButtonContainer(string Name = "", ButtonRowAlign Align = ButtonRowAlign.Center, List<IButtonRow> rows = null)
		{
			Indent = 0;
			HeaderSettings = new HeaderSettings
			{
				HeaderLabel = Name
			};

			if (rows != null)
			{
				Rows = rows;
			}
			else
			{
				Rows = new List<IButtonRow>() { new EditorButtonRow() };
			}
		}
	}
}