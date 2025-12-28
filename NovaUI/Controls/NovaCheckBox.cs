using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(CheckBox))]
	[DefaultEvent("CheckedChanged")]
	public class NovaCheckBox : CheckBox
	{
		private readonly SolidBrush checkedBackNormalBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush checkedBackHoverBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush checkedBackDownBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush uncheckedBackNormalBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush uncheckedBackHoverBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush uncheckedBackDownBrush = Color.Transparent.ToBrush();
		private readonly Pen checkedBorderNormalPen = Color.Transparent.ToPen();
		private readonly Pen checkedBorderHoverPen = Color.Transparent.ToPen();
		private readonly Pen checkedBorderDownPen = Color.Transparent.ToPen();
		private readonly Pen uncheckedBorderPen = Color.Transparent.ToPen();
		private readonly SolidBrush textBrush = Color.Transparent.ToBrush();
		private readonly Font checkFont;

		private Color borderColor = Constants.BorderColor;
		private Color checkColor = Constants.AccentColor;
		private int borderWidth = 1;
		private int borderRadius = 2;
		private bool useUserSchemeCursor = true;
		private Cursor originalCursor = Cursors.Hand;

		private int pos;
		private const int size = 16;

		private bool mouseHover = false;
		private bool mouseDown = false;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="CheckColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the CheckColor property changes.")]
		public event EventHandler CheckColorChanged;

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

		protected virtual void OnCheckColorChanged(EventArgs e)
		{
			CheckColorChanged?.Invoke(this, e);
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
		/// Gets or sets the border color of the control check box.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border color of the control check box.")]
		public Color BorderColor
		{
			get => borderColor;
			set
			{
				borderColor = value;
				if (uncheckedBorderPen.Color != value) uncheckedBorderPen.Color = value;
				OnBorderColorChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the background color of the control check box.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the control check box.")]
		public Color CheckColor
		{
			get => checkColor;
			set
			{
				checkColor = value;
				if (checkedBorderNormalPen.Color != value)
				{
					checkedBorderNormalPen.Color = value;
					checkedBorderHoverPen.Color = value.Lighter(0.1f);
					checkedBorderDownPen.Color = value.Lighter(0.1f).Darker(0.1f);
					checkedBackNormalBrush.Color = value;
					checkedBackHoverBrush.Color = value.Lighter(0.1f);
					checkedBackDownBrush.Color = value.Lighter(0.1f).Darker(0.1f);
				}
				OnCheckColorChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the border width of the check box.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets border width of the check box.")]
		public int BorderWidth
		{
			get => borderWidth;
			set
			{
				if (value < 1) value = 1;
				borderWidth = value;
				if (checkedBorderNormalPen.Width != value)
				{
					checkedBorderNormalPen.Width = value;
					checkedBorderHoverPen.Width = value;
					checkedBorderDownPen.Width = value;
					uncheckedBorderPen.Width = value;
				}
				OnBorderWidthChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the border radius of the check box.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border radius of the check box.")]
		public int BorderRadius
		{
			get => borderRadius;
			set
			{
				if (value < 0) value = 0;
				else if (value > Height / 2)
					value = Height / 2;
				borderRadius = value;
				OnBorderRadiusChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the check box will use the user-selected system scheme cursor.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the check box will use the user-selected system scheme cursor.")]
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

		public NovaCheckBox()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			checkedBorderNormalPen = new Pen(checkColor);
			checkedBorderHoverPen = new Pen(checkColor.Lighter(0.1f));
			checkedBorderDownPen = new Pen(checkColor.Lighter(0.1f).Darker(0.1f));
			checkedBackNormalBrush = checkColor.ToBrush();
			checkedBackHoverBrush = checkColor.Lighter(0.1f).ToBrush();
			checkedBackDownBrush = checkColor.Lighter(0.1f).Darker(0.1f).ToBrush();
			uncheckedBorderPen = new Pen(borderColor);
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;

			checkFont = new Font("Segoe MDL2 Assets", size * 0.5f);
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
			if (uncheckedBackNormalBrush.Color != BackColor)
			{
				uncheckedBackNormalBrush.Color = BackColor;
				uncheckedBackHoverBrush.Color = BackColor.Lighter(0.1f);
				uncheckedBackDownBrush.Color = BackColor.Lighter(0.1f).Darker(0.1f);
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
			pos = (Height - size) / 2 + 1;

			if (borderRadius > 0)
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				e.Graphics.FillPath(Checked ? (mouseHover ? (mouseDown ? checkedBackDownBrush : checkedBackHoverBrush) : checkedBackNormalBrush) : (mouseHover ? (mouseDown ? uncheckedBackDownBrush : uncheckedBackHoverBrush) : uncheckedBackNormalBrush),
					new Rectangle(borderWidth - 1, pos + borderWidth - 1, size - (borderWidth * 2) + 1, size - (borderWidth * 2) + 1).Round(Math.Max(1, borderRadius - borderWidth)));
				e.Graphics.DrawPath(Checked ? (mouseHover ? (mouseDown ? checkedBorderDownPen : checkedBorderHoverPen) : checkedBorderNormalPen) : uncheckedBorderPen,
					new RectangleF(borderWidth / 2f - 0.5f, borderWidth / 2f + pos - 0.5f, size - borderWidth, size - borderWidth).Round(borderRadius));
			}
			else
			{
				e.Graphics.FillRectangle(Checked ? (mouseHover ? (mouseDown ? checkedBackDownBrush : checkedBackHoverBrush) : checkedBackNormalBrush) : (mouseHover ? (mouseDown ? uncheckedBackDownBrush : uncheckedBackHoverBrush) : uncheckedBackNormalBrush),
					new Rectangle(borderWidth - 1, pos + borderWidth, size - (borderWidth * 2) + 1, size - (borderWidth * 2)));
				e.Graphics.DrawRectangle(Checked ? (mouseHover ? (mouseDown ? checkedBorderDownPen : checkedBorderHoverPen) : checkedBorderNormalPen) : uncheckedBorderPen,
					borderWidth / 2f - 1, borderWidth / 2f + pos, size - borderWidth + 1, size - borderWidth);
			}

			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			if (Checked) e.Graphics.DrawString("\uE65F", checkFont, textBrush,
				new Rectangle(-1, pos, size, size), Constants.CenterAlign);
			e.Graphics.DrawString(Text, Font, textBrush,
				new Rectangle(size + borderWidth + 1, 0, Width - size - borderWidth - 1, Height), Constants.LeftAlign);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				checkedBackNormalBrush?.Dispose();
				checkedBackHoverBrush?.Dispose();
				checkedBackDownBrush?.Dispose();
				uncheckedBackNormalBrush?.Dispose();
				uncheckedBackHoverBrush?.Dispose();
				uncheckedBackDownBrush?.Dispose();
				checkedBorderNormalPen?.Dispose();
				checkedBorderHoverPen?.Dispose();
				checkedBorderDownPen?.Dispose();
				uncheckedBorderPen?.Dispose();
				textBrush?.Dispose();
			}
		}
	}
}
