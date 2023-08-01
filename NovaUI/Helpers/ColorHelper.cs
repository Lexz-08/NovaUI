using System;
using System.Drawing;

namespace NovaUI.Helpers
{
	public static class ColorHelper
	{
		/// <summary>
		/// Modifies the brightness of a given color by a given factor.
		/// </summary>
		/// <param name="color">The color to modify the brightness of.</param>
		/// <param name="factor">The factor by which brightness will be modified. (+/- factors lighten or darknen respectively)</param>
		/// <returns>The given color of modified brightness.</returns>
		public static Color ChangeBrightness(this Color color, float factor)
		{
			float red = color.R;
			float green = color.G;
			float blue = color.B;

			if (factor < 0)
			{
				factor += 1;
				red *= factor;
				green *= factor;
				blue *= factor;
			}
			else
			{
				red = (255 - red) * factor + red;
				green = (255 - green) * factor + green;
				blue = (255 - blue) * factor + blue;
			}

			return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
		}

		/// <summary>
		/// Makes a given color lighter by a given factor.
		/// </summary>
		/// <param name="color">The color to make lighter.</param>
		/// <param name="factor">The factor by which the color will be made lighter.</param>
		/// <returns>A lighter color.</returns>
		public static Color Lighter(this Color color, float factor) => color.ChangeBrightness(Math.Abs(factor));

		/// <summary>
		/// Makes a given color darker by a given factor.
		/// </summary>
		/// <param name="color">The color to make darker.</param>
		/// <param name="factor">The factor by which the color will be made darker.</param>
		/// <returns>A darker color.</returns>
		public static Color Darker(this Color color, float factor) => color.ChangeBrightness(-Math.Abs(factor));

		/// <summary>
		/// Blends a given color with another color halfway.
		/// </summary>
		/// <param name="color1">The first color to half-blend.</param>
		/// <param name="color2">The second color to half-blend.</param>
		/// <returns>Two colors blended halfway into one.</returns>
		public static Color BlendWith(this Color color1, Color color2)
		{
			return Color.FromArgb(
				color1.A == 255 && color2.A == 255 ? 255 : (byte)(color1.A * 0.5) + (byte)(color2.A * 0.5),
				(byte)(color1.R * 0.5) + (byte)(color2.R * 0.5),
				(byte)(color1.G * 0.5) + (byte)(color2.G * 0.5),
				(byte)(color1.B * 0.5) + (byte)(color2.B * 0.5)
				);
		}

		/// <summary>
		/// Converts a given color to a solid brush.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		/// <returns>The convert solid brush.</returns>
		internal static SolidBrush ToBrush(this Color color) => new SolidBrush(color);
	}
}
