using NovaUI.Helpers.LibMain;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(LinkLabel))]
	public class NovaLinkLabel : Label
	{
		private string _link = "https://www.google.com/";
		private Color _linkColor = Constants.AccentColor;
		private Color _foreColor = Constants.TextColor;
		private bool _useUserSchemeCursor = true;
		private Cursor _originalCrsr = Cursors.Hand;

		private bool _mouseHover = false;

		/// <summary>
		/// Occurs when the value of the <see cref="Link"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the Link property changes.")]
		public event EventHandler LinkChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="LinkColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the LinkColor property changes.")]
		public event EventHandler LinkColorChanged;

		/// <summary>
		/// Raises the <see cref="LinkChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnLinkChanged(EventArgs e)
		{
			LinkChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="LinkColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnLinkColorChanged(EventArgs e)
		{
			LinkColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Gets or sets the link opened when the control is clicked.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets the link opened when the control is clicked.")]
		public string Link
		{
			get => _link;
			set { _link = value; OnLinkChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the foreground color of the control when hovered over.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the foreground color of the control when hovered over.")]
		public Color LinkColor
		{
			get => _linkColor;
			set { _linkColor = value; OnLinkColorChanged(EventArgs.Empty); }
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

		/// <summary>
		/// Gets or sets the foreground color of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the foreground color of the control.")]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set
			{
				base.ForeColor = value;
				if (!_mouseHover) _foreColor = value;

				OnForeColorChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		public NovaLinkLabel()
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
			ForeColor = _linkColor;
			Font = new Font(Font, FontStyle.Underline);
			if (_useUserSchemeCursor) Cursor = Win32.RegCursor("Hand");
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			_mouseHover = false;
			ForeColor = _foreColor;
			Font = new Font(Font, FontStyle.Regular);
			Invalidate();
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);

			Process.Start(_link);
		}
	}
}
