using AdamsLair.WinForms.PropertyEditing;
using EditorButtons.Editor.Backgrounds;
using System.Collections.Generic;
using System.Drawing;

namespace EditorButtons.Editor.Buttons
{
	public interface IButtonPropertyEditor
	{
		int TotalWidth { get; set; }
		int SpacingX { get; set; }

		List<ButtonProperty> Buttons { get; }

		Rectangle ButtonPanel { get; set; }

		ButtonRowAlign Align { get; set; }

		PropertyEditor Editor { get; }

		IButtonContainer Container { get; set; }

		IButtonBackground Background { get; set; }
		IBrushSettings<Brush> BrushSettings { get; set; }
	}
}