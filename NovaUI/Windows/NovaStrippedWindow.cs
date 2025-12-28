using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Enums;
using NovaUI.Events;
using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Windows
{
	[DefaultEvent("Load")]
	public class NovaStrippedWindow : Form
	{
		private int v = 2;
		private Win32.MARGINS margins = new Win32.MARGINS()
		{
			leftWidth = 0,
			rightWidth = 0,
			topHeight = 0,
			bottomHeight = 1
		};

		private readonly SolidBrush borderBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush backBrush = Color.Transparent.ToBrush();

		private Color borderColor = Constants.BorderColor;
		private bool canResize = true;
		private bool animateWindow = false;
		private bool useAeroSnap = true;
		private bool useAeroShadow = true;
		private bool useUserSchemeCursor = true;
		private Cursor originalCursor = Cursors.Default;

		private bool aeroEnabled = false;
		private bool canFade = true;
		private Size stateChangeSize;
		private CursorType cursorType = CursorType.Arrow;
		private Timer fadeTimer = new Timer() { Interval = 10 };

		private Rectangle topLeft, top, topRight,
			left, right,
			bottomLeft, bottom, bottomRight;

		private int resizeWidth = 6;

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

		protected virtual void OnBorderColorChanged(EventArgs e)
		{
			BorderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnWindowStateChanged(WindowStateChangedEventArgs e)
		{
			WindowStateChanged?.Invoke(this, e);
			Invalidate();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the border color of the form.")]
		public Color BorderColor
		{
			get => borderColor;
			set
			{
				borderColor = value;
				if (borderBrush.Color != value) borderBrush.Color = value;

				OnBorderColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form can be resized by the user.")]
		public bool CanResize
		{
			get => canResize;
			set { canResize = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form's window state changes will be animated.")]
		public bool AnimateWindow
		{
			get => animateWindow;
			set { animateWindow = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form will utilize Windows Aero snapping.")]
		public bool UseAeroSnap
		{
			get => useAeroSnap;
			set { useAeroSnap = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form will display with the Windows Aero drop shadow behind it.")]
		public bool UseAeroShadow
		{
			get => useAeroShadow;
			set { useAeroShadow = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form will use the user-selected system scheme cursor.")]
		public bool UseUserSchemeCursor
		{
			get => useUserSchemeCursor;
			set { useUserSchemeCursor = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the cursor that is displayed when the mouse pointer is over the form.")]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form is minimized, maximized, or normal.")]
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

		public NovaStrippedWindow()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			borderBrush = borderColor.ToBrush();
			backBrush = (BackColor = Constants.PrimaryColor).ToBrush();
			ForeColor = Constants.TextColor;

			MinimumSize = new Size(200, 100);
			FormBorderStyle = FormBorderStyle.Sizable;
		}

		private void Fade(bool fadeIn, Action callback = null)
		{
			if (canFade)
			{
				canFade = false;

				void FadeEvent(object sender, EventArgs e)
				{
					if (Opacity == (fadeIn ? 1 : 0))
					{
						fadeTimer.Enabled = false;
						fadeTimer.Tick -= FadeEvent;
						canFade = true;
						callback?.Invoke();
					}

					Opacity += fadeIn ? 0.05 : -0.05;
				}

				fadeTimer.Tick += FadeEvent;
				fadeTimer.Start();
			}
		}

		protected void DragForm(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Win32.ReleaseCapture();
				_ = Win32.SendMessage(Handle, 161, 2, 0);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if ((left.Contains(e.Location) || right.Contains(e.Location)) && canResize && cursorType != CursorType.SizeWE)
			{
				cursorType = CursorType.SizeWE;

				if (useUserSchemeCursor) Win32.GetRegistryCursor(Win32.RegistryCursor.SizeWE, this);
				else Cursor = Cursors.SizeWE;

				Invalidate();
				GC.Collect();
			}
			else if ((top.Contains(e.Location) || bottom.Contains(e.Location)) && canResize && cursorType != CursorType.SizeNS)
			{
				cursorType = CursorType.SizeNS;

				if (useUserSchemeCursor) Win32.GetRegistryCursor(Win32.RegistryCursor.SizeNS, this);
				else Cursor = Cursors.SizeNS;

				Invalidate();
				GC.Collect();
			}
			else if ((topLeft.Contains(e.Location) || bottomRight.Contains(e.Location)) && canResize && cursorType != CursorType.SizeNWSE)
			{
				cursorType = CursorType.SizeNWSE;

				if (useUserSchemeCursor) Win32.GetRegistryCursor(Win32.RegistryCursor.SizeNWSE, this);
				else Cursor = Cursors.SizeNWSE;

				Invalidate();
				GC.Collect();
			}
			else if ((topRight.Contains(e.Location) || bottomLeft.Contains(e.Location)) && canResize && cursorType != CursorType.SizeNESW)
			{
				cursorType = CursorType.SizeNESW;

				if (useUserSchemeCursor) Win32.GetRegistryCursor(Win32.RegistryCursor.SizeNESW, this);
				else Cursor = Cursors.SizeNESW;

				Invalidate();
				GC.Collect();
			}
			else if (cursorType != CursorType.Arrow)
			{
				cursorType = CursorType.Arrow;

				if (useUserSchemeCursor) Win32.GetRegistryCursor(Win32.RegistryCursor.Arrow, this);
				else Cursor = originalCursor;

				Invalidate();
				GC.Collect();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left)
			{
				if (left.Contains(e.Location) && canResize)
				{
					Win32.ReleaseCapture();
					_ = Win32.SendMessage(Handle, 161, 10, 0);
				}
				else if (right.Contains(e.Location) && canResize)
				{
					Win32.ReleaseCapture();
					_ = Win32.SendMessage(Handle, 161, 11, 0);
				}
				else if (top.Contains(e.Location) && canResize)
				{
					Win32.ReleaseCapture();
					_ = Win32.SendMessage(Handle, 161, 12, 0);
				}
				else if (topLeft.Contains(e.Location) && canResize)
				{
					Win32.ReleaseCapture();
					_ = Win32.SendMessage(Handle, 161, 13, 0);
				}
				else if (topRight.Contains(e.Location) && canResize)
				{
					Win32.ReleaseCapture();
					_ = Win32.SendMessage(Handle, 161, 14, 0);
				}
				else if (bottom.Contains(e.Location) && canResize)
				{
					Win32.ReleaseCapture();
					_ = Win32.SendMessage(Handle, 161, 15, 0);
				}
				else if (bottomLeft.Contains(e.Location) && canResize)
				{
					Win32.ReleaseCapture();
					_ = Win32.SendMessage(Handle, 161, 16, 0);
				}
				else if (bottomRight.Contains(e.Location) && canResize)
				{
					Win32.ReleaseCapture();
					_ = Win32.SendMessage(Handle, 161, 17, 0);
				}
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
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

			if (aeroEnabled && useAeroShadow)
			{
				_ = Win32.DwmSetWindowAttribute(Handle, 2, ref v, 4);
				_ = Win32.DwmExtendFrameIntoClientArea(Handle, ref margins);
			}

			if (!useAeroShadow)
			{
				e.Graphics.FillRectangle(borderBrush, 0, 0, Width, Height);
				e.Graphics.FillRectangle(backBrush, 1, 1, Width - 2, Height - 2);
			}
			else e.Graphics.FillRectangle(backBrush, 0, 0, Width, Height);

			topLeft = new Rectangle(0, 0, resizeWidth, resizeWidth);
			top = new Rectangle(resizeWidth, 0, Width - (resizeWidth * 2), resizeWidth);
			topRight = new Rectangle(Width - resizeWidth, 0, resizeWidth, resizeWidth);
			left = new Rectangle(0, resizeWidth, resizeWidth, Height - (resizeWidth * 2));
			right = new Rectangle(Width - resizeWidth, resizeWidth, resizeWidth, Height - (resizeWidth * 2));
			bottomLeft = new Rectangle(0, Height - resizeWidth, resizeWidth, resizeWidth);
			bottom = new Rectangle(resizeWidth, Height - resizeWidth, Width - (resizeWidth * 2), resizeWidth);
			bottomRight = new Rectangle(Width - resizeWidth, Height - resizeWidth, resizeWidth, resizeWidth);
		}

		protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);

			if (canResize && !useAeroSnap)
			{
				Point screenLoc = Screen.FromPoint(Location).Bounds.Location;
				int screenY = screenLoc.Y;

				if (Location.Y >= screenY && (Location.Y <= screenY + 6 || MousePosition.Y <= screenY))
					WindowState = FormWindowState.Maximized;
			}
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			if (animateWindow) Fade(true);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (animateWindow)
			{
				Opacity = 0;
				Invalidate();
			}

			stateChangeSize = ClientSize;
			Size = new Size(ClientSize.Width - 16, ClientSize.Height - 39);

			if (!DesignMode) WindowStateChanged += (_, _e) =>
			{
				if (_e.CurrentState == FormWindowState.Maximized && _e.PreviousState == FormWindowState.Normal)
					foreach (Control c in Controls)
					{
						if ((c.Anchor & AnchorStyles.Left) != 0) c.Left += 8;
						if ((c.Anchor & AnchorStyles.Top) != 0) c.Top += 8;
						if ((c.Anchor & AnchorStyles.Right) != 0) c.Width -= (c.Anchor & AnchorStyles.Left) != 0 ? 16 : 8;
						if ((c.Anchor & AnchorStyles.Bottom) != 0) c.Height -= (c.Anchor & AnchorStyles.Top) != 0 ? 16 : 8;
					}
				else if (_e.CurrentState == FormWindowState.Normal && _e.PreviousState == FormWindowState.Maximized)
					foreach (Control c in Controls)
					{
						if ((c.Anchor & AnchorStyles.Left) != 0) c.Left -= 8;
						if ((c.Anchor & AnchorStyles.Top) != 0) c.Top -= 8;
						if ((c.Anchor & AnchorStyles.Right) != 0) c.Width += (c.Anchor & AnchorStyles.Left) != 0 ? 16 : 8;
						if ((c.Anchor & AnchorStyles.Bottom) != 0) c.Height += (c.Anchor & AnchorStyles.Top) != 0 ? 16 : 8;
					}
			};
		}

		protected override void WndProc(ref Message m)
		{
			if (useAeroSnap && !DesignMode)
			{
				if (m.Msg == Win32.WM_NCCALCSIZE && (int)m.WParam == 1)
					return;

				if (m.Msg == Win32.WM_SYSCOMMAND)
				{
					int wParam = (int)m.WParam & 0xFFF0;

					if (wParam == Win32.SC_MINIMIZE) stateChangeSize = ClientSize;
					if (wParam == Win32.SC_RESTORE) Size = stateChangeSize;
				}
			}

			base.WndProc(ref m);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				borderBrush?.Dispose();
				backBrush?.Dispose();
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				aeroEnabled = Win32.CheckAeroEnabled();

				if (!aeroEnabled && useAeroShadow)
					cp.ClassStyle |= Win32.CS_DROPSHADOW;

				return cp;
			}
		}
	}
}
