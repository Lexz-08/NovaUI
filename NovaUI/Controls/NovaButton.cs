using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(Button))]
	[DefaultEvent("Click")]
	public class NovaButton : Button
	{
		private readonly Pen borderNormalPen = Color.Transparent.ToPen();
		private readonly Pen borderDownPen = Color.Transparent.ToPen();
		private readonly SolidBrush backNormalBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush backHoverBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush backDownBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush noBorderNormalBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush noBorderHoverBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush noBorderDownBrush = Color.Transparent.ToBrush();
		private readonly Pen noBorderNormalPen = Color.Transparent.ToPen();
		private readonly Pen noBorderHoverPen = Color.Transparent.ToPen();
		private readonly Pen noBorderDownPen = Color.Transparent.ToPen();
		private readonly SolidBrush textBrush = Color.Transparent.ToBrush();

		private Color borderColor = Constants.BorderColor;
		private Color activeColor = Constants.AccentColor;
		private int borderWidth = 1;
		private int borderRadius = 6;
		private bool useUserSchemeCursor = true;
		private Cursor originalCursor = Cursors.Hand;

		private bool mouseHover = false;
		private bool mouseDown = false;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"/> property changes.
		/// </summary>
		[Category("Appearance"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="ActiveColor"/> property changes.
		/// </summary>
		[Category("Appearance"), Description("Occurs when the value of the ActiveColor property changes.")]
		public event EventHandler ActiveColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderWidth"/> property changes.
		/// </summary>
		[Category("Appearance"), Description("Occurs when the value of the BorderWidth property changes.")]
		public event EventHandler BorderWidthChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderRadius"/> property changes.
		/// </summary>
		[Category("Appearance"), Description("Occurs when the value of the BorderRadius property changes.")]
		public event EventHandler BorderRadiusChanged;

		protected virtual void OnBorderColorChanged(EventArgs e)
		{
			BorderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnActiveColorChanged(EventArgs e)
		{
			ActiveColorChanged?.Invoke(this, e);
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

		/// <summary>
		/// Gets or sets the border color of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border color of the control.")]
		public Color BorderColor
		{
			get => borderColor;
			set
			{
				borderColor = value;
				if (borderNormalPen.Color != value) borderNormalPen.Color = value;
				OnBorderColorChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the border color of the control when selected.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border color of the control when selected.")]
		public Color ActiveColor
		{
			get => activeColor;
			set
			{
				activeColor = value;
				if (borderDownPen.Color != value)
				{
					borderDownPen.Color = value;
					noBorderDownBrush.Color = value;
					noBorderDownPen.Color = value;
				}
				OnActiveColorChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the border width of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border width of the control.")]
		public int BorderWidth
		{
			get => borderWidth;
			set
			{
				if (value < 0) value = 0;
				borderWidth = value;
				if (borderNormalPen.Width != value)
				{
					borderNormalPen.Width = value;
					borderDownPen.Width = value;
				}
				OnBorderWidthChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the border radius of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border radius of the control.")]
		public int BorderRadius
		{
			get => borderRadius;
			set
			{
				if (value < 0) value = 0;
				else if (value > Math.Min(Width, Height) / 2)
					value = Math.Min(Width, Height) / 2;
				if (value != borderRadius)
					Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, value, value));
				borderRadius = value;
				OnBorderRadiusChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control will use the user-selected system scheme cursor.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the control will use the user-selected system scheme cursor.")]
		public bool UseUserSchemeCursor
		{
			get => useUserSchemeCursor;
			set { useUserSchemeCursor = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the cursor that is displayed when the mouse pointer is over the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the cursor that is displayed when the mouse pointer is over the control.")]
		public override Cursor Cursor
		{
			get => base.Cursor;
			set
			{
				base.Cursor = value;
				if (!useUserSchemeCursor) originalCursor = value;

				OnCursorChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		public NovaButton()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			borderNormalPen = new Pen(borderColor);
			borderDownPen = new Pen(activeColor);
			noBorderDownBrush = activeColor.ToBrush();
			noBorderDownPen = new Pen(activeColor);
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;
			Size = new Size(100, 30);
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

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);

			mouseHover = true;
			if (useUserSchemeCursor) Win32.GetRegistryCursor(Win32.RegistryCursor.Hand, this);
			else Cursor = originalCursor;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			mouseHover = false;
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			mouseDown = true;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			mouseDown = false;
			Invalidate();
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
			Invalidate();
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);
			if (backNormalBrush.Color != BackColor)
			{
				backNormalBrush.Color = BackColor;
				backHoverBrush.Color = BackColor.Lighter(0.1f);
				backDownBrush.Color = BackColor.Lighter(0.1f).Darker(0.1f);
				noBorderNormalBrush.Color = BackColor;
				noBorderHoverBrush.Color = BackColor.Lighter(0.1f);
				noBorderDownBrush.Color = BackColor.Lighter(0.1f).Darker(0.1f);
				noBorderNormalPen.Color = BackColor;
				noBorderHoverPen.Color = BackColor.Lighter(0.1f);
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
					e.Graphics.FillPath(mouseHover ? (mouseDown ? backDownBrush : backHoverBrush) : backNormalBrush,
						new Rectangle(borderWidth - 1, borderWidth - 1, Width - (borderWidth * 2) + 1, Height - (borderWidth * 2) + 1)
						.Round(Math.Max(1, borderRadius - borderWidth)));
					e.Graphics.DrawPath(mouseDown ? borderDownPen : borderNormalPen,
						new RectangleF(borderWidth / 2f - 0.5f, borderWidth / 2f - 0.5f, Width - borderWidth, Height - borderWidth)
						.Round(borderRadius));
				}
				else
				{
					e.Graphics.FillPath(mouseHover ? (mouseDown ? noBorderDownBrush : noBorderHoverBrush) : noBorderNormalBrush,
						new Rectangle(0, 0, Width - 1, Height - 2).Round(borderRadius));
					e.Graphics.DrawPath(mouseHover ? (mouseDown ? noBorderDownPen : noBorderHoverPen) : noBorderNormalPen,
						new Rectangle(0, 0, Width - 1, Height - 2).Round(borderRadius));
				}
			}
			else
			{
				if (borderWidth > 0)
				{
					e.Graphics.FillRectangle(mouseHover ? (mouseDown ? backDownBrush : backHoverBrush) : backNormalBrush,
						new Rectangle(borderWidth, borderWidth, Width - (borderWidth * 2), Height - (borderWidth * 2)));
					e.Graphics.DrawRectangle(mouseDown ? borderDownPen : borderNormalPen,
						borderWidth / 2f, borderWidth / 2f, Width - borderWidth, Height - borderWidth);
				}
				else e.Graphics.FillRectangle(mouseHover ? (mouseDown ? noBorderDownBrush : noBorderHoverBrush) : noBorderNormalBrush,
					new Rectangle(0, 0, Width, Height));
			}

			e.Graphics.DrawString(Text, Font, textBrush,
				new Rectangle(0, 0, Width, Height), Constants.CenterAlign);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				borderNormalPen?.Dispose();
				borderDownPen?.Dispose();
				backNormalBrush?.Dispose();
				backHoverBrush?.Dispose();
				backDownBrush?.Dispose();
				noBorderNormalBrush?.Dispose();
				noBorderHoverBrush?.Dispose();
				noBorderDownBrush?.Dispose();
				noBorderNormalPen?.Dispose();
				noBorderHoverPen?.Dispose();
				noBorderDownPen?.Dispose();
			}
		}
	}
}
