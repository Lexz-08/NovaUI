using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(PictureBox))]
	[DefaultEvent("Click")]
	public class NovaPictureBox : PictureBox
	{
		public NovaPictureBox()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			BackColor = Constants.PrimaryColor;
		}
	}
}
