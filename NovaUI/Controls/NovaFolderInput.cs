using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.Designers;
using NovaUI.Helpers.LibMain;

using FolderSelectedEventArgs = NovaUI.Events.FolderSelectedEventArgs;
using FolderSelectedEventHandler = NovaUI.Events.FolderSelectedEventHandler;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(TextBox))]
	[DefaultEvent("FolderSelected")]
	[Designer(typeof(LengthResizeDesigner))]
	public class NovaFolderInput : Control
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
		private bool useUserSchemeCursor = true;
		private string dialogDescription = "Please select a folder...";
		private bool allowNewFolder = false;
		private CharacterCasing casing = CharacterCasing.Normal;
		private HorizontalAlignment align = HorizontalAlignment.Left;
		private Cursor originalCursor = Cursors.IBeam;

		private readonly TextBox input = new TextBox();
		private bool mouseHover = false;

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
		/// Occurs when a file is selected.
		/// </summary>
		[Category("Property"), Description("Occurs when a file is selected.")]
		public event FolderSelectedEventHandler FolderSelected;

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

		protected virtual void OnFolderSelected(FolderSelectedEventArgs e)
		{
			FolderSelected?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Gets or sets the border color of the control.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the border color of the control when it is selected.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the border width of the control.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the border radius of the control.
		/// </summary>
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
					Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, value, value));
				borderRadius = value;
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
			get => underlineBorder;
			set { underlineBorder = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control will use the user-selected system scheme cursor.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the control will use the user-selected system scheme cursor.")]
		public bool UseUserSchemeCursor
		{
			get => useUserSchemeCursor;
			set { useUserSchemeCursor = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the description of the folder dialog.
		/// </summary>
		[Category("Folder Dialog"), Description("Gets or sets the description of the folder dialog.")]
		public string DialogDescription
		{
			get => dialogDescription;
			set { dialogDescription = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the folder dialog will allow new folders to be created.
		/// </summary>
		[Category("Folder Dialog"), Description("Gets or sets a value indicating whether the folder dialog will allow new folders to be created.")]
		public bool AllowNewFolder
		{
			get => allowNewFolder;
			set { allowNewFolder = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control modifies the case of characters as they are typed.
		/// </summary>
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string Folder
		{
			get => base.Text;
			set
			{
				base.Text = value;
				input.Text = value;
				input.Invalidate();
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets how text is aligned in a control.
		/// </summary>
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string ParentFolder => Directory.GetParent(input.Text).Name;

		/// <summary>
		/// Gets or sets the cursor that is displayed when the mouse pointer is over the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the cursor that is displayed when the mouse pointer is over the control.")]
		public override Cursor Cursor
		{
			get => base.Cursor;
			set
			{
				base.Cursor = value;
				input.Cursor = value;
				if (!UseUserSchemeCursor) originalCursor = value;

				OnCursorChanged(EventArgs.Empty);
				input.Invalidate();
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text => string.Empty;

		public override bool Focused => input.Focused;

		public NovaFolderInput()
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

			input.ReadOnly = true;
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

		public void ClearFile() => input.Clear();

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
			base.OnGotFocus(e);

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

			using (FolderBrowserDialog fbd = new FolderBrowserDialog()
			{
				Description = dialogDescription,
				ShowNewFolderButton = AllowNewFolder
			}) if (fbd.ShowDialog() == DialogResult.OK)
				{
					string old = input.Text;
					input.Text = fbd.SelectedPath;

					if (!input.Text.Equals(old)) OnFolderSelected(new FolderSelectedEventArgs(input.Text));
				}

			input.Focus();
			Invalidate();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

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
