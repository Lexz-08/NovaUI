namespace NovaUI.Events
{
	/// <summary>
	/// Represents the method that will handle the <see langword="FileSelected"/> event of a <see cref="Controls.NovaFileInput"/>.
	/// </summary>
	/// <param name="sender">The file input that raised the event.</param>
	/// <param name="e">The <see cref="FileSelectedEventArgs"/> containing information about the selected file.</param>
	public delegate void FileSelectedEventHandler(object sender, FileSelectedEventArgs e);
}
