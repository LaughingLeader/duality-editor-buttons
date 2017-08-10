using ButtonSample.Buttons;
using Duality;
using Duality.Drawing;
using Duality.Editor;
using Duality.Resources;
using EditorButtons.Editor;
using EditorButtons.Editor.Backgrounds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ButtonSample
{
	[EditorHintCategory("Buttons")]
	public class ButtonComponent : Component, ICmpInitializable
	{
		[DontSerialize] private EditorButtonContainer singleButtonContainer;
		[DontSerialize] private EditorButtonContainer multiRowButtonContainer;
		[DontSerialize] private EditorButtonRow row4;
		[DontSerialize] private HeaderChanger singleHeaderChanger;
		[DontSerialize] private HeaderChanger multiHeaderChanger;

		private List<int> testList;
		private EditorButton addGameObjectButton;
		private GameObject target;
		private ContentRef<Pixmap> bgImage;

		public ContentRef<Pixmap> BackgroundImage
		{
			get => bgImage;
			set
			{
				if (bgImage != value)
				{
					bgImage = value;

					if (bgImage != null)
					{
						row4.Background = new TextureBackground(bgImage, BgWrapMode.Tile);
					}
					else
					{
						row4.Background = null;
					}

					multiRowButtonContainer.Dirty = true;
				}
			}
		}

		public EditorButtonContainer SingleButtonContainer => singleButtonContainer;
		public EditorButtonContainer MultiRowButtonContainer => multiRowButtonContainer;

		public HeaderChanger HeaderChangerSingle
		{
			get => singleHeaderChanger;
			set
			{
				if (value != null) singleHeaderChanger = value;
			}
		}
		public HeaderChanger HeaderChangerMulti
		{
			get => multiHeaderChanger;
			set
			{
				if (value != null) multiHeaderChanger = value;
			}
		}

		public List<int> TestList { get => testList; set => testList = value; }

		public GameObject Target { get => target; set => target = value; }

		public int IndentMulti
		{
			get => multiRowButtonContainer.Indent;
			set
			{
				if (multiRowButtonContainer.Indent != value)
				{
					multiRowButtonContainer.Indent = value;
					multiRowButtonContainer.Dirty = true;
				}
			}
		}

		[EditorHintFlags(MemberFlags.AffectsOthers)]
		public bool Collapsible
		{
			get
			{
				if (multiRowButtonContainer != null) return multiRowButtonContainer.Collapsible;
				return false;
			}
			set
			{
				if (multiRowButtonContainer != null)
				{
					if (multiRowButtonContainer.Collapsible != value)
					{
						multiRowButtonContainer.Dirty = true;
						multiRowButtonContainer.Collapsible = value;
					}
				}
			}
		}

		public void ButtonHit()
		{
			Log.Editor.WriteWarning($"Button was hit!");
		}

		public void AddToList()
		{
			if (TestList == null) testList = new List<int>();
			TestList.Add(1);
		}

		public void ResetList()
		{
			if (TestList != null)
			{
				TestList.Clear();
			}
		}

		public void AddGameObjectExternal()
		{
			if (Target != null)
			{
				if (addGameObjectButton.AdditionalUpdates.Count == 0) addGameObjectButton.AdditionalUpdates.Add(Target);

				var objName = "NewObj" + Target.Children.Count();
				var newObj = new GameObject(objName, Target);
			}
		}

		private void OnSinglePropertyChanged()
		{
			singleButtonContainer.Dirty = true;
		}

		private void OnMultiPropertyChanged()
		{
			multiRowButtonContainer.Dirty = true;
		}

		public void OnInit(Component.InitContext context)
		{
			if (context == InitContext.Activate)
			{
				if (testList == null) testList = new List<int>();

				if (singleButtonContainer == null)
				{
					singleButtonContainer = new EditorButtonContainer("Single Row", ButtonRowAlign.Center);
					singleButtonContainer.ShowPropertyName = false;
					singleButtonContainer.Rows.First().Background = new SolidBackground(ColorRgba.Green);

					addGameObjectButton = new EditorButton("Add GameObject", AddGameObjectExternal, 0.25f);
					var testButton = new EditorButton("Test Button 2", ButtonHit, 0.60f);

					singleButtonContainer.Rows.First().Buttons.Add(addGameObjectButton);
					singleButtonContainer.Rows.First().Buttons.Add(testButton);

					singleHeaderChanger = new HeaderChanger(singleButtonContainer.HeaderSettings, OnSinglePropertyChanged);
				}

				if (multiRowButtonContainer == null)
				{
					multiRowButtonContainer = new EditorButtonContainer("Sample Buttons", ButtonRowAlign.Center);
					multiRowButtonContainer.ShowPropertyName = false;
					multiRowButtonContainer.Collapsible = false;

					var addButton = new EditorButton("Add", AddToList, 0.25f);
					var resetButton = new EditorButton("Reset List", ResetList, 0.25f);

					resetButton.Colors = new ButtonColors();
					//resetTransformButton.Colors.ColorText = ColorRgba.Red.WithRed(200);
					resetButton.Colors.ColorText = ColorRgba.White;
					resetButton.Colors.ColorInner = new ColorRgba(61, 189, 68); // Green
					resetButton.Colors.ColorHighlight = new ColorRgba(30, 220, 39); // Highlighted Green

					multiRowButtonContainer.Rows.First().Buttons.Add(addButton);
					multiRowButtonContainer.Rows.First().Buttons.Add(resetButton);
					multiRowButtonContainer.Rows.First().Background = new LinearBackground(
						ColorRgba.White.WithAlpha(180), ColorRgba.Black.WithAlpha(140), BgGradientMode.Vertical, new Point2(0, 10), new Point2(200, 100));

					var testButton1 = new EditorButton("Test Button 3", ButtonHit, 0.25f);
					var testButton2 = new EditorButton("Test Button 4", ButtonHit, 0.6f);
					testButton2.Colors = new ButtonColors();
					testButton2.Colors.ColorText = ColorRgba.White;
					testButton2.Colors.ColorInner = ColorRgba.DarkGrey;
					testButton2.Colors.ColorHighlight = ColorRgba.VeryLightGrey;

					var testButton3 = new EditorButton("Test Button 5", ButtonHit, 0.30f);
					testButton3.Colors = new ButtonColors();
					testButton3.Colors.ColorText = ColorRgba.White;
					testButton3.Colors.ColorInner = ColorRgba.Blue.WithRed(100);
					testButton3.Colors.ColorHighlight = testButton3.Colors.ColorInner.WithRed(255);

					var row2 = new EditorButtonRow(ButtonRowAlign.Right);
					row2.Background = new HatchBackground(BgHatchStyle.Sphere, ColorRgba.Blue);
					row2.Add(testButton1);
					row2.Add(testButton3);

					var row3 = new EditorButtonRow(ButtonRowAlign.Left);
					row3.Background = new HatchBackground(BgHatchStyle.SmallCheckerBoard, ColorRgba.Red, ColorRgba.Red.WithAlpha(100));
					row3.Add(testButton2);

					row4 = new EditorButtonRow(ButtonRowAlign.Center);
					if (bgImage != null)
					{
						row4.Background = new TextureBackground(bgImage, BgWrapMode.Tile);
					}

					int btnNum = 5;
					float buttonWidth = 0.80f / btnNum;
					Random ran = new Random();
					for (int i = 0; i < btnNum; i++)
					{
						var btn = new EditorButton("Button" + i, ButtonHit, buttonWidth);
						btn.Colors = new ButtonColors();
						btn.Colors.ColorInner = ran.NextColorRgba().WithAlpha(200);
						btn.Colors.ColorHighlight = testButton3.Colors.ColorInner.WithAlpha(255);
						row4.Add(btn);
					}

					//Add an empty row for spacing between the next property down.
					var emptyRow = new EditorButtonRow();
					emptyRow.Background = new SolidBackground(ColorRgba.LightGrey);
					//emptyRow.HeightPercentage = 0.5f;

					multiRowButtonContainer.Rows.Add(row2);
					multiRowButtonContainer.Rows.Add(row3);
					multiRowButtonContainer.Rows.Add(row4);
					multiRowButtonContainer.Rows.Add(emptyRow);

					multiHeaderChanger = new HeaderChanger(multiRowButtonContainer.HeaderSettings, OnMultiPropertyChanged);
				}
			}
		}

		public void OnShutdown(Component.ShutdownContext context)
		{
		}
	}
}