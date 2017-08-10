using System.Collections.Generic;

namespace EditorButtons.Editor
{
	public interface IButtonContainer
	{	
		HeaderSettings HeaderSettings { get; set; }

		List<IButtonRow> Rows { get; set; }

		bool Collapsible { get; set; }

		int Indent { get; set; }

		/// <summary>
		/// Notifies the ButtonContainerPropertyEditor to rebuild and re-paint.
		/// </summary>
		bool Dirty { get; set; }
	}
}