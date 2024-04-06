using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Enums;
using NovaUI.EventManagement.ArgumentContainers;
using NovaUI.EventManagement.Handlers;
using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	public class NovaStrippedWindow : Form
	{
		private Color _borderColor = Constants.BorderColor;
		private bool _canResize = true;
		private bool _animateWindow = false;
		private bool _useAeroSnap = true;
		private bool _useAeroShadow = true;
		private bool _useUserSchemeCursor = true;
		private Cursor _originalCrsr = Cursors.Default;

		private bool _aeroEnabled = false;
		private bool _canFade = true;
		private Size _stateChangeSize;
		private CursorType _cursorType = CursorType.Arrow;

		private Rectangle _topLeft, _top, _topRight,
			_left, _right,
			_bottomLeft, _bottom, _bottomRight;

		private int _resizeWidth = 6;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Occurs when the window state of the form changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the window state of the form changes.")]
		public event WindowStateChangedEventHandler WindowStateChanged;

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
		/// Raises the <see cref="WindowStateChanged"/> event.
		/// </summary>
		/// <param name="e">A WindowStateChangedEventArgs that contains the event data specifying the previous and current window state of the form.</param>
		protected virtual void OnWindowStateChanged(WindowStateChangedEventArgs e)
		{
			WindowStateChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Gets or sets the border color of the form.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border color of the form.")]
		public Color BorderColor
		{
			get => _borderColor;
			set { _borderColor = value; OnBorderColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the form can be resized by the user.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form can be resized by the user.")]
		public bool CanResize
		{
			get => _canResize;
			set { _canResize = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the form's window state changes will be animated.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form's window state changes will be animated.")]
		public bool AnimateWindow
		{
			get => _animateWindow;
			set { _animateWindow = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the form will utilize Windows Aero snapping.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form will utilize Windows Aero snapping.")]
		public bool UseAeroSnap
		{
			get => _useAeroSnap;
			set { _useAeroSnap = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the form will display with the Windows Aero drop shadow behind it.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form will display with the Windows Aero drop shadow behind it.")]
		public bool UseAeroShadow
		{
			get => _useAeroShadow;
			set { _useAeroShadow = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the form will use the user-selected system scheme cursor.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form will use the user-selected system scheme cursor.")]
		public bool UseUserSchemeCursor
		{
			get => _useUserSchemeCursor;
			set { _useUserSchemeCursor = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the cursor that displayed when the mouse pointer is over the form.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the cursor that displayed when the mouse pointer is over the form.")]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new FormBorderStyle FormBorderStyle
		{
			get => base.FormBorderStyle;
			set { base.FormBorderStyle = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool DoubleBuffered
		{
			get => base.DoubleBuffered;
			set { base.DoubleBuffered = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShowIcon
		{
			get => base.ShowIcon;
			set { base.ShowIcon = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value that indicates whether form is minimized, maximized, or normal.
		/// </summary>
		/// <returns>
		/// A <see cref="FormWindowState"/> that represents whether form is minimized, maximized, or normal. The default is <see langword="FormWindowState.Normal"/>.
		/// </returns>
		/// <exception cref="InvalidEnumArgumentException"></exception>
		[Category("Behavior"), Description("Gets or sets a value that indicates whether form is minimized, maximized, or normal.")]
		public new FormWindowState WindowState
		{
			get => base.WindowState;
			set
			{
				FormWindowState prevState = base.WindowState;
				base.WindowState = value;
				if (value != prevState) OnWindowStateChanged(new WindowStateChangedEventArgs(prevState, value));
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Icon Icon
		{
			get => base.Icon;
			set { base.Icon = value; Invalidate(); }
		}

		public NovaStrippedWindow()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;

			MinimumSize = new Size(200, 100);
			FormBorderStyle = FormBorderStyle.Sizable;
		}

		private void Fade(bool fadeIn, Action callback = null)
		{
			if (_canFade)
			{
				_canFade = false;

				Timer t = new Timer { Interval = 10 };
				t.Tick += (_, __) =>
				{
					if (Opacity == (fadeIn ? 1 : 0))
					{
						t.Enabled = false;
						_canFade = true;
						callback?.Invoke();
					}

					Opacity += fadeIn ? 0.05 : -0.05;
				};
				t.Start();
			}
		}

		protected void DragForm()
		{
			Win32.ReleaseCapture();
			Win32.SendMessage(Handle, 161, 2, 0);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			CursorType newCursor = 0;

			if ((_left.Contains(e.Location) || _right.Contains(e.Location)) && _canResize)
			{
				newCursor = CursorType.SizeWE;
				if (newCursor != _cursorType)
				{
					_cursorType = newCursor;

					if (_useUserSchemeCursor) Cursor = Win32.RegCursor("SizeWE");
					else Cursor = Cursors.SizeWE;
				}
			}
			else if ((_top.Contains(e.Location) || _bottom.Contains(e.Location)) && _canResize)
			{
				newCursor = CursorType.SizeNS;
				if (newCursor != _cursorType)
				{
					_cursorType = newCursor;

					if (_useUserSchemeCursor) Cursor = Win32.RegCursor("SizeNS");
					else Cursor = Cursors.SizeNS;
				}
			}
			else if ((_topLeft.Contains(e.Location) || _bottomRight.Contains(e.Location)) && _canResize)
			{
				newCursor = CursorType.SizeNWSE;
				if (newCursor != _cursorType)
				{
					_cursorType = newCursor;

					if (_useUserSchemeCursor) Cursor = Win32.RegCursor("SizeNWSE");
					else Cursor = Cursors.SizeNWSE;
				}
			}
			else if ((_topRight.Contains(e.Location) || _bottomLeft.Contains(e.Location)) && _canResize)
			{
				newCursor = CursorType.SizeNESW;
				if (newCursor != _cursorType)
				{
					_cursorType = newCursor;

					if (_useUserSchemeCursor) Cursor = Win32.RegCursor("SizeNESW");
					else Cursor = Cursors.SizeNESW;
				}
			}
			else
			{
				newCursor = CursorType.Arrow;
				if (newCursor != _cursorType)
				{
					_cursorType = newCursor;

					if (_useUserSchemeCursor) Cursor = Win32.RegCursor("Arrow");
					else Cursor = _originalCrsr;
				}
			}

			GC.Collect();
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (_left.Contains(e.Location) && _canResize)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 10, 0);
			}
			else if (_right.Contains(e.Location) && _canResize)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 11, 0);
			}
			else if (_top.Contains(e.Location) && _canResize)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 12, 0);
			}
			else if (_topLeft.Contains(e.Location) && _canResize)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 13, 0);
			}
			else if (_topRight.Contains(e.Location) && _canResize)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 14, 0);
			}
			else if (_bottom.Contains(e.Location) && _canResize)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 15, 0);
			}
			else if (_bottomLeft.Contains(e.Location) && _canResize)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 16, 0);
			}
			else if (_bottomRight.Contains(e.Location) && _canResize)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 17, 0);
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (_aeroEnabled && _useAeroShadow)
			{
				int v = 2;
				Win32.DwmSetWindowAttribute(Handle, 2, ref v, 4);
				Win32.MARGINS margins = new Win32.MARGINS
				{
					leftWidth = 0,
					rightWidth = 0,
					topHeight = 0,
					bottomHeight = 1
				};
				Win32.DwmExtendFrameIntoClientArea(Handle, ref margins);
			}

			if (!_useAeroShadow)
			{
				e.Graphics.FillRectangle(_borderColor.ToBrush(), 0, 0, Width, Height);
				e.Graphics.FillRectangle(BackColor.ToBrush(), 1, 1, Width - 2, Height - 2);
			}
			else if (_useAeroShadow)
				e.Graphics.FillRectangle(BackColor.ToBrush(), 0, 0, Width, Height);

			_topLeft = new Rectangle(0, 0, _resizeWidth, _resizeWidth);
			_top = new Rectangle(_resizeWidth, 0, Width - (_resizeWidth * 2), _resizeWidth);
			_topRight = new Rectangle(Width - _resizeWidth, 0, _resizeWidth, _resizeWidth);
			_left = new Rectangle(0, _resizeWidth, _resizeWidth, Height - (_resizeWidth * 2));
			_right = new Rectangle(Width - _resizeWidth, _resizeWidth, _resizeWidth, Height - (_resizeWidth * 2));
			_bottomLeft = new Rectangle(0, Height - _resizeWidth, _resizeWidth, _resizeWidth);
			_bottom = new Rectangle(_resizeWidth, Height - _resizeWidth, Width - (_resizeWidth * 2), _resizeWidth);
			_bottomRight = new Rectangle(Width - _resizeWidth, Height - _resizeWidth, _resizeWidth, _resizeWidth);
		}

		protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);

			if (_canResize && !_useAeroSnap)
			{
				Point screenLoc = Screen.FromPoint(Location).Bounds.Location;
				int screenY = screenLoc.Y;

				//bool windowSnap = Location.Y <= screenY + 6 && Location.Y >= screenY;
				//bool mouseSnap = MousePosition.Y <= screenY + 6 && MousePosition.Y >= screenY;
				bool topSnap = (Location.Y <= screenY + 6 && Location.Y >= screenY) ||
					(MousePosition.Y <= screenY + 6 && Location.Y >= screenY);

				if (topSnap) WindowState = FormWindowState.Maximized;
			}
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			if (_animateWindow) Fade(true);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (_animateWindow)
			{
				Opacity = 0;
				Invalidate();
			}

			_stateChangeSize = ClientSize;
			Size = new Size(ClientSize.Width - 16, ClientSize.Height - 39);
		}

		protected override void WndProc(ref Message m)
		{
			const int WM_NCCALCSIZE = 0x0083;
			const int WM_SYSCOMMAND = 0x0112;
			const int SC_MINIMIZE = 0xF020;
			const int SC_RESTORE = 0xF120;

			if (_useAeroSnap && !DesignMode)
			{
				if (m.Msg == WM_NCCALCSIZE && (int)m.WParam == 1)
					return;

				if (m.Msg == WM_SYSCOMMAND)
				{
					int wParam = (int)m.WParam & 0xFFF0;

					if (wParam == SC_MINIMIZE) _stateChangeSize = ClientSize;
					if (wParam == SC_RESTORE) Size = _stateChangeSize;
				}
			}

			base.WndProc(ref m);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				_aeroEnabled = Win32.CheckAeroEnabled();

				if (!_aeroEnabled && _useAeroShadow)
					cp.ClassStyle |= 0x20000;

				return cp;
			}
		}
	}
}
