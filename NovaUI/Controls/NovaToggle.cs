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
		private Color _borderColor = Constants.BorderColor;
		private Color _checkColor = Constants.AccentColor;
		private bool _useUserSchemeCursor = true;
		private int _tglSize = 24;
		private Cursor _originalCrsr = Cursors.Hand;

		private bool _mouseHover = false;
		private bool _mouseDown = false;

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
		/// Raises the <see cref="CheckColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnCheckColorChanged(EventArgs e)
		{
			CheckColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="ToggleSizeChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnToggleSizeChanged(EventArgs e)
		{
			ToggleSizeChanged?.Invoke(this, e);
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
		/// Gets or sets the background color of the control toggle knob in a checked state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the control toggle knob in a checked state.")]
		public Color CheckColor
		{
			get => _checkColor;
			set { _checkColor = value; OnCheckColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control will use the user-selected system scheme cursor.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the control will use the user-selected system scheme cursor.")]
		public bool UseUserSchemeCursor
		{
			get => _useUserSchemeCursor;
			set { _useUserSchemeCursor = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the size of the toggle.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the size of the toggle.")]
		public int ToggleSize
		{
			get => _tglSize;
			set
			{
				_tglSize = Math.Max(16, value);
				Size = new Size(value * 2, value);
				OnToggleSizeChanged(EventArgs.Empty);
				Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, Math.Min(Width, Height) / 2, Math.Min(Width, Height) / 2));
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The ForeColor property of this control is not needed.", true)]
		public new Color ForeColor => Color.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The Text property of this control is not needed.", true)]
		public new string Text => string.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The Font property of this control is not needed.", true)]
		public new Font Font => new Font("Segoe UI", 0.1f);

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The AutoSize property of this control is not needed.", true)]
		public override bool AutoSize { get; set; } = true;

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

			BackColor = Constants.PrimaryColor;
			Size = new Size(48, 24);
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, Math.Min(Width, Height) / 2, Math.Min(Width, Height) / 2));
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (Size != new Size(_tglSize * 2, _tglSize)) Size = new Size(_tglSize * 2, _tglSize);

			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, Math.Min(Width, Height) / 2, Math.Min(Width, Height) / 2));
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (Size != new Size(_tglSize * 2, _tglSize)) Size = new Size(_tglSize * 2, _tglSize);

			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, Math.Min(Width, Height) / 2, Math.Min(Width, Height) / 2));
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);

			_mouseHover = true;
			if (_useUserSchemeCursor) Cursor = Win32.RegCursor("Hand");
			else Cursor = _originalCrsr;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			_mouseHover = false;
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			_mouseDown = true;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			_mouseDown = false;
			Invalidate();
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			int radius = Math.Min(Width, Height) / 2;

			e.Graphics.Clear(Parent.BackColor);

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			e.Graphics.FillPath(BackColor.ToBrush(), new Rectangle(2, 2, Width - 5, Height - 5).Roundify(radius));
			e.Graphics.DrawPath(new Pen(_borderColor.ToBrush(), 1), new Rectangle(0, 0, Width - 1, Height - 1).Roundify(radius));
			e.Graphics.DrawPath(new Pen(_borderColor.ToBrush(), 1), new RectangleF(0.5f, 0.5f, Width - 2, Height - 2).Roundify(radius));
			e.Graphics.FillEllipse((
				Checked ? (_mouseHover ? _checkColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _checkColor) :
				(_mouseHover ? _borderColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _borderColor)
				).ToBrush(), Checked ? Width - (Height - 12) - 7 : 6, 5.5f, Height - 12, Height - 12);
		}
	}
}
