using NovaUI.Helpers.LibMain;
using System.Drawing;
using System.Windows.Forms;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(Label))]
	public class NovaLabel : Label
	{
		public NovaLabel()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;
		}
	}
}
