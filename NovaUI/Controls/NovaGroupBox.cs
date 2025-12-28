using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(GroupBox))]
	[DefaultEvent("Click")]
	public class NovaGroupBox : GroupBox
	{
		private readonly Pen borderPen = Color.Transparent.ToPen();
		private readonly Pen backPen = Color.Transparent.ToPen();
		private readonly SolidBrush backBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush textBrush = Color.Transparent.ToBrush();

		private Color borderColor = Constants.BorderColor;
		private int borderWidth = 1;
		private int borderRadius = 6;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderWidth"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderWidth property changes.")]
		public event EventHandler BorderWidthChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderRadius"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderRadius property changes.")]
		public event EventHandler BorderRadiusChanged;

		protected virtual void OnBorderColorChanged(EventArgs e)
		{
			BorderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnBorderWidthChanged(EventArgs e)
		{
			BorderWidthChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnBorderRadiusChanged(EventArgs e)
		{
			BorderRadiusChanged?.Invoke(this, e);
			Invalidate();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the border color of the control.")]
		public Color BorderColor
		{
			get => borderColor;
			set
			{
				borderColor = value;
				if (borderPen.Color != value) borderPen.Color = value;
				OnBorderColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the border width of the control.")]
		public int BorderWidth
		{
			get => borderWidth;
			set
			{
				borderWidth = value;
				if (borderPen.Width != value) borderPen.Width = value;
				OnBorderWidthChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the border radius of the control.")]
		public int BorderRadius
		{
			get => borderRadius;
			set
			{
				if (value != borderRadius)
					Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, value, value));
				borderRadius = value;
				OnBorderRadiusChanged(EventArgs.Empty);
			}
		}

		public NovaGroupBox()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			borderPen = new Pen(borderColor, borderWidth);
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;
			Size = new Size(250, 200);
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
			Invalidate();
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);
			if (backBrush.Color != BackColor)
			{
				backBrush.Color = BackColor;
				backPen.Color = BackColor;
			}
			Invalidate();
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			base.OnForeColorChanged(e);
			if (textBrush.Color != ForeColor) textBrush.Color = ForeColor;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.Clear(Parent != null ? Parent.BackColor : Color.Transparent);

			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			if (borderRadius > 0)
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (borderWidth > 0)
				{
					e.Graphics.FillPath(backBrush,
						new Rectangle(borderWidth - 1, Font.Height + borderWidth + 7, Width - (borderWidth * 2) + 1, Height - Font.Height - (borderWidth * 2) - 7).Round(Math.Max(1, borderRadius - borderWidth)));
					e.Graphics.DrawPath(borderPen,
						new RectangleF(borderWidth / 2f - 0.5f, Font.Height + borderWidth / 2f + 8 - 0.5f, Width - borderWidth, Height - Font.Height - borderWidth - 8).Round(borderRadius));
				}
				else
				{
					e.Graphics.FillPath(backBrush,
						new Rectangle(0, Font.Height + 8, Width - 1, Height - Font.Height - 9).Round(borderRadius));
					e.Graphics.DrawPath(backPen,
						new Rectangle(0, Font.Height + 8, Width - 1, Height - Font.Height - 9).Round(borderRadius));
				}
			}
			else
			{
				if (borderWidth > 0)
				{
					e.Graphics.FillRectangle(backBrush,
						new Rectangle(borderWidth, Font.Height + borderWidth + 8, Width - (borderWidth * 2), Height - Font.Height - (borderWidth * 2) - 8));
					e.Graphics.DrawRectangle(borderPen,
						borderWidth / 2f, borderWidth / 2f + Font.Height + 8, Width - borderWidth, Height - Font.Height - borderWidth - 8);
				}
				else e.Graphics.FillRectangle(backBrush,
					new Rectangle(0, Font.Height + 8, Width, Height - Font.Height - 8));
			}

			e.Graphics.DrawString(Text, Font, textBrush,
				new PointF(3, 2), new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near });
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				borderPen?.Dispose();
				backPen?.Dispose();
				backBrush?.Dispose();
				textBrush?.Dispose();
			}
		}
	}
}
