using NovaUI.Controls;

namespace NovaUI.Enums
{
	/// <summary>
	/// Specifies where the value of a progress bar should be displayed on a <see cref="NovaProgressBar"/>, if <see cref="NovaProgressBar.ShowValue"/> is set to <see langword="true"/>.
	/// </summary>
	public enum ProgressValueAlignment
	{
		/// <summary>
		/// The progress value should be displayed on the left side of the progress bar.
		/// </summary>
		Left,

		/// <summary>
		/// The progress value should be displayed on the right of the progress bar.
		/// </summary>
		Right,

		/// <summary>
		/// The progress value should be displayed in the center of the progress bar.
		/// </summary>
		Center,

		/// <summary>
		/// The progress value should be displayed in the center of the progress indicator.
		/// </summary>
		ProgressIndicator
	}
}
