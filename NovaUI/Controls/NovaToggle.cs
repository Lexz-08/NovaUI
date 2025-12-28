using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.Designers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(CheckBox))]
	[DefaultEvent("CheckedChanged")]
	[Designer(typeof(NoResizeDesigner))]
	public class NovaToggle : CheckBox
	{
		private readonly Pen borderPen = new Pen(Color.Transparent, 2);
		private readonly SolidBrush checkedToggleNormalBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush checkedToggleHoverBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush checkedToggleDownBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush uncheckedToggleNormalBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush uncheckedToggleHoverBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush uncheckedToggleDownBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush backBrush = Color.Transparent.ToBrush();

		private Color borderColor = Constants.BorderColor;
		private Color checkColor = Constants.AccentColor;
		private bool useUserSchemeCursor = true;
		private int toggleSize = 24;
		private Cursor originalCursor = Cursors.Hand;

		private bool mouseHover = false;
		private bool mouseDown = false;

		private int radius;

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
		/// Occurs when the value of the <see cref="ToggleSize"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the ToggleSize property changes.")]
		public event EventHandler ToggleSizeChanged;

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

		protected virtual void OnToggleSizeChanged(EventArgs e)
		{
			ToggleSizeChanged?.Invoke(this, e);
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
				if (borderPen.Color != value)
				{
					borderPen.Color = value;
					uncheckedToggleNormalBrush.Color = value;
					uncheckedToggleHoverBrush.Color = value.Lighter(0.1f);
					uncheckedToggleDownBrush.Color = value.Lighter(0.1f).Darker(0.1f);
				}
				OnBorderColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the background color of the control toggle knob in a checked state.")]
		public Color CheckColor
		{
			get => checkColor;
			set
			{
				checkColor = value;
				if (checkedToggleNormalBrush.Color != value)
				{
					checkedToggleNormalBrush.Color = value;
					checkedToggleHoverBrush.Color = value.Lighter(0.1f);
					checkedToggleDownBrush.Color = value.Lighter(0.1f).Darker(0.1f);
				}
				OnCheckColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the control will use the user-selected system scheme cursor.")]
		public bool UseUserSchemeCursor
		{
			get => useUserSchemeCursor;
			set { useUserSchemeCursor = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the size of the toggle.")]
		public int ToggleSize
		{
			get => toggleSize;
			set
			{
				toggleSize = Math.Max(16, value);
				Size = new Size(value * 2, value);
				OnToggleSizeChanged(EventArgs.Empty);
				Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, Math.Min(Width, Height) / 2, Math.Min(Width, Height) / 2));
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The ForeColor property of this control is not needed.", true)]
		public new Color ForeColor = Color.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The Text property of this control is not needed.")] // Designer doesn't understand what obselete means apparently XD
		public new string Text = string.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The Font property of this control is not needed.", true)]
		public new Font Font => new Font("Segoe UI", 0.1f);

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The AutoSize property of this control is not needed.", true)]
#pragma warning disable CS0809
		public override bool AutoSize { get; set; } = true;
#pragma warning disable CS0809

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size Size
		{
			get => base.Size;
			set { base.Size = value; OnSizeChanged(EventArgs.Empty); Invalidate(); }
		}

		public NovaToggle()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			borderPen.Color = borderColor;
			uncheckedToggleNormalBrush.Color = borderColor;
			uncheckedToggleHoverBrush.Color = borderColor.Lighter(0.1f);
			uncheckedToggleDownBrush.Color = borderColor.Lighter(0.1f).Darker(0.1f);
			checkedToggleNormalBrush.Color = checkColor;
			checkedToggleHoverBrush.Color = checkColor.Lighter(0.1f);
			checkedToggleDownBrush.Color = checkColor.Lighter(0.1f).Darker(0.1f);
			BackColor = Constants.PrimaryColor;
			Size = new Size(48, 24);
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, Math.Min(Width, Height) / 2, Math.Min(Width, Height) / 2));
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if ((Width, Height) != (toggleSize * 2, toggleSize)) Size = new Size(toggleSize * 2, toggleSize);

			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, Math.Min(Width, Height) / 2, Math.Min(Width, Height) / 2));
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if ((Width, Height) != (toggleSize * 2, toggleSize)) Size = new Size(toggleSize * 2, toggleSize);

			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, Math.Min(Width, Height) / 2, Math.Min(Width, Height) / 2));
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
			if (backBrush.Color != BackColor) backBrush.Color = BackColor;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			radius = Math.Min(Width, Height) / 2;

			e.Graphics.Clear(Parent != null ? Parent.BackColor : Color.Transparent);

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			e.Graphics.FillPath(backBrush, new RectangleF(0.5f, 0.5f, Width - 2, Height - 2).Round(radius));
			e.Graphics.DrawPath(borderPen, new RectangleF(0.5f, 0.5f, Width - 2, Height - 2).Round(radius - 2));
			e.Graphics.FillEllipse(Checked
				? (mouseHover ? (mouseDown ? checkedToggleDownBrush : checkedToggleHoverBrush) : checkedToggleNormalBrush)
				: (mouseHover ? (mouseDown ? uncheckedToggleDownBrush : uncheckedToggleHoverBrush) : uncheckedToggleNormalBrush),
				Checked ? Width - (Height - 12) - 7 : 6, 5.5f, Height - 12, Height - 12);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				borderPen?.Dispose();
				checkedToggleNormalBrush?.Dispose();
				checkedToggleHoverBrush?.Dispose();
				checkedToggleDownBrush?.Dispose();
				uncheckedToggleNormalBrush?.Dispose();
				uncheckedToggleHoverBrush?.Dispose();
				uncheckedToggleDownBrush?.Dispose();
				backBrush?.Dispose();
			}
		}
	}
}
