using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	public class NovaToolWindow : Form
	{
		private Color _headerColor = Constants.SecondaryColor;
		private Color _closeColor = Color.Red;
		private Color _borderColor = Constants.BorderColor;
		private bool _stretchCaptions = false;
		private bool _canResize = true;
		private bool _animateWindow = false;
		private bool _showCaptionBox = true;
		private bool _useAeroSnap = true;
		private bool _useAeroShadow = true;
		private bool _useUserSchemeCursor = true;
		private Cursor _originalCrsr = Cursors.Default;

		private bool _aeroEnabled = false;
		private bool _canFade = true;
		private Size _stateChangeSize;

		private Rectangle _topLeft, _top, _topRight,
			_left, _right,
			_bottomLeft, _bottom, _bottomRight,
			_header, _close,
			_caption1, _caption2;
		private Color _caption1Color, _caption2Color;
		private Bitmap _caption1Content, _caption2Content;
		private bool _allowCaption1, _allowCaption2;
		private EventHandler _caption1Click, _caption2Click;

		private const int _headerHeight = 32, _resizeWidth = 6;

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
		/// Occurs when the value of the <see cref="MinimizeColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the MinimizeColor property changes.")]
		public event EventHandler MinimizeColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="MaximizeColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the MaximizeColor property changes.")]
		public event EventHandler MaximizeColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Raises the <see cref="HeaderColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnHeaderColorChanged(EventArgs e)
		{
			HeaderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="CloseColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnCloseColorChanged(EventArgs e)
		{
			CloseColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="MinimizeColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnMinimizeColorChanged(EventArgs e)
		{
			MinimizeColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="MaximizeColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnMaximizeColorChanged(EventArgs e)
		{
			MaximizeColorChanged?.Invoke(this, e);
			Invalidate();
		}

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
		/// Gets or sets the background color of the form header.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the form header.")]
		public Color HeaderColor
		{
			get => _headerColor;
			set { _headerColor = value; OnHeaderColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the color of the close button on the form header.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the color of the close button on the form header.")]
		public Color CloseColor
		{
			get => _closeColor;
			set { _closeColor = value; OnCloseColorChanged(EventArgs.Empty); }
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
		/// Gets or sets a value indicating whether the caption icons on the form header will be stretched to their caption boundaries.<br/><br/>
		/// 
		/// <b>Note:</b> <i>This is just for fun to make a slight alteration to the form header captions.</i>
		/// </summary>
		[Category("Appearance"), Description("Gets or sets a value indicating whether the caption icons on the form header will be stretched to their caption boundaries.\n\nNote: This is just for fun to make a slight alteration to the form header captions.")]
		public bool StretchCaptions
		{
			get => _stretchCaptions;
			set { _stretchCaptions = value; Invalidate(); }
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
			set
			{
				_animateWindow = value;
				if (value) _useAeroSnap = false;

				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether button boxes will be displayed when the mouse pointer is hovered over the caption buttons.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets a value indicating whether button boxes will be displayed when the mouse pointer is hovered over the caption buttons.")]
		public bool ShowCaptionBox
		{
			get => _showCaptionBox;
			set { _showCaptionBox = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the form will utilize Windows Aero snapping.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the form will utilize Windows Aero snapping.")]
		public bool UseAeroSnap
		{
			get => _useAeroSnap;
			set
			{
				_useAeroSnap = value;
				if (value) _animateWindow = false;
				FormBorderStyle = value ? (_canResize ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle) : FormBorderStyle.None;

				Invalidate();
			}
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
		/// Gets or sets the cursor that is displayed when the mouse pointer is over the form.
		/// </summary>
		/// <returns>
		/// A <see cref="Cursor"/> representing the cursor that is displayed when the mouse pointer is over the form.
		/// </returns>
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

		public NovaToolWindow()
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

		public void AddCustomCaption(Bitmap Content, Color HoverColor, EventHandler ClickEvent)
		{
			if (Content == null)
				throw new ArgumentNullException("Content");
			if (HoverColor == null || HoverColor == Color.Empty)
				throw new ArgumentNullException("HoverColor");
			if (ClickEvent == null)
				throw new ArgumentNullException("ClickEvent");

			if (!_allowCaption1)
			{
				_allowCaption1 = true;
				_caption1Color = HoverColor;
				_caption1Content = Content;
				_caption1Click = ClickEvent;
			}

			if (!_allowCaption2)
			{
				_allowCaption2 = true;
				_caption2Color = HoverColor;
				_caption2Content = Content;
				_caption2Click = ClickEvent;
			}

			if (_allowCaption1 && _allowCaption2)
				throw new InvalidOperationException("Cannot add another caption to the form header because only 2 custom captions at maximum are supported.");
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if ((_left.Contains(e.Location) || _right.Contains(e.Location)) && _canResize)
			{
				if (_useUserSchemeCursor) Cursor = Win32.RegCursor("SizeWE");
				else Cursor = Cursors.SizeWE;
			}
			else if ((_top.Contains(e.Location) || _bottom.Contains(e.Location)) && _canResize)
			{
				if (_useUserSchemeCursor) Cursor = Win32.RegCursor("SizeNS");
				else Cursor = Cursors.SizeNS;
			}
			else if ((_topLeft.Contains(e.Location) || _bottomRight.Contains(e.Location)) && _canResize)
			{
				if (_useUserSchemeCursor) Cursor = Win32.RegCursor("SizeNWSE");
				else Cursor = Cursors.SizeNWSE;
			}
			else if ((_topRight.Contains(e.Location) || _bottomLeft.Contains(e.Location)) && _canResize)
			{
				if (_useUserSchemeCursor) Cursor = Win32.RegCursor("SizeNESW");
				else Cursor = Cursors.SizeNESW;
			}
			else if (_close.Contains(e.Location) ||
				(_caption1.Contains(e.Location) && _allowCaption1) ||
				(_caption2.Contains(e.Location) && _allowCaption2))
			{
				if (_useUserSchemeCursor) Cursor = Win32.RegCursor("Hand");
				else Cursor = Cursors.Hand;
			}
			else
			{
				if (_useUserSchemeCursor) Cursor = Win32.RegCursor("Arrow");
				else Cursor = _originalCrsr;
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
			else if (_close.Contains(e.Location))
			{
				if (_animateWindow)
					Fade(false, Close);
				else Close();
			}
			else if (_allowCaption1 && _caption1.Contains(e.Location)) _caption1Click(this, e);
			else if (_allowCaption2 && _caption2.Contains(e.Location)) _caption2Click(this, e);
			else if (_header.Contains(e.Location))
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Handle, 161, 2, 0);
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

			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			_header = new Rectangle(0, 0, Width, _headerHeight);

			if (!_useAeroShadow)
			{
				e.Graphics.FillRectangle(_borderColor.ToBrush(), 0, 0, Width, Height);
				e.Graphics.FillRectangle(BackColor.ToBrush(), 1, 1, Width - 2, Height - 2);
				e.Graphics.FillRectangle(_headerColor.ToBrush(), _header.X + 1, _header.Y + 1, _header.Width - 2, _header.Height - 2);
			}
			else if (_useAeroShadow)
			{
				e.Graphics.FillRectangle(BackColor.ToBrush(), 0, 0, Width, Height);
				e.Graphics.FillRectangle(_headerColor.ToBrush(), _header);
			}

			_topLeft = new Rectangle(0, 0, _resizeWidth, _resizeWidth);
			_top = new Rectangle(_resizeWidth, 0, Width - (_resizeWidth * 2), _resizeWidth);
			_topRight = new Rectangle(Width - _resizeWidth, 0, _resizeWidth, _resizeWidth);
			_left = new Rectangle(0, _resizeWidth, _resizeWidth, Height - (_resizeWidth * 2));
			_right = new Rectangle(Width - _resizeWidth, _resizeWidth, _resizeWidth, Height - (_resizeWidth * 2));
			_bottomLeft = new Rectangle(0, Height - _resizeWidth, _resizeWidth, _resizeWidth);
			_bottom = new Rectangle(_resizeWidth, Height - _resizeWidth, Width - (_resizeWidth * 2), _resizeWidth);
			_bottomRight = new Rectangle(Width - _resizeWidth, Height - _resizeWidth, _resizeWidth, _resizeWidth);

			Rectangle text = new Rectangle(_resizeWidth, _resizeWidth, Width - (_resizeWidth * 2), _headerHeight - (_resizeWidth * 2));
			if (ShowIcon)
			{
				text.X += _headerHeight - (_resizeWidth * 2) + 2;
				text.Width -= _headerHeight - (_resizeWidth * 2) + 2;
			}
			if (_allowCaption1) text.Width -= _headerHeight;
			if (_allowCaption2) text.Width -= _headerHeight;
			if (_useAeroShadow) text.Y += 1;

			if (ShowIcon)
			{
				SmoothingMode smode = e.Graphics.SmoothingMode;
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				e.Graphics.DrawIcon(Icon, new Rectangle(_resizeWidth, _resizeWidth + (_useAeroShadow ? 1 : 0), _headerHeight - (_resizeWidth * 2), _headerHeight - (_resizeWidth * 2)));
				e.Graphics.SmoothingMode = smode;
			}

			e.Graphics.DrawString(Text, Font, ForeColor.ToBrush(), text, Constants.LeftAlign);

			// for visual debugging of the Form layout
			bool debug = false;

			_close = new Rectangle(Width - _headerHeight - _resizeWidth, _resizeWidth, _headerHeight, _headerHeight - (_resizeWidth * 2));

			Size captionIcon = new Size(_headerHeight - (_resizeWidth * 2), _headerHeight - (_resizeWidth * 2));
			Point mouse = PointToClient(MousePosition);

			if (_showCaptionBox) e.Graphics.FillRectangle(_headerColor.Lighter(_close.Contains(mouse) ? 0.1f : 0).ToBrush(), _close.X, _close.Y - _resizeWidth + 1, _headerHeight, _headerHeight - (_useAeroShadow ? 1 : 2));
			if (debug)
			{
				e.Graphics.FillRectangle(_closeColor.ToBrush(), _close);
				e.Graphics.FillRectangle(_closeColor.Lighter(0.5f).ToBrush(),
					new Rectangle(_close.X + ((_close.Width - captionIcon.Width) / 2), _close.Y, captionIcon.Width, captionIcon.Height));
			}
			e.Graphics.DrawImage(Bitmaps.Instance[_headerHeight, _close.Contains(mouse) ? _closeColor : ForeColor, BitmapType.Close],
				new Rectangle(_close.X + (!_stretchCaptions ? ((_close.Width - captionIcon.Width) / 2) : 0), _close.Y, !_stretchCaptions ? captionIcon.Width : _close.Width, !_stretchCaptions ? captionIcon.Height : _close.Height)
				.Rescale(-_resizeWidth, _resizeWidth * 2));

			if (_allowCaption1)
			{
				_caption1 = new Rectangle(_close.X - _headerHeight, 0, _headerHeight, _headerHeight - (_resizeWidth * 2));

				ImageAttributes attributes = new ImageAttributes();
				attributes.SetRemapTable(new ColorMap[] { new ColorMap
					{
						OldColor = ForeColor,
						NewColor = _caption1.Contains(mouse) ? _caption1Color : ForeColor
					} });

				if (_showCaptionBox) e.Graphics.FillRectangle(_headerColor.Lighter(_caption1.Contains(mouse) ? 0.1f : 0).ToBrush(), _caption1.X, _caption1.Y - _resizeWidth + 1, _headerHeight, _headerHeight - (_useAeroShadow ? 1 : 2));
				e.Graphics.DrawImage(_caption1Content,
					new Rectangle(_caption1.X + (!_stretchCaptions ? ((_caption1.Width - captionIcon.Width) / 2) : 0), _caption1.Y, !_stretchCaptions ? captionIcon.Width : _caption1.Width, !_stretchCaptions ? captionIcon.Height : _caption1.Height)
					.Rescale(-_resizeWidth, _resizeWidth * 2).Rescale(2, -4));
			}

			if (_allowCaption2)
			{
				_caption2 = new Rectangle(_caption1.X - _headerHeight, 0, _headerHeight, _headerHeight - (_resizeWidth * 2));

				ImageAttributes attributes = new ImageAttributes();
				attributes.SetRemapTable(new ColorMap[] {
					new ColorMap
					{
						OldColor = ForeColor,
						NewColor = _caption2.Contains(mouse) ? _caption2Color : ForeColor
					} });

				if (_showCaptionBox) e.Graphics.FillRectangle(_headerColor.Lighter(_caption2.Contains(mouse) ? 0.1f : 0).ToBrush(), _caption2.X, _caption2.Y - _resizeWidth + 1, _headerHeight, _headerHeight - (_useAeroShadow ? 1 : 2));
				e.Graphics.DrawImage(_caption2Content,
					new Rectangle(_caption2.X + (!_stretchCaptions ? ((_caption2.Width - captionIcon.Width) / 2) : 0), _caption2.Y, !_stretchCaptions ? captionIcon.Width : _caption2.Width, !_stretchCaptions ? captionIcon.Height : _caption2.Height)
					.Rescale(-_resizeWidth, _resizeWidth * 2).Rescale(2, -4));
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
