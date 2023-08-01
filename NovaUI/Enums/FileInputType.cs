using NovaUI.Controls;
using System.Windows.Forms;

namespace NovaUI.Enums
{
	/// <summary>
	/// Specifies the type of file dialog to display when clicking on a <see cref="NovaFileInput"/>.
	/// </summary>
	public enum FileInputType
	{
		/// <summary>
		/// Display an <see cref="OpenFileDialog"/>.
		/// </summary>
		OpenFile,

		/// <summary>
		/// Display a <see cref="SaveFileDialog"/>.
		/// </summary>
		SaveFile
	}
}
