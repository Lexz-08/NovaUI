using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(RichTextBox))]
	[DefaultEvent("TextChanged")]
	public class NovaRichInput : Control
	{
		private Color _borderColor = Constants.BorderColor;
		private Color _activeColor = Constants.AccentColor;
		private int _borderWidth = 1;
		private int _borderRadius = 6;
		private bool _underlineBorder = false;
		private bool _useUserSchemeCursor = true;
		private Cursor _originalCrsr = Cursors.IBeam;

		private RichTextBox _input = new RichTextBox();
		private bool _mouseHover = false;
		private Stopwatch _delay = new Stopwatch();
		private Timer _delayTracker = new Timer { Interval = 1 };
		private bool _canDelay = true;

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
		/// Occurs when the user begins input.
		/// </summary>
		[Category("Behavior"), Description("Occurs when the user begins input.")]
		public event EventHandler InputStarted;

		/// <summary>
		/// Occurs when the user stops input.
		/// </summary>
		[Category("Behavior"), Description("Occurs when the user stops input.")]
		public event EventHandler InputEnded;

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
		/// Raises the <see cref="InputStarted"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnInputStarted(EventArgs e)
		{
			InputStarted?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="InputEnded"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnInputEnded(EventArgs e)
		{
			InputEnded?.Invoke(this, e);
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
		/// Gets or sets the border color of the control when it is selected.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border color of the control when it is selected.")]
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
				_borderWidth = value;
				UpdateInputBounds();
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
				_borderRadius = value;
				UpdateInputBounds();
				OnBorderRadiusChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control will display a line under the input area of the control or a full border.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets a value indicating whether the control will display a line under the input area of the control or a full border.")]
		public bool UnderlineBorder
		{
			get => _underlineBorder;
			set { _underlineBorder = value; Invalidate(); }
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
		/// Gets or sets the text associated with this control.
		/// </summary>
		/// <returns>
		/// The text associated with this control.
		/// </returns>
		[Category("Appearance"), Description("Gets or sets the text associated with this control.")]
		public override string Text
		{
			get => _input.Text;
			set { _input.Text = value; OnTextChanged(EventArgs.Empty); _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether text in the control is read-only.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the control is read-only; otherwise <see langword="false"/>. The default is <see langword="false"/>.
		/// </returns>
		[Category("Behavior"), Description("Gets or sets a value indicating whether text in the control is read-only.")]
		public bool ReadOnly
		{
			get => _input.ReadOnly;
			set { _input.ReadOnly = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the maximum number of characters the user can type or paste into the control.
		/// </summary>
		/// <returns>
		/// The number of characters that can be entered into the control. The default is <see cref="int.MaxValue"/>.
		/// </returns>
		[Category("Behavior"), Description("Gets or sets the maximum number of characters the user can type or paste into the control.")]
		public int MaxLength
		{
			get => _input.MaxLength;
			set { _input.MaxLength = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the text of the control, including all rich text format (RTF) codes.
		/// </summary>
		/// <returns>
		/// The text of the control if RTF format.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string Rtf
		{
			get => _input.Rtf;
			set { _input.Rtf = value; OnTextChanged(EventArgs.Empty); _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the type of scroll bars to display in the control.
		/// </summary>
		/// <returns>
		/// One of the <see cref="RichTextBoxScrollBars"/> values. The default is <see cref="RichTextBoxScrollBars.Both"/>.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public RichTextBoxScrollBars ScrollBars
		{
			get => _input.ScrollBars;
			set { _input.ScrollBars = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the currently selected rich text format (RTF) formatted text in the control.
		/// </summary>
		/// <returns>
		/// The selected RTF text in the control.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string SelectedRtf
		{
			get => _input.SelectedRtf;
			set { _input.SelectedRtf = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the alignment to apply to the current selection or insertion point.
		/// </summary>
		/// <returns>
		/// One of the <see cref="HorizontalAlignment"/> values.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public HorizontalAlignment SelectionAlignment
		{
			get => _input.SelectionAlignment;
			set { _input.SelectionAlignment = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the starting point of text selected in the control.
		/// </summary>
		/// <returns>
		/// The starting point of text selected in the control.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int SelectionStart
		{
			get => _input.SelectionStart;
			set { _input.SelectionStart = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the color of the text when the text is selected in a control.
		/// </summary>
		/// <returns>
		/// A <see cref="Color"/> that represents the text color when the text is selected. The default is the value of the <see cref="Control.DefaultBackColor"/> property.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Color SelectionBackColor
		{
			get => _input.SelectionBackColor;
			set { _input.SelectionBackColor = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets whether text in the control appears on the baseline, as a superscript, or as a subscript below the baseline.
		/// </summary>
		/// <returns>
		/// A number that specifies the character offset.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int SelectionCharOffset
		{
			get => _input.SelectionCharOffset;
			set { _input.SelectionCharOffset = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the text color of the current text selection or insertion point.
		/// </summary>
		/// <returns>
		/// A <see cref="Color"/> that represents the color to apply to the current text selection or to text entered after the insertion point.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Color SelectionColor
		{
			get => _input.SelectionColor;
			set { _input.SelectionColor = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the font of the current text selection or insertion point.
		/// </summary>
		/// <returns>
		/// A <see cref="Font"/> the represents the font to apply to the current text selection or to text entered after the insertion point.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Font SelectionFont
		{
			get => _input.SelectionFont;
			set { _input.SelectionFont = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the distance between the left edge of the first line of text in the selected paragraph and the left edge of subsequent lines in the same paragraph.
		/// </summary>
		/// <returns>
		/// The distance, in pixels, for the hanging indent applied to the current text selection or the insertion point.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int SelectionHangingIndent
		{
			get => _input.SelectionHangingIndent;
			set { _input.SelectionHangingIndent = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the length, in pixels, of the indentation of the line where the selection starts.
		/// </summary>
		/// <returns>
		/// The current distance, in pixels, of the indentation applied to the left of the current text selection or the insertion point.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int SelectionIndent
		{
			get => _input.SelectionIndent;
			set { _input.SelectionIndent = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the number of characters selected in the control.
		/// </summary>
		/// <returns>
		/// The number of characters selected in the control.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int SelectionLength
		{
			get => _input.SelectionLength;
			set { _input.SelectionLength = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the current text selection is protected.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the current selection prevents any changes to its content; otherwise, <see langword="false"/>. The default is <see langword="false"/>.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public bool SelectionProtected
		{
			get => _input.SelectionProtected;
			set { _input.SelectionProtected = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// The distance (in pixels) between the right edge of the control and the right edge of the text that is selected or added at the current insertion point.
		/// </summary>
		/// <returns>
		/// The indentation space, in pixels, at the right of the current selection or insertion point.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int SelectionRightIndent
		{
			get => _input.SelectionIndent;
			set { _input.SelectionIndent = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the absolute tab stop positions in a control.
		/// </summary>
		/// <returns>
		/// An array in which each member specifies a tab offset, in pixels.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int[] SelectionTabs
		{
			get => _input.SelectionTabs;
			set { _input.SelectionTabs = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets the selection type within the control.
		/// </summary>
		/// <returns>
		/// A bitwise combination of the <see cref="RichTextBoxSelectionTypes"/> values.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public RichTextBoxSelectionTypes SelectionType => _input.SelectionType;

		/// <summary>
		/// Gets the length of text in the control.
		/// </summary>
		/// <returns>
		/// The number of characters contained in the text of the control.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int TextLength => _input.TextLength;

		/// <summary>
		/// Gets or sets the current zoom level of the control.
		/// </summary>
		/// <returns>
		/// The factor by which the contents of the control is zoomed.
		/// </returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public float ZoomFactor
		{
			get => _input.ZoomFactor;
			set { _input.ZoomFactor = value; _input.Invalidate(); Invalidate(); }
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
			get => _input.Cursor;
			set
			{
				_input.Cursor = value;
				base.Cursor = value;
				if (!_useUserSchemeCursor) _originalCrsr = value;

				OnCursorChanged(EventArgs.Empty);
				_input.Invalidate();
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the background color of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the control.")]
		public override Color BackColor
		{
			get => _input.BackColor;
			set { _input.BackColor = value; OnBackColorChanged(EventArgs.Empty); _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the foreground color of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the foreground color of the control.")]
		public override Color ForeColor
		{
			get => _input.ForeColor;
			set { _input.ForeColor = value; OnForeColorChanged(EventArgs.Empty); _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the font of the text displayed by the control.
		/// </summary>
		/// <returns>
		/// The <see cref="Font"/> to apply to the text displayed by the control. The default is the value of the <see cref="Control.DefaultFont"/> property.
		/// </returns>
		[Category("Appearance"), Description("Gets or sets the font of the text displayed by the control.")]
		public override Font Font
		{
			get => _input.Font;
			set { _input.Font = value; OnFontChanged(EventArgs.Empty); _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets a value indicating whether the control has input focus.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the control has focus; otherwise, <see langword="false."/>
		/// </returns>
		public override bool Focused => _input.Focused;

		public NovaRichInput()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;
			Width = 200;
			Height = 150;

			_input.Multiline = true;
			_input.BorderStyle = BorderStyle.None;
			_input.Location = new Point(8, 8);
			_input.Width = Width - 16;
			_input.Height = Height - 16;
			_input.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

			_input.GotFocus += (_, __) => { _input.Invalidate(); Invalidate(); };
			_input.LostFocus += (_, e) => OnLostFocus(e);

			_input.TextChanged += (_, e) => OnTextChanged(e);
			_input.KeyDown += (_, e) => OnKeyDown(e);
			_input.KeyPress += (_, e) => OnKeyPress(e);
			_input.KeyUp += (_, e) => OnKeyUp(e);

			_input.MouseEnter += (_, e) => OnMouseEnter(e);
			_input.MouseLeave += (_, e) => OnMouseLeave(e);

			_input.Click += (_, e) => OnClick(e);
			_input.MouseClick += (_, e) => OnMouseClick(e);

			_delayTracker.Tick += (_, __) =>
			{
				if (_delay.ElapsedMilliseconds > 250)
				{
					OnInputEnded(EventArgs.Empty);
					_delayTracker.Stop();
					_delay.Stop();
					_canDelay = true;
				}
			};

			Controls.Add(_input);
		}

		private void UpdateHeight()
		{
			//if (Height < _input.Height + 16) Height = _input.Height + 16;
			if (_input.Multiline) _input.Height = Height - 16;
		}

		private void UpdateInputBounds()
		{
			_input.Location = new Point(6 + (_borderRadius == 0 ? 0 : _borderRadius / (_underlineBorder ? 2 : 4)) + _borderWidth, 8 + _borderWidth);
			_input.Width = Width - 16 - (2 * (_borderRadius == 0 ? 0 : _borderRadius / (_underlineBorder ? 2 : 4))) - (_borderWidth * 2);
			_input.Height = Height - 16 - (2 * (_borderRadius == 0 ? 0 : _borderRadius / (_underlineBorder ? 2 : 4))) - (_borderWidth * 2);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (_canDelay)
			{
				OnInputStarted(EventArgs.Empty);
				_canDelay = false;
			}
			else
			{
				_delay.Restart();
				_delayTracker.Stop();
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			_delay.Start();
			_delayTracker.Start();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			UpdateHeight();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			UpdateHeight();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);

			_mouseHover = true;
			if (_useUserSchemeCursor) Cursor = Win32.RegCursor("IBeam");
			else Cursor = _originalCrsr;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			_mouseHover = false;
			Invalidate();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			_input.Focus();
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);

			Invalidate();
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);

			_input.Focus();
			Invalidate();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			_input.Focus();
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			_input.Focus();
			Invalidate();
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
			_input.Invalidate();
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.Clear(Parent.BackColor);
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, _borderRadius, _borderRadius));

			if (_underlineBorder)
			{
				if (_borderRadius > 0)
				{
					e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					for (int i = 0; i < Math.Max(1, _borderWidth); i++)
						e.Graphics.DrawPath(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
							new Rectangle(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1).Roundify(_borderRadius - i));
					e.Graphics.FillPath(BackColor.ToBrush(),
						new Rectangle(0, 0, Width - 1, Height - Math.Max(1, _borderWidth) - 1).Roundify(_borderRadius));
					e.Graphics.DrawPath(new Pen(BackColor.ToBrush()),
						new Rectangle(0, 0, Width - 1, Height - Math.Max(1, _borderWidth) - 1).Roundify(_borderRadius));
				}
				else
				{
					e.Graphics.FillRectangle(BackColor.ToBrush(),
						new Rectangle(0, 0, Width, Height - Math.Max(1, _borderWidth)));
					e.Graphics.DrawRectangle(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
						new Rectangle(0, Height - 1, Width, Math.Max(1, _borderWidth)));
				}
			}
			else
			{
				if (_borderRadius > 0)
				{
					e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					if (_borderWidth > 0)
					{
						e.Graphics.FillPath(BackColor.ToBrush(),
							new Rectangle(_borderWidth - 1, _borderWidth - 1, Width - (_borderWidth * 2) + 1, Height - (_borderWidth * 2) + 1).Roundify(Math.Max(1, _borderRadius - _borderWidth)));
						for (int i = 0; i < _borderWidth; i++)
							e.Graphics.DrawPath(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
								new Rectangle(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1).Roundify(_borderRadius - i));
					}
					else
					{
						e.Graphics.FillPath(BackColor.ToBrush(),
							new Rectangle(0, 0, Width - 1, Height - 1).Roundify(_borderRadius));
						e.Graphics.DrawPath(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
							new Rectangle(0, 0, Width - 1, Height - 1).Roundify(_borderRadius));
					}
				}
				else
				{
					if (_borderWidth > 0)
					{
						e.Graphics.FillRectangle(BackColor.ToBrush(),
							new Rectangle(_borderWidth, _borderWidth, Width - (_borderWidth * 2), Height - (_borderWidth * 2)));
						for (int i = 0; i < _borderWidth; i++)
							e.Graphics.DrawRectangle(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
								new Rectangle(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1));
					}
					else
					{
						e.Graphics.FillRectangle(BackColor.ToBrush(),
							new Rectangle(1, 1, Width - 2, Height - 2));
						e.Graphics.DrawRectangle(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
							new Rectangle(0, 0, Width - 1, Height - 1));
					}
				}
			}
		}
	}
}
