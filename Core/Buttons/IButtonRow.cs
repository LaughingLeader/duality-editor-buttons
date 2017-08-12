using EditorButtons.Buttons.Backgrounds;
using System.Collections.Generic;

namespace EditorButtons.Buttons
{
	public enum ButtonRowAlign
	{
		Left,
		Center,
		Right
	}

	public enum ButtonRowScaleMode
	{
		Auto,
		Manual
	}

	public interface IButtonRow
	{
		ButtonRowAlign Align { get; set; }
		List<IButtonValue> Buttons { get; set; }

		IButtonBackground Background { get; set; }

		int ButtonSpacing { get; set; }

		float HeightPercentage { get; set; }
		float WidthPercentage { get; set; }

		ButtonRowScaleMode ScaleMode { get; set; }
	}
}