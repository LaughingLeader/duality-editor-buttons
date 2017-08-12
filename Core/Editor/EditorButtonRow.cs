using EditorButtons.Buttons;
using EditorButtons.Buttons.Backgrounds;
using System.Collections.Generic;
using System.Linq;

namespace EditorButtons.Editor
{
	public class EditorButtonRow : IButtonRow
	{
		public int ButtonSpacing { get; set; }
		public float HeightPercentage { get; set; } = 1;
		public float WidthPercentage { get; set; } = 1;
		public IButtonBackground Background { get; set; }
		public ButtonRowAlign Align { get; set; }
		public List<IButtonValue> Buttons { get; set; }
		public ButtonRowScaleMode ScaleMode { get; set; }

		public void Add(IButtonValue button)
		{
			if(Buttons != null)
			{
				Buttons.Add(button);
			}
		}

		public void Add(params IButtonValue[] addButtons)
		{
			if (Buttons != null)
			{
				foreach(var button in addButtons)
				{
					Buttons.Add(button);
				}
			}
		}

		public EditorButtonRow(ButtonRowAlign align = ButtonRowAlign.Center)
		{
			Buttons = new List<IButtonValue>();
			Align = align;
		}


		public EditorButtonRow(float widthPercentage = 1, params IButtonValue[] buttons)
		{
			WidthPercentage = widthPercentage;

			if (buttons != null && buttons.Length > 0)
			{
				Buttons = buttons.ToList();
			}
			else
			{
				Buttons = new List<IButtonValue>();
			}
		}

		public EditorButtonRow(List<IButtonValue> buttons, float widthPercentage = 1, ButtonRowAlign align = ButtonRowAlign.Center)
		{
			WidthPercentage = 1;
			Align = align;
			Buttons = buttons;
		}
	}
}