using System.Drawing;
using System.Drawing.Drawing2D;

namespace NovaUI.Helpers
{
	internal static class GraphicsHelper
	{
		/// <summary>
		/// Rounds all corners of a graphics path created from the rectangular bounds specified.
		/// </summary>
		/// <param name="rect">The rectangular bounds of the rounded graphics path.</param>
		/// <param name="radius">The corner radius of the graphics path.</param>
		public static GraphicsPath Roundify(this Rectangle rect, int radius)
		{
			//GraphicsPath path = new GraphicsPath();
			//radius *= 2;

			//path.StartFigure();
			//path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
			//path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
			//path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
			//path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
			//path.CloseFigure();

			//return path;
			return rect.RoundifyCorners(radius);
		}

		/// <summary>
		/// If specified for any corner, rounds the corners specified of a graphics path created from the rectangular bounds specified.
		/// </summary>
		/// <param name="rect">The rectangular bounds of the graphics path, rounded if any corners are specified.</param>
		/// <param name="radius">The corner radius of the graphics path.</param>
		/// <param name="topLeft">Indicates whether the top-left corner of the graphics path should be rounded.</param>
		/// <param name="topRight">Indicates whether the top-right corner of the graphics path should be rounded.</param>
		/// <param name="bottomLeft">Indicates whether the bottom-left corner of the graphics path should be rounded.</param>
		/// <param name="bottomRight">Indicates whether the bottom-right corner of the graphics path should be rounded.</param>
		public static GraphicsPath RoundifyCorners(this Rectangle rect, int radius, bool topLeft = true, bool topRight = true, bool bottomLeft = true, bool bottomRight = true)
		{
			GraphicsPath path = new GraphicsPath();
			radius *= 2;

			path.StartFigure();

			if (topLeft) path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
			else
			{
				path.AddLine(rect.X, rect.Y - radius, rect.X, rect.Y);
				path.AddLine(rect.X, rect.Y, rect.X + radius, rect.Y);
			}

			if (topRight) path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
			else
			{
				path.AddLine(rect.X + rect.Width - radius, rect.Y, rect.X + rect.Width, rect.Y);
				path.AddLine(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + radius);
			}

			if (bottomRight) path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
			else
			{
				path.AddLine(rect.X + rect.Width, rect.Y + rect.Height - radius, rect.X + rect.Width, rect.Y + rect.Height);
				path.AddLine(rect.X + rect.Width, rect.Y + rect.Height, rect.X + rect.Width - radius, rect.Y + rect.Height);
			}

			if (bottomLeft) path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
			else
			{
				path.AddLine(rect.X + radius, rect.Y + rect.Height, rect.X, rect.Y + rect.Height);
				path.AddLine(rect.X, rect.Y + rect.Height, rect.X, rect.Y + rect.Height - radius);
			}

			path.CloseFigure();

			return path;
		}

		/// <summary>
		/// Rescales the location and size of a specified rectangular boundary.
		/// </summary>
		/// <param name="rect">The rectangular boundary to be rescaled.</param>
		/// <param name="posOffset">The offset scaling for the boundary location.</param>
		/// <param name="sizeOffset">The offset scaling for the boundary size.</param>
		public static Rectangle Rescale(this Rectangle rect, int posOffset, int sizeOffset)
		{
			return new Rectangle(rect.X + posOffset, rect.Y + posOffset, rect.Width + sizeOffset, rect.Height + sizeOffset);
		}

		/// <summary>
		/// Converts an image to a tiled texture brush.
		/// </summary>
		/// <param name="image">The image to convert.</param>
		public static TextureBrush ToTiledBrush(this Image image) => new TextureBrush(image, WrapMode.Tile);

		/// <summary>
		/// Converts an image to a clamped texture brush.
		/// </summary>
		/// <param name="image">The image to convert.</param>
		public static TextureBrush ToClampedBrush(this Image image) => new TextureBrush(image, WrapMode.Clamp);
	}
}
