using Duality.Editor;
using Duality.Editor.Forms;
using EditorButtons.Editor.Buttons;

namespace EditorButtons.Editor
{
	/// <summary>
	/// Defines a Duality editor plugin.
	/// </summary>
	public class EditorButtonsEditorPlugin : EditorPlugin
	{
		public override string Id
		{
			get { return "EditorButtonsEditorPlugin"; }
		}

		protected override void InitPlugin(MainForm main)
		{
			ButtonPropertyMethods.InitColors();
		}
	}
}