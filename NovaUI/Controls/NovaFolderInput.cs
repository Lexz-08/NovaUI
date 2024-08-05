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
		private Color _borderColor = Constants.BorderColor;
		private Color _activeColor = Constants.AccentColor;
		private int _borderWidth = 1;
		private int _borderRadius = 6;
		private bool _underlineBorder = false;
		private bool _useUserSchemeCursor = true;
		private string _dialogDescription = "Please select a folder...";
		private bool _allowNewFolder = false;
		private Cursor _originalCrsr = Cursors.IBeam;

		private TextBox _input = new TextBox();
		private bool _mouseHover = false;

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
		/// Occurs when a folder is selected.
		/// </summary>
		[Category("Property"), Description("Occurs when a folder is selected.")]
		public event FolderSelectedEventHandler FolderSelected;

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
		/// Raises the <see cref="FolderSelected"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
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
				_borderWidth = Math.Max(1, value);
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
				if (value != _borderRadius)
					Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, _borderRadius, _borderRadius));
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
		/// Gets or sets the description of the folder dialog.
		/// </summary>
		[Category("Folder Dialog"), Description("Gets or sets the description of the folder dialog.")]
		public string DialogDescription
		{
			get => _dialogDescription;
			set { _dialogDescription = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the folder dialog will allow new folders to be created.
		/// </summary>
		[Category("Folder Dialog"), Description("Gets or sets a value indicating whether the folder dialog will allow new folders to be created.")]
		public bool AllowNewFolder
		{
			get => _allowNewFolder;
			set { _allowNewFolder = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control modifies the case of characters as they are typed.
		/// </summary>
		/// <returns>
		/// One of the <see cref="CharacterCasing"/> enumeration values that specifies whether the control modifies the case of characters. The default is <see cref="CharacterCasing.Normal"/>.
		/// </returns>
		[Category("Behavior"), Description("Gets or sets a value indicating whether the control modifies the case of characters as they are typed.")]
		public CharacterCasing CharacterCasing
		{
			get => _input.CharacterCasing;
			set { _input.CharacterCasing = value; _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		/// <returns>
		/// The text associated with this control.
		/// </returns>
		[Category("Appearance"), Description("Gets or sets the text associated with this control.")]
		public string Folder
		{
			get => _input.Text;
			set { _input.Text = value; OnTextChanged(EventArgs.Empty); _input.Invalidate(); Invalidate(); }
		}

		/// <summary>
		/// Gets or sets how text is aligned in a control.
		/// </summary>
		/// <returns>
		/// One of the <see cref="HorizontalAlignment"/> enumeration values that specifies how text is aligned in the control. The default is <see cref="HorizontalAlignment.Left"/>.
		/// </returns>
		[Category("Behavior"), Description("Gets or sets how text is aligned in a control.")]
		public HorizontalAlignment TextAlign
		{
			get => _input.TextAlign;
			set { _input.TextAlign = value; _input.Invalidate(); Invalidate(); }
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
		/// Gets the file extension of the currently file path.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string Extension => Path.GetExtension(_input.Text);

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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text => string.Empty;

		/// <summary>
		/// Gets a value indicating whether the control has input focus.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the control has focus; otherwise, <see langword="false."/>
		/// </returns>
		public override bool Focused => _input.Focused;

		public NovaFolderInput()
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
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, _borderRadius, _borderRadius));

			_input.ReadOnly = true;
			_input.BorderStyle = BorderStyle.None;
			_input.Location = new Point(8, 8);
			_input.Width = Width - 16;
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

			Controls.Add(_input);

			UpdateHeight();
			Invalidate();
		}

		private void UpdateHeight()
		{
			if (Height < _input.Height + 16 || Height > _input.Height + 16) Height = _input.Height + 16;
			if (!_input.Multiline) Height = _input.Height + 16;
		}

		private void UpdateInputBounds()
		{
			_input.Location = new Point(8 + (_borderRadius == 0 ? 0 : _borderRadius / (_underlineBorder ? 2 : 4)) + _borderWidth, _input.Location.Y);
			_input.Width = Width - 16 - (2 * (_borderRadius == 0 ? 0 : _borderRadius / (_underlineBorder ? 2 : 4))) - (_borderWidth * 2);
		}

		/// <summary>
		/// Clears the current file path.
		/// </summary>
		public void ClearFile() => _input.Clear();

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

			using (FolderBrowserDialog fbd = new FolderBrowserDialog
			{
				Description = _dialogDescription,
				ShowNewFolderButton = _allowNewFolder
			}) if (fbd.ShowDialog() == DialogResult.OK)
				{
					string old = _input.Text;
					_input.Text = fbd.SelectedPath;

					if (!_input.Text.Equals(old)) OnFolderSelected(new FolderSelectedEventArgs(_input.Text));
				}

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

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			UpdateHeight();
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

			if (_underlineBorder)
			{
				if (_borderRadius > 0)
				{
					e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					for (int i = 0; i < _borderWidth; i++)
						e.Graphics.DrawPath(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
							new Rectangle(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1).Roundify(_borderRadius - i));
					e.Graphics.FillPath(BackColor.ToBrush(),
						new Rectangle(0, 0, Width - 1, Height - _borderWidth - 1).Roundify(_borderRadius));
					e.Graphics.DrawPath(new Pen(BackColor.ToBrush()),
						new Rectangle(0, 0, Width - 1, Height - _borderWidth - 1).Roundify(_borderRadius));
				}
				else
				{
					e.Graphics.FillRectangle(BackColor.ToBrush(),
						new Rectangle(0, 0, Width, Height - _borderWidth));
					e.Graphics.DrawRectangle(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
						new Rectangle(0, Height - 1, Width, _borderWidth));
				}
			}
			else
			{
				if (_borderRadius > 0)
				{
					e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					e.Graphics.FillPath(BackColor.ToBrush(),
						new Rectangle(_borderWidth - 1, _borderWidth - 1, Width - (_borderWidth * 2) + 1, Height - (_borderWidth * 2) + 1).Roundify(Math.Max(1, _borderRadius - _borderWidth)));
					for (int i = 0; i < _borderWidth; i++)
						e.Graphics.DrawPath(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
							new Rectangle(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1).Roundify(_borderRadius - i));
				}
				else
				{
					e.Graphics.FillRectangle(BackColor.ToBrush(),
						new Rectangle(_borderWidth, _borderWidth, Width - (_borderWidth * 2), Height - (_borderWidth * 2)));
					for (int i = 0; i < _borderWidth; i++)
						e.Graphics.DrawRectangle(new Pen((Focused ? _activeColor : _borderColor.Lighter(_mouseHover ? 0.1f : 0)).ToBrush()),
							new Rectangle(i, i, Width - (i * 2) - 1, Height - (i * 2) - 1));
				}
			}
		}
	}
}
