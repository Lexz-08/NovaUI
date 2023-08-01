using NovaUI.Helpers.LibMain;
using System.Drawing;
using System.Windows.Forms;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(PictureBox))]
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
