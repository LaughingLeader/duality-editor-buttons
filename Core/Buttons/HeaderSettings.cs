using Duality;
using Duality.Drawing;
using Duality.Resources;

namespace EditorButtons.Buttons
{
	public enum ContainerHeaderStyle
	{
		Flat = 0,
		Simple = 1,
		Emboss = 2,
		SmoothSunken = 3
	}

	public class HeaderSettings
	{
		public string HeaderLabel { get; set; }
		public string PropertyLabel { get; set; }
		public ColorRgba Color { get; set; }
		public int Height { get; set; }

		public ContainerHeaderStyle Style { get; set; }

		public ContentRef<Pixmap> Icon { get; set; }

		public bool ShowPropertyLabel { get; set; }
		public bool ShowHeaderLabel { get; set; }

		public HeaderSettings()
		{
			HeaderLabel = "";
			PropertyLabel = "";
			Height = -1;
			ShowPropertyLabel = false;
			ShowHeaderLabel = true;
		}
	}
}
