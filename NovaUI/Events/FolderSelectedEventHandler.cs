namespace NovaUI.Events
{
	/// <summary>
	/// Represents the method that will handle the <see langword="FolderSelected"/> event of a <see cref="Controls.NovaFolderInput"/>.
	/// </summary>
	/// <param name="sender">The folder input that raised the event.</param>
	/// <param name="e">The <see cref="FolderSelectedEventArgs"/> containing information about the selected folder.</param>
	public delegate void FolderSelectedEventHandler(object sender, FolderSelectedEventArgs e);
}
