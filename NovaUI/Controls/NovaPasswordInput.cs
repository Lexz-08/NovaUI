using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.Designers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(TextBox))]
	[DefaultEvent("TextChanged")]
	[Designer(typeof(LengthResizeDesigner))]
	public class NovaPasswordInput : Control
	{
		private readonly Pen borderFocusedPen = Color.Transparent.ToPen();
		private readonly Pen borderHoverPen = Color.Transparent.ToPen();
		private readonly Pen borderNormalPen = Color.Transparent.ToPen();
		private readonly SolidBrush backBrush = Color.Transparent.ToBrush();
		private readonly Pen backPen = Color.Transparent.ToPen();

		private Color borderColor = Constants.BorderColor;
		private Color activeColor = Constants.AccentColor;
		private int borderWidth = 1;
		private int borderRadius = 6;
		private bool underlineBorder = false;
		private bool preventEditOnShow = true;
		private bool useUserSchemeCursor = true;
		private CharacterCasing casing = CharacterCasing.Normal;
		private HorizontalAlignment align = HorizontalAlignment.Left;
		private char passChar = '•';
		private Cursor originalCursor = Cursors.IBeam;
		private char passwordChar = '•';

		private readonly TextBox input = new TextBox();
		private bool mouseHover = false;
		private readonly Stopwatch delay = new Stopwatch();
		private readonly Timer delayTracker = new Timer() { Interval = 1 };
		private bool canDelay = true;

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

		protected virtual void OnBorderColorChanged(EventArgs e)
		{
			BorderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnActiveColorChanged(EventArgs e)
		{

			ActiveColorChanged?.Invoke(this, e);
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

		protected virtual void OnInputStarted(EventArgs e)
		{

			InputStarted?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnInputEnded(EventArgs e)
		{

			InputEnded?.Invoke(this, e);
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
				if (borderNormalPen.Color != value)
				{
					borderNormalPen.Color = value;
					borderHoverPen.Color = value.Lighter(0.1f);
				}
				OnBorderColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the border color of the control when it is selected.")]
		public Color ActiveColor
		{
			get => activeColor;
			set
			{
				activeColor = value;
				if (borderFocusedPen.Color != value) borderFocusedPen.Color = value;
				OnActiveColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the border width of the control.")]
		public int BorderWidth
		{
			get => borderWidth;
			set
			{
				borderWidth = Math.Max(1, value);
				if (borderNormalPen.Width != value)
				{
					borderNormalPen.Width = value;
					borderHoverPen.Width = value;
					borderFocusedPen.Width = value;
				}
				UpdateInputBounds();
				OnBorderWidthChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the border radius of the control.")]
		public int BorderRadius
		{
			get => borderRadius;
			set
			{
				if (value < 0) value = 0;
				else if (value > Math.Min(Width, Height) / 2)
					value = Math.Min(Width, Height) / 2;
				if (value != borderRadius)
					Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));
				borderRadius = value;
				UpdateInputBounds();
				OnBorderRadiusChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets a value indicating whether the control will display a line under the input area of the control or a full border.")]
		public bool UnderlineBorder
		{
			get => underlineBorder;
			set { underlineBorder = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether input is prevented when showing the password.")]
		public bool PreventEditOnShow
		{
			get => preventEditOnShow;
			set
			{
				if (input.PasswordChar == 0) input.ReadOnly = value;
				preventEditOnShow = value;
				Invalidate();
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
		[Category("Behavior"), Description("Gets or sets a value indicating whether the control modifies the case of characters as they are typed.")]
		public CharacterCasing CharacterCasing
		{
			get => casing = input.CharacterCasing;
			set
			{
				casing = value;
				input.CharacterCasing = value;
				input.Invalidate();
				Invalidate();
			}
		}

		[Category("Appearance"), Description("Gets or sets the text associated with this control.")]
		public override string Text
		{
			get => base.Text;
			set
			{
				base.Text = value;
				input.Text = value;
				OnTextChanged(EventArgs.Empty);
				input.Invalidate();
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets how text is aligned in a control.")]
		public HorizontalAlignment TextAlign
		{
			get => align = input.TextAlign;
			set
			{
				align = value;
				input.TextAlign = value;
				input.Invalidate();
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the character used to mask characters of a password in a single-line input control.")]
		public char PasswordChar
		{
			get => passChar = input.PasswordChar;
			set
			{
				if (value == 0) value = '•';
				else passwordChar = value;
				passChar = value;
				input.PasswordChar = value;
				input.Invalidate();
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int SelectionStart
		{
			get => input.SelectionStart;
			set { input.SelectionStart = value; input.Invalidate(); Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int SelectionLength
		{
			get => input.SelectionLength;
			set { input.SelectionLength = value; input.Invalidate(); Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string SelectedText
		{
			get => input.SelectedText;
			set { input.SelectedText = value; input.Invalidate(); Invalidate(); }
		}

		[Category("Apppearance"), Description("Gets or sets the cursor that is displayed when the mouse pointer is over the control.")]
		public override Cursor Cursor
		{
			get => base.Cursor;
			set
			{
				base.Cursor = value;
				input.Cursor = value;
				if (!useUserSchemeCursor) originalCursor = value;

				OnCursorChanged(EventArgs.Empty);
				input.Invalidate();
				Invalidate();
			}
		}

		public override bool Focused => input.Focused;

		public NovaPasswordInput()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			borderFocusedPen = activeColor.ToPen();
			borderHoverPen = borderColor.Lighter(0.1f).ToPen();
			borderNormalPen = borderColor.ToPen();
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;
			Width = 200;
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));

			input.PasswordChar = '•';
			input.BorderStyle = BorderStyle.None;
			input.Location = new Point(8, 8);
			input.Width = Width - 16;
			input.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

			input.GotFocus += (_, __) => { input.Invalidate(); Invalidate(); };
			input.LostFocus += (_, e) => OnLostFocus(e);
			input.TextChanged += (_, e) => { base.Text = input.Text; OnTextChanged(e); };
			input.KeyDown += (_, e) => OnKeyDown(e);
			input.KeyPress += (_, e) => OnKeyPress(e);
			input.KeyUp += (_, e) => OnKeyUp(e);

			input.MouseEnter += (_, e) => OnMouseEnter(e);
			input.MouseLeave += (_, e) => OnMouseLeave(e);

			input.Click += (_, e) => OnClick(e);
			input.MouseClick += (_, e) => OnMouseClick(e);

			delayTracker.Tick += (_, __) =>
			{
				if (delay.ElapsedMilliseconds > 300)
				{
					OnInputEnded(EventArgs.Empty);
					delayTracker.Stop();
					delay.Stop();
					canDelay = true;
				}
			};

			Controls.Add(input);

			UpdateHeight();
			Invalidate();
		}

		private void UpdateHeight()
		{
			if (Height < input.Height + 16 || Height > input.Height + 16) Height = input.Height + 16;
		}

		private void UpdateInputBounds()
		{
			input.Location = new Point(8 + (borderRadius == 0 ? 0 : borderRadius / (underlineBorder ? 2 : 4)) + borderWidth, input.Location.Y);
			input.Width = Width - 16 - (2 * (borderRadius == 0 ? 0 : borderRadius / (underlineBorder ? 2 : 4))) - (borderWidth * 2);
		}

		public void ShowPassword()
		{
			input.PasswordChar = (char)0;
			if (preventEditOnShow) input.ReadOnly = true;
		}

		public void HidePassword()
		{
			input.PasswordChar = passwordChar;
			if (preventEditOnShow) input.ReadOnly = false;
		}

		public void ClearPassword() => input.Clear();

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (canDelay)
			{
				OnInputStarted(EventArgs.Empty);
				canDelay = false;
			}
			else
			{
				delay.Restart();
				delayTracker.Stop();
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			delay.Start();
			delayTracker.Start();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			UpdateHeight();
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			UpdateHeight();
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);

			mouseHover = true;
			if (useUserSchemeCursor) Win32.GetRegistryCursor(Win32.RegistryCursor.IBeam, this);
			else Cursor = originalCursor;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			mouseHover = false;
			Invalidate();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			input.Focus();
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

			input.Focus();
			Invalidate();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			input.Focus();
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			input.Focus();
			Invalidate();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			input.Font = Font;
			UpdateHeight();
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
			input.Invalidate();
			Invalidate();
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);
			if (backBrush.Color != BackColor)
			{
				backBrush.Color = BackColor;
				backPen.Color = BackColor;
			}
			input.BackColor = BackColor;
			input.Invalidate();
			Invalidate();
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			base.OnForeColorChanged(e);
			input.ForeColor = ForeColor;
			input.Invalidate();
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.Clear(Parent != null ? Parent.BackColor : Color.Transparent);

			if (underlineBorder)
			{
				e.Graphics.FillRectangle(backBrush,
					new Rectangle(0, 0, Width, Height - borderWidth));
				e.Graphics.DrawRectangle(Focused ? borderFocusedPen : (mouseHover ? borderHoverPen : borderNormalPen),
					new Rectangle(0, Height - borderWidth, Width, borderWidth));
			}
			else
			{
				if (borderRadius > 0)
				{
					e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					e.Graphics.FillPath(backBrush,
						new Rectangle(borderWidth - 1, borderWidth - 1, Width - (borderWidth * 2) + 1, Height - (borderWidth * 2) + 1).Round(Math.Max(1, borderRadius - borderWidth)));
					e.Graphics.DrawPath(Focused ? borderFocusedPen : (mouseHover ? borderHoverPen : borderNormalPen),
						new RectangleF(borderWidth / 2f - 0.5f, borderWidth / 2f - 0.5f, Width - borderWidth, Height - borderWidth).Round(borderRadius));
				}
				else
				{
					e.Graphics.FillRectangle(backBrush,
						new Rectangle(borderWidth, borderWidth, Width - (borderWidth * 2), Height - (borderWidth * 2)));
					e.Graphics.DrawRectangle(Focused ? borderFocusedPen : (mouseHover ? borderHoverPen : borderNormalPen),
						borderWidth / 2f, borderWidth / 2f, Width - borderWidth, Height - borderWidth);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				borderFocusedPen?.Dispose();
				borderHoverPen?.Dispose();
				borderNormalPen?.Dispose();
				backBrush?.Dispose();
				backPen?.Dispose();
			}
		}
	}
}
