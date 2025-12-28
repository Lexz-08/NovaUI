using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using NovaUI.Enums;
using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Windows
{
	[DefaultEvent("Load")]
	public class NovaToolWindow : Form
	{
		private int v = 2;
		private Win32.MARGINS margins = new Win32.MARGINS()
		{
			leftWidth = 0,
			rightWidth = 0,
			topHeight = 0,
			bottomHeight = 1
		};

		private readonly SolidBrush headerBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush closeBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush borderBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush backBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush textBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush captionHoverBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush closeDebugBrush = Color.Transparent.ToBrush();

		private Color headerColor = Constants.SecondaryColor;
		private Color closeColor = Color.Red;
		private Color borderColor = Constants.BorderColor;
		private bool stretchCaptions = false;
		private bool canResize = true;
		private bool animateWindow = false;
		private bool showCaptionBox = true;
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
			bottomLeft, bottom, bottomRight,
			header, close;
		private Size captionIcon;
		private Point mouse;

		private const int headerHeight = 32, resizeWidth = 6;

		/// <summary>
		/// Occurs when the value of the <see cref="HeaderColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the HeaderColor property changes.")]
		public event EventHandler HeaderColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="CloseColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the CloseColor property changes.")]
		public event EventHandler CloseColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		protected virtual void OnHeaderColorChanged(EventArgs e)
		{
			HeaderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnCloseColorChanged(EventArgs e)
		{
			CloseColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnBorderColorChanged(EventArgs e)
		{
			BorderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the background color of the form header.")]
		public Color HeaderColor
		{
			get => headerColor;
			set
			{
				headerColor = value;
				if (headerBrush.Color != value)
				{
					headerBrush.Color = value;
					captionHoverBrush.Color = value.Lighter(0.1f);
				}

				OnHeaderColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the color of the close button on the form header.")]
		public Color CloseColor
		{
			get => closeColor;
			set
			{
				closeColor = value;
				if (closeBrush.Color != value)
				{
					closeBrush.Color = value;
					closeDebugBrush.Color = value.Lighter(0.5f);
				}

				OnCloseColorChanged(EventArgs.Empty);
			}
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
		[Category("Appearance"), Description("Gets or sets a value indicating whether the caption icons on the form header will be stretched to their caption boundaries.\n\nNote: This is just for fun to make a slight alteration to the form header captions.")]
		public bool StretchCaptions
		{
			get => stretchCaptions;
			set { stretchCaptions = value; Invalidate(); }
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
			set
			{
				animateWindow = value;
				if (value) useAeroSnap = false;

				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets a value indicating whether button boxes will be displayed when the mouse pointer is hovered over the caption buttons.")]
		public bool ShowCaptionBox
		{
			get => showCaptionBox;
			set { showCaptionBox = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form will utilize Windows Aero snapping.")]
		public bool UseAeroSnap
		{
			get => useAeroSnap;
			set
			{
				useAeroSnap = value;
				if (value) animateWindow = false;
				FormBorderStyle = value ? (canResize ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle) : FormBorderStyle.None;

				Invalidate();
			}
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
		[Category("Appearance"), Description("Gets or sets the cursor that displayed when the mouse pointer is over the form.")]
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
		[Obsolete("The window state of the tool window does not need to be modified.", true)]
		public new FormWindowState WindowState
		{
			get => base.WindowState;
			set { base.WindowState = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The minimize button is not needed for the tool window.", true)]
		public new bool MinimizeBox
		{
			get => base.MinimizeBox;
			set { base.MinimizeBox = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The maximize button is not needed for the tool window.", true)]
		public new bool MaximizeBox
		{
			get => base.MaximizeBox;
			set { base.MaximizeBox = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Panel Content { get; private set; }

		public NovaToolWindow()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			headerBrush = headerColor.ToBrush();
			closeBrush = closeColor.ToBrush();
			borderBrush = borderColor.ToBrush();
			captionHoverBrush = headerColor.Lighter(0.1f).ToBrush();
			closeDebugBrush = closeColor.Lighter(0.5f).ToBrush();
			BackColor = Constants.PrimaryColor;
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
			else if (close.Contains(e.Location))
			{
				if (animateWindow) Fade(false, Close);
				else Close();
			}
			else if (header.Contains(e.Location))
			{
				Win32.ReleaseCapture();
				_ = Win32.SendMessage(Handle, 161, 2, 0);
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

		protected override void OnForeColorChanged(EventArgs e)
		{
			base.OnForeColorChanged(e);
			if (textBrush.Color != ForeColor) textBrush.Color = ForeColor;
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

			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			header = new Rectangle(0, 0, Width, headerHeight);

			if (!useAeroShadow)
			{
				e.Graphics.FillRectangle(borderBrush, 0, 0, Width, Height);
				e.Graphics.FillRectangle(backBrush, 1, 1, Width - 2, Height - 2);
				e.Graphics.FillRectangle(headerBrush, header.X + 1, header.Y + 1, header.Width - 2, header.Height - 2);
			}
			else
			{
				e.Graphics.FillRectangle(backBrush, 0, 0, Width, Height);
				e.Graphics.FillRectangle(headerBrush, header);
			}

			topLeft = new Rectangle(0, 0, resizeWidth, resizeWidth);
			top = new Rectangle(resizeWidth, 0, Width - (resizeWidth * 2), resizeWidth);
			topRight = new Rectangle(Width - resizeWidth, 0, resizeWidth, resizeWidth);
			left = new Rectangle(0, resizeWidth, resizeWidth, Height - (resizeWidth * 2));
			right = new Rectangle(Width - resizeWidth, resizeWidth, resizeWidth, Height - (resizeWidth * 2));
			bottomLeft = new Rectangle(0, Height - resizeWidth, resizeWidth, resizeWidth);
			bottom = new Rectangle(resizeWidth, Height - resizeWidth, Width - (resizeWidth * 2), resizeWidth);
			bottomRight = new Rectangle(Width - resizeWidth, Height - resizeWidth, resizeWidth, resizeWidth);

			Rectangle text = new Rectangle(resizeWidth, resizeWidth - 1, Width - (resizeWidth * 2), headerHeight - (resizeWidth * 2));
			if (ShowIcon)
			{
				text.X += headerHeight - (resizeWidth * 2) + 2;
				text.Width -= headerHeight - (resizeWidth * 2) + 2;
			}
			if (useAeroShadow) text.Y += 1;

			if (ShowIcon)
			{
				if (Icon != null)
				{
					SmoothingMode smode = e.Graphics.SmoothingMode;
					e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
					e.Graphics.DrawImage(Icon.ToBitmap(), new Rectangle(resizeWidth, resizeWidth + (useAeroShadow ? 1 : 0) - 1, headerHeight - (resizeWidth * 2), headerHeight - (resizeWidth * 2)));
					e.Graphics.SmoothingMode = smode;
				}
				else e.Graphics.FillRectangle(borderBrush, new Rectangle(resizeWidth, resizeWidth + (useAeroShadow ? 1 : 0) - 1, headerHeight - (resizeWidth * 2), headerHeight - (resizeWidth * 2)));
			}

			e.Graphics.DrawString(Text, Font, textBrush, text, Constants.LeftAlign);

			// for visual debugging of the Form layout
			bool debug = false;

			close = new Rectangle(Width - headerHeight - resizeWidth, resizeWidth, headerHeight, headerHeight - (resizeWidth * 2));

			captionIcon = new Size(headerHeight - (resizeWidth * 2), headerHeight - (resizeWidth * 2));
			mouse = PointToClient(MousePosition);

			if (showCaptionBox)
			{
				e.Graphics.FillRectangle(close.Contains(mouse) ? captionHoverBrush : headerBrush, close.X, close.Y - resizeWidth + 1, headerHeight, headerHeight - (useAeroShadow ? 1 : 2));
			}
			if (debug)
			{
				e.Graphics.FillRectangle(closeBrush, close);
				e.Graphics.FillRectangle(closeDebugBrush,
					new Rectangle(close.X + ((close.Width - captionIcon.Width) / 2), close.Y, captionIcon.Width, captionIcon.Height));
			}
			e.Graphics.DrawImage(Bitmaps.Instance[headerHeight, close.Contains(mouse) ? closeColor : ForeColor, BitmapType.Close],
				new Rectangle(close.X + (!stretchCaptions ? ((close.Width - captionIcon.Width) / 2) : 0), close.Y,
				!stretchCaptions ? captionIcon.Width : close.Width, !stretchCaptions ? captionIcon.Height : close.Height)
				.Rescale(-resizeWidth, resizeWidth * 2));
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
				headerBrush?.Dispose();
				closeBrush?.Dispose();
				borderBrush?.Dispose();
				backBrush?.Dispose();
				textBrush?.Dispose();
				captionHoverBrush?.Dispose();
				closeDebugBrush?.Dispose();
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
