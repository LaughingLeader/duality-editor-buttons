﻿namespace EditorButtons.Editor.Backgrounds
{
	public interface IButtonBackground
	{
		BackgroundStyle BackgroundStyle { get; }

		bool Outline { get; set; }
	}
}