using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(RadioButton))]
	public class NovaRadioButton : RadioButton
	{
		private Color _borderColor = Constants.BorderColor;
		private Color _radioColor = Constants.AccentColor;
		private int _borderWidth = 1;
		private int _borderRadius = 6;
		private bool _useUserSchemeCursor = true;
		private Cursor _originalCrsr = Cursors.Hand;

		private bool _mouseHover = false;
		private bool _mouseDown = false;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="RadioColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the RadioColor property changes.")]
		public event EventHandler RadioColorChanged;

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
		/// Raises the <see cref="RadioColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnRadioColorChanged(EventArgs e)
		{
			RadioColorChanged?.Invoke(this, e);
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
		/// Gets or sets the border color of the check box.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border color of the control check box.")]
		public Color BorderColor
		{
			get => _borderColor;
			set { _borderColor = value; OnBorderColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the background color of the check box.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the control check box.")]
		public Color RadioColor
		{
			get => _radioColor;
			set { _radioColor = value; OnRadioColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the border width of the check box.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border width of the check box.")]
		public int BorderWidth
		{
			get => _borderWidth;
			set
			{
				if (value < 1) value = 1;
				_borderWidth = value;
				OnBorderWidthChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the border radius of the check box.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border radius of the check box.")]
		public int BorderRadius
		{
			get => _borderRadius;
			set
			{
				if (value < 0) value = 0;
				else if (value > Height / 2)
					value = Height / 2;
				_borderRadius = value;
				OnBorderRadiusChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the check box will use the user-selected system scheme cursor.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the check box will use the user-selected system scheme cursor.")]
		public bool UseUserSchemeCursor
		{
			get => _useUserSchemeCursor;
			set { _useUserSchemeCursor = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the cursor that is displayed when the mouse pointer is over the control.
		/// </summary>
		/// <returns>
		/// A <see cref="Cursor"/> that represents the cursor to display when the mouse pointer is over the control.
		/// </returns>
		[Category("Appearance"), Description("Gets or sets the cursor that is displayed when the mouse pointer is over the control.")]
		public override Cursor Cursor
		{
			get => base.Cursor;
			set
			{
				base.Cursor = value;
				if (!_useUserSchemeCursor) _originalCrsr = value;

				OnCursorChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		public NovaRadioButton()
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

			e.Graphics.Clear(Parent.BackColor);
			int size = 16;
			int pos = (Height - size) / 2;

			if (_borderRadius > 0)
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (_borderWidth > 0)
				{
					e.Graphics.FillPath((Checked ? (_mouseHover ? _radioColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _radioColor) : (_mouseHover ? BackColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : BackColor)).ToBrush(),
						new Rectangle(_borderWidth - 1, pos + _borderWidth - 1, size - (_borderWidth * 2) + 1, size - (_borderWidth * 2) + 1).Roundify(Math.Max(1, _borderRadius - _borderWidth)));
					for (int i = 0; i < _borderWidth; i++)
						e.Graphics.DrawPath(new Pen((Checked ? (_mouseHover ? _radioColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _radioColor) : _borderColor).ToBrush()),
							new Rectangle(i, pos + i, size - (i * 2) - 1, size - (i * 2) - 1).Roundify(_borderRadius - i));
				}
				else
				{
					e.Graphics.FillPath((Checked ? (_mouseHover ? _radioColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _radioColor) : (_mouseHover ? BackColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : BackColor)).ToBrush(),
						new Rectangle(0, pos, size - 1, size - 1).Roundify(_borderRadius));
					e.Graphics.DrawPath(new Pen((Checked ? (_mouseHover ? _radioColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _radioColor) : _borderColor).ToBrush()),
						new Rectangle(0, pos, size - 1, size - 1).Roundify(_borderRadius));
				}
			}
			else
			{
				if (_borderWidth > 0)
				{
					e.Graphics.FillRectangle((Checked ? (_mouseHover ? _radioColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _radioColor) : (_mouseHover ? BackColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : BackColor)).ToBrush(),
						new Rectangle(_borderWidth, pos + _borderWidth, size - (_borderWidth * 2), size - (_borderWidth * 2)));
					for (int i = 0; i < _borderWidth; i++)
						e.Graphics.DrawRectangle(new Pen((Checked ? (_mouseHover ? _radioColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _radioColor) : _borderColor).ToBrush()),
							new Rectangle(i, pos + i, size - (i * 2) - 1, size - (i * 2) - 1));
				}
				else e.Graphics.FillRectangle((Checked ? (_mouseHover ? _radioColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : _radioColor) : (_mouseHover ? BackColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : BackColor)).ToBrush(),
						new Rectangle(0, pos, size, size));
			}

			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			bool debug = false;
			Rectangle text = new Rectangle(size + 1, 0, Width - size + 2, Height);
			if (debug) e.Graphics.FillRectangle(ForeColor.Darker(0.5f).ToBrush(), text);
			e.Graphics.DrawString(Text, Font, ForeColor.ToBrush(), text, Constants.LeftAlign);
		}
	}
}
