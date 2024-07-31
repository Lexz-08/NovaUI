using System;
using System.IO;

namespace NovaUI.Events
{
	/// <summary>
	/// Provides data for the <see cref="Controls.NovaFileInput.FileSelected"/> event.
	/// </summary>
	public class FileSelectedEventArgs : EventArgs
	{
		private string _fullPath;
		private string _fileName;
		private string _fileExt;

		/// <summary>
		/// The full, valid path of the selected file.
		/// </summary>
		public string FullPath => _fullPath;

		/// <summary>
		/// The file name of the selected file, including its extension.
		/// </summary>
		public string FileName => _fileName;

		/// <summary>
		/// The extension of the selected file, including the preceding period.
		/// </summary>
		public string FileExtension => _fileExt;

		/// <summary>
		/// Creates a new <see cref="FileSelectedEventArgs"/>.
		/// </summary>
		/// <param name="FileName">The currently selected file.</param>
		public FileSelectedEventArgs(string FileName)
		{
			_fullPath = FileName;
			_fileName = Path.GetFileName(FileName);
			_fileExt = Path.GetExtension(FileName);
		}
	}
}
