using System.Collections.Generic;
using System.Drawing;

namespace NovaUI.Helpers.LibMain
{
	public static class Constants
	{
		public static Color PrimaryColor => Color.FromArgb(25, 25, 25);
		public static Color SecondaryColor => Color.FromArgb(50, 50, 50);
		public static Color BorderColor => Color.FromArgb(75, 75, 75);
		public static Color TextColor => Color.FromArgb(235, 235, 235);
		public static Color AccentColor => Color.Gold.BlendWith(Color.Magenta);

		public static StringFormat CenterAlign => new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
		public static StringFormat LeftAlign => new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
		public static StringFormat RightAlign => new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

		public static Dictionary<bool, Dictionary<int, string>> Hexadecimals = new Dictionary<bool, Dictionary<int, string>>
		{
			{ false, new Dictionary<int, string>
			{
				{ 0, "0" },
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
				{ 6, "6" },
				{ 7, "7" },
				{ 8, "8" },
				{ 9, "9" },
				{ 10, "a" },
				{ 11, "b" },
				{ 12, "c" },
				{ 13, "d" },
				{ 14, "e" },
				{ 15, "f" }
			} },
			{ true, new Dictionary<int, string>
			{
				{ 0, "0" },
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
				{ 6, "6" },
				{ 7, "7" },
				{ 8, "8" },
				{ 9, "9" },
				{ 10, "A" },
				{ 11, "B" },
				{ 12, "C" },
				{ 13, "D" },
				{ 14, "E" },
				{ 15, "F" }
			} }
		};
	}
}
