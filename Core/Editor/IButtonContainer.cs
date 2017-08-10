﻿using System.Collections.Generic;

namespace EditorButtons.Editor
{
	public interface IButtonContainer
	{
		ButtonRowAlign DefaultAlign { get; set; }
		bool Collapsible { get; set; }
		HeaderSettings HeaderSettings { get; set; }

		List<IButtonRow> Rows { get; set; }

		int ButtonSpacingX { get; set; }

		/// <summary>
		/// Notifies the ButtonContainerPropertyEditor to rebuild the buttons.
		/// </summary>
		bool Dirty { get; set; }
	}
}