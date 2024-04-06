using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(Label))]
	[DefaultEvent("Click")]
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
