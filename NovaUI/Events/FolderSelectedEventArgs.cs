using System;
using System.IO;

namespace NovaUI.Events
{
	/// <summary>
	/// Provides data for the <see cref="Controls.NovaFolderInput.FolderSelected"/> event.
	/// </summary>
	public class FolderSelectedEventArgs : EventArgs
	{
		private string _fullPath;
		private string _parentFolder;
		private string _folderName;

		/// <summary>
		/// The full, valid path of the selected folder.
		/// </summary>
		public string FullPath => _fullPath;

		/// <summary>
		/// The folder name of the selected folder's parent folder.
		/// </summary>
		public string ParentFolder => _parentFolder;

		/// <summary>
		/// The folder name of the selected folder.
		/// </summary>
		public string FolderName => _folderName;

		/// <summary>
		/// Creates a new <see cref="FolderSelectedEventArgs"/>.
		/// </summary>
		/// <param name="FolderPath">The currently selected folder.</param>
		public FolderSelectedEventArgs(string FolderPath)
		{
			_fullPath = FolderPath;
			_parentFolder = Directory.GetParent(FolderPath).Name;
			_folderName = new DirectoryInfo(FolderPath).Name;
		}
	}
}
