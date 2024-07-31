using NovaUI.Controls;

namespace NovaUI.Events
{
	/// <summary>
	/// Represents the method that will handle the <see langword="WindowStateChanged"/> event of a <see cref="NovaWindow"/> or <see cref="NovaStrippedWindow"/>.
	/// </summary>
	/// <param name="sender">The window that raised the event.</param>
	/// <param name="e">The <see cref="WindowStateChangedEventArgs"/> containing information about the window states.</param>
	public delegate void WindowStateChangedEventHandler(object sender, WindowStateChangedEventArgs e);
}
