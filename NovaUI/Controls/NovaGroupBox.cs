using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(GroupBox))]
	public class NovaGroupBox : GroupBox
	{
		private Color _borderColor = Constants.BorderColor;
		private int _borderWidth = 1;
		private int _borderRadius = 6;
		private bool _boundedTitle = false;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderWidth"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderWidth property changes.")]
		public event EventHandler BorderWidthChanged;

		/// <summary>
		/// Occurs when the vlaue of the <see cref="BorderRadius"/> propert changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderRadius property changes.")]
		public event EventHandler BorderRadiusChanged;

		/// <summary>
		/// Raises the <see cref="BorderColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnBorderColorChanged(EventArgs e)
		{
			BorderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="BorderWidthChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnBorderWidthChanged(EventArgs e)
		{
			BorderWidthChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="BorderRadiusChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnBorderRadiusChanged(EventArgs e)
		{
			BorderRadiusChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Gets or sets the border color of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border color of the control.")]
		public Color BorderColor
		{
			get => _borderColor;
			set { _borderColor = value; OnBorderColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the border width of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border width of the control.")]
		public int BorderWidth
		{
			get => _borderWidth;
			set { _borderWidth = value; OnBorderWidthChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the border radius of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border radius of the control.")]
		public int BorderRadius
		{
			get => _borderRadius;
			set { _borderRadius = value; OnBorderRadiusChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the line under the title completes the border.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets a value indicating whether the line under the title completes the border.")]
		public bool BoundedTitle
		{
			get => _boundedTitle;
			set { _boundedTitle = value; Invalidate(); }
		}

		public NovaGroupBox()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;
			Size = new Size(250, 200);
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.Clear(Parent.BackColor);
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, _borderRadius, _borderRadius));

			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			if (_borderRadius > 0)
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

				if (_borderWidth > 0) e.Graphics.FillPath(_borderColor.ToBrush(), new Rectangle(0, 0, Width - 1, Height - 1).Roundify(_borderRadius));
				e.Graphics.FillPath(BackColor.ToBrush(),
					new Rectangle(_borderWidth + 1, _borderWidth + 1, Width - (_borderWidth * 2) - 3, Height - (_borderWidth * 2) - 3).Roundify(_borderRadius > 1 ? _borderRadius - 1 : _borderRadius));
			}
			else
			{
				e.Graphics.FillRectangle(_borderColor.ToBrush(), 0, 0, Width, Height);
				e.Graphics.FillRectangle(BackColor.ToBrush(),
					_borderWidth, _borderWidth, Width - (_borderWidth * 2), Height - (_borderWidth * 2));
			}

			if (_boundedTitle)
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
				e.Graphics.DrawLine(new Pen(_borderColor, _borderWidth * 2),
					new PointF(_borderWidth / 2, (_borderWidth * 2) + 9 + Font.Height),
					new PointF(Width - _borderWidth / 2, (_borderWidth * 2) + 9 + Font.Height));
			}
			else
			{
				e.Graphics.FillPath(_borderColor.ToBrush(),
					new Rectangle(_borderWidth + 4, _borderWidth + 9 + Font.Height, Width - ((_borderWidth + 4) * 2), _borderWidth * 2).Roundify(_borderWidth));
			}

			e.Graphics.DrawString(Text, Font, ForeColor.ToBrush(),
				new PointF(_borderWidth + 4, _borderWidth + 4),
				new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near });
		}
	}
}
