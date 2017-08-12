using Duality;
using Duality.Drawing;
using Duality.Editor;
using EditorButtons.Buttons;
using EditorButtons.Buttons.Backgrounds;
using EditorButtons.Editor;

namespace ButtonSample
{
	[EditorHintCategory("Buttons")]
	public class SimpleButtonComponent : Component, ICmpInitializable
	{
		[DontSerialize] private EditorButtonContainer buttonContainer;

		public EditorButtonContainer Z_ButtonContainer
		{
			get => buttonContainer;
			set => buttonContainer = value;
		}

		public void ButtonHit()
		{
			Log.Editor.WriteWarning($"Button was hit!");
		}

		public void OnInit(Component.InitContext context)
		{
			if (context == InitContext.Activate)
			{
				if (buttonContainer == null)
				{
					var testButton1 = new EditorButton("Test Button 1", ButtonHit, 0.4f);
					var testButton2 = new EditorButton("Test Button 2", ButtonHit, 0.5f);

					var singleRow = new EditorButtonRow(0.75f, testButton1, testButton2);
					singleRow.Background = new SolidBackground(ColorRgba.Green);
					singleRow.ScaleMode = ButtonRowScaleMode.Manual;

					buttonContainer = new EditorButtonContainer("Buttons", ButtonRowAlign.Center, singleRow);
					
				}
			}
		}

		public void OnShutdown(Component.ShutdownContext context)
		{
		}
	}
}