using Duality;
using EditorButtons.Buttons;
using EditorButtons.Buttons.Backgrounds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorButtons.Editor
{
	public class EditorButton : IButtonValue
	{
		public float WidthPercentage { get; set; }
		public float HeightPercentage { get; set; }
		public string ButtonLabel { get; set; }

		public Action OnClick { get; set; }

		public ButtonColors Colors { get; set; }

		public List<GameObject> AdditionalUpdates { get; set; }

		public EditorButton()
		{
			WidthPercentage = 1;
			HeightPercentage = 1;
		}

		public EditorButton(string Label, Action OnButtonHit, float WidthPerc = 1, List<GameObject> GameObjectsToRefresh = null)
		{
			ButtonLabel = Label;
			OnClick = OnButtonHit;

			WidthPercentage = WidthPerc;
			HeightPercentage = 1;

			if (GameObjectsToRefresh != null)
			{
				AdditionalUpdates = GameObjectsToRefresh;
			}
		}
	}
}