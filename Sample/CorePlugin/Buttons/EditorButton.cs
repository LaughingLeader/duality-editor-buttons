using Duality;
using EditorButtons.Editor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ButtonSample.Buttons
{
	public class EditorButton : IButtonValue
	{
		private string label;
		private ButtonColors colors = null;

		public float WidthPercentage { get; set; }
		public float HeightPercentage { get; set; }
		public string ButtonLabel { get { return label; } }

		public Action OnClick { get; set; }

		public ButtonColors Colors { get => colors; set => colors = value; }

		public List<GameObject> AdditionalUpdates { get; set; }

		public EditorButton(string Label, Action OnButtonHit, float WidthPerc, params GameObject[] GameObjectsToRefresh)
		{
			OnClick = OnButtonHit;
			WidthPercentage = WidthPerc;
			HeightPercentage = 1;
			label = Label;

			if (GameObjectsToRefresh != null)
			{
				AdditionalUpdates = GameObjectsToRefresh.ToList();
			}
			else
			{
				AdditionalUpdates = new List<GameObject>();
			}
		}

		public EditorButton(string Label, Action OnButtonHit, float WidthPerc, float HeightPerc, params GameObject[] GameObjectsToRefresh)
		{
			OnClick = OnButtonHit;
			WidthPercentage = WidthPerc;
			HeightPercentage = HeightPerc;
			label = Label;

			if (GameObjectsToRefresh != null)
			{
				AdditionalUpdates = GameObjectsToRefresh.ToList();
			}
			else
			{
				AdditionalUpdates = new List<GameObject>();
			}
		}
	}
}