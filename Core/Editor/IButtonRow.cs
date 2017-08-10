using EditorButtons.Editor.Backgrounds;
using System.Collections.Generic;

namespace EditorButtons.Editor
{
	public enum ButtonRowAlign
	{
		Left,
		Center,
		Right
	}

	public interface IButtonRow
	{
		ButtonRowAlign Align { get; set; }
		List<IButtonValue> Buttons { get; set; }

		IButtonBackground Background { get; set; }

		int ButtonSpacing { get; set; }

		float HeightPercentage { get; set; }
	}
}