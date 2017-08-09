using Duality;
using System;
using System.Collections.Generic;

namespace EditorButtons.Editor
{
	public interface IButtonValue
	{
		float WidthPercentage { get; set; }
		float HeightPercentage { get; set; }

		string ButtonLabel { get; }
		Action OnClick { get; set; }

		ButtonColors Colors { get; set; }

		/// <summary>
		/// Additional GameObjects to refresh once a button is clicked.
		/// </summary>
		List<GameObject> AdditionalUpdates { get; set; }
	}
}