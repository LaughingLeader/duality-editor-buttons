using Duality;
using Duality.Drawing;
using Duality.Resources;
using EditorButtons.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSample.Buttons
{
	[DontSerialize]
	public class HeaderChanger
	{
		private HeaderSettings target;
		private Action OnPropertyChanged;


		public string HeaderLabel
		{
			get
			{
				return target.HeaderLabel;
			}
			set
			{
				if (target.HeaderLabel != value)
				{
					target.HeaderLabel = value;
					OnPropertyChanged?.Invoke();
				}
			}
		}

		public string PropertyLabel
		{
			get
			{
				return target.PropertyLabel;
			}
			set
			{
				if (target.PropertyLabel != value)
				{
					target.PropertyLabel = value;
					OnPropertyChanged?.Invoke();
				}
			}
		}

		public ColorRgba Color
		{
			get
			{
				return target.Color;
			}
			set
			{
				if (target.Color != value)
				{
					target.Color = value;
					OnPropertyChanged?.Invoke();
				}
			}
		}

		public int Height
		{
			get
			{
				return target.Height;
			}
			set
			{
				if (target.Height != value)
				{
					target.Height = value;
					OnPropertyChanged?.Invoke();
				}
			}
		}

		public ContainerHeaderStyle Style
		{
			get
			{
				return target.Style;
			}
			set
			{
				if (target.Style != value)
				{
					target.Style = value;
					OnPropertyChanged?.Invoke();
				}
			}
		}

		public ContentRef<Pixmap> Icon
		{
			get => target.Icon;
			set
			{
				if (target.Icon != value)
				{
					target.Icon = value;
					OnPropertyChanged?.Invoke();
				}
			}
		}

		public bool ShowPropertyLabel
		{
			get
			{
				return target.ShowPropertyLabel;
			}
			set
			{
				if (target.ShowPropertyLabel != value)
				{
					target.ShowPropertyLabel = value;
					OnPropertyChanged?.Invoke();
				}
			}
		}

		public bool ShowHeaderLabel
		{
			get
			{
				return target.ShowHeaderLabel;
			}
			set
			{
				if (target.ShowHeaderLabel != value)
				{
					target.ShowHeaderLabel = value;
					OnPropertyChanged?.Invoke();
				}
			}
		}

		public HeaderChanger(HeaderSettings Target, Action propChanged)
		{
			target = Target;
			OnPropertyChanged = propChanged;
		}
	}
}
