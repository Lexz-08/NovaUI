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
		private Color _borderColor = Constants.BorderColor;
		private Color _activeColor = Constants.AccentColor;
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
		/// Occurs when the value of the <see cref="ActiveColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the ActiveColor property changes.")]
		public event EventHandler ActiveColorChanged;

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
		/// Raises the <see cref="ActiveColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnActiveColorChanged(EventArgs e)
		{
			ActiveColorChanged?.Invoke(this, e);
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
		/// Gets or sets the border color of the control when selected.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border color of the control when selected.")]
		public Color ActiveColor
		{
			get => _activeColor;
			set { _activeColor = value; OnActiveColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the border width of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border width of the control.")]
		public int BorderWidth
		{
			get => _borderWidth;
			set
			{
				if (value < 0) value = 0;
				_borderWidth = value;
				OnBorderWidthChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the border radius of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border radius of the control.")]
		public int BorderRadius
		{
			get => _borderRadius;
			set
			{
				if (value < 0) value = 0;
				else if (value > Math.Min(Width, Height) / 2)
					value = Math.Min(Width, Height) / 2;
				if (value != _borderRadius)
					Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, _borderRadius, _borderRadius));
				_borderRadius = value;
				OnBorderRadiusChanged(EventArgs.Empty);
			}
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

		public NovaButton()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;
			Size = new Size(100, 30);
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, _borderRadius, _borderRadius));
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

			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			if (_borderRadius > 0)
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (_borderWidth > 0)
				{
					e.Graphics.FillPath((_mouseHover ? BackColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : BackColor).ToBrush(),
						new Rectangle(_borderWidth - 1, _borderWidth - 1, Width - (_borderWidth * 2) + 1, Height - (_borderWidth * 2) + 1).Roundify(Math.Max(1, _borderRadius - _borderWidth)));
					for (int i = 0; i < _borderWidth; i++)
						e.Graphics.DrawPath(new Pen((_mouseDown ? _activeColor : _borderColor).ToBrush()),
							new Rectangle(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1).Roundify(_borderRadius - i));
				}
				else
				{
					e.Graphics.FillPath((_mouseHover ? (_mouseDown ? _activeColor : BackColor.Lighter(0.1f)) : BackColor).ToBrush(),
						new Rectangle(0, 0, Width - 1, Height - 1).Roundify(_borderRadius));
					e.Graphics.DrawPath(new Pen((_mouseHover ? (_mouseDown ? _activeColor : BackColor.Lighter(0.1f)) : BackColor).ToBrush()),
						new Rectangle(0, 0, Width - 1, Height - 1).Roundify(_borderRadius));
				}
			}
			else
			{
				if (_borderWidth > 0)
				{
					e.Graphics.FillRectangle((_mouseHover ? BackColor.Lighter(0.1f).Darker(_mouseDown ? 0.1f : 0) : BackColor).ToBrush(),
						new Rectangle(_borderWidth, _borderWidth, Width - (_borderWidth * 2), Height - (_borderWidth * 2)));
					for (int i = 0; i < _borderWidth; i++)
						e.Graphics.DrawRectangle(new Pen((_mouseDown ? _activeColor : _borderColor).ToBrush()),
							new Rectangle(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1));
				}
				else e.Graphics.FillRectangle((_mouseHover ? (_mouseDown ? _activeColor : BackColor.Lighter(0.1f)) : BackColor).ToBrush(),
						new Rectangle(0, 0, Width, Height));
			}

			e.Graphics.DrawString(Text, Font, ForeColor.ToBrush(),
				new Rectangle(0, 0, Width, Height), Constants.CenterAlign);
		}
	}
}
