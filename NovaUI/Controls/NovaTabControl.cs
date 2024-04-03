using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Enums;
using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(TabControl))]
	public class NovaTabControl : TabControl
	{
		private Color _activeTabColor = Constants.AccentColor;
		private Color _tabColor = Constants.SecondaryColor;
		private Color _activeTabForeColor = Constants.TextColor;
		private Color _tabForeColor = Constants.TextColor.Darker(0.2f);
		private bool _underlineTabs = false;
		private int _borderRadius = 6;
		private TabStyle _tabStyle = TabStyle.Block;
		private bool _useUserSchemeCursor = true;
		private Cursor _originalCrsr = Cursors.Hand;

		private bool _mouseHover = false;
		private bool _mouseDown = false;

		/// <summary>
		/// Occurs when the value of the <see cref="ActiveTabColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the ActiveTabColor property changes.")]
		public event EventHandler ActiveTabColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="TabColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the TabColor property changes.")]
		public event EventHandler TabColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="ActiveTabForeColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the ActiveTabForeColor property changes.")]
		public event EventHandler ActiveTabForeColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="TabForeColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the TabForeColor property changes.")]
		public event EventHandler TabForeColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="BorderRadius"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderRadius property changes.")]
		public event EventHandler BorderRadiusChanged;

		/// <summary>
		/// Raises the <see cref="ActiveTabColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnActiveTabColorChanged(EventArgs e)
		{
			ActiveTabColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="TabColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnTabColorChanged(EventArgs e)
		{
			TabColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="ActiveTabForeColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnActiveTabForeColorChanged(EventArgs e)
		{
			ActiveTabForeColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="TabForeColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnTabForeColorChanged(EventArgs e)
		{
			TabForeColorChanged?.Invoke(this, e);
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
		/// Gets or sets the background color of the currently active tab.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the currently active tab.")]
		public Color ActiveTabColor
		{
			get => _activeTabColor;
			set { _activeTabColor = value; OnActiveTabColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the background color of the currently inactive tabs.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the currently inactive tabs.")]
		public Color TabColor
		{
			get => _tabColor;
			set { _tabColor = value; OnTabColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the foreground color of the currently active tab.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the foreground color of the currently active tab.")]
		public Color ActiveTabForeColor
		{
			get => _activeTabForeColor;
			set { _activeTabForeColor = value; OnActiveTabForeColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the foreground color of the currently inactive tabs.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the foreground color of the currently inactive tabs.")]
		public Color TabForeColor
		{
			get => _tabForeColor;
			set { _tabForeColor = value; OnTabForeColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether a line is displayed under the tab buttons rather than a full rectangle.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets a value indicating whether a line is displayed under the tab buttons rather than a full rectangle.")]
		public bool UnderlineTabs
		{
			get => _underlineTabs;
			set { _underlineTabs = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the border radius of the tabs if <see cref="TabStyle"/> is set to:<br/>
		/// <list type="bullet">
		///		<item>RadialBlock</item>
		///		<item>RadialBox</item>
		///		<item>RadialUnderline</item>
		/// </list>
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border radius of the tabs if TabStyle is set to:\n\n\t-RadialBlock\n\t-RadialBox\n\t-RadialUnderline")]
		public int BorderRadius
		{
			get => _borderRadius;
			set { _borderRadius = value; OnBorderRadiusChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets how the tabs are drawn.
		/// </summary>
		[Category("Appearance"), Description("Gets or setes how the tabs are drawn.")]
		public TabStyle TabStyle
		{
			get => _tabStyle;
			set { _tabStyle = value; Invalidate(); }
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

		public override Rectangle DisplayRectangle
		{
			get
			{
				Rectangle displayRect = base.DisplayRectangle;

				return new Rectangle(displayRect.X - 2, displayRect.Y - 2, displayRect.Width + 4, displayRect.Height + 4);
			}
		}

		public NovaTabControl()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			ItemSize = new Size(120, 30);
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

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.Clear(Parent.BackColor);

			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			for (int i = 0; i < TabCount; i++)
			{
				Rectangle tab = GetTabRect(i);
				bool activeTab = i == SelectedIndex;
				bool containsMouse = tab.Contains(PointToClient(MousePosition));

				switch (_tabStyle)
				{
					case TabStyle.Block:
						e.Graphics.FillRectangle((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(), tab);
						break;
					case TabStyle.Box:
						e.Graphics.FillRectangle((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(), tab);
						e.Graphics.FillRectangle(Parent.BackColor.Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(),
							tab.X + 1, tab.Y + 1, tab.Width - 2, tab.Height - 2);
						break;
					case TabStyle.Underline:
						e.Graphics.FillRectangle((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(),
							tab.X + 4, tab.Y + tab.Height - 2, tab.Width - 8, 2);
						break;
					case TabStyle.RadialBlock:
						if (_borderRadius > 0)
						{
							e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
							e.Graphics.FillPath((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseHover && containsMouse ? 0.1f : 0).ToBrush(),
								new Rectangle(tab.X, tab.Y, tab.Width - 1, tab.Height - 1).RoundifyCorners(_borderRadius, i == 0, i == TabPages.Count - 1, i == 0, i == TabPages.Count - 1));
						}
						else e.Graphics.FillRectangle((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(), tab);
						break;
					case TabStyle.RadialBox:
						if (_borderRadius > 0)
						{
							e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
							e.Graphics.FillPath((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseHover && containsMouse ? 0.1f : 0).ToBrush(),
								new Rectangle(tab.X, tab.Y, tab.Width - 1, tab.Height - 1).RoundifyCorners(_borderRadius, i == 0, i == TabPages.Count - 1, i == 0, i == TabPages.Count - 1));
							e.Graphics.FillPath(Parent.BackColor.Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseHover && containsMouse ? 0.1f : 0).ToBrush(),
								new Rectangle(tab.X + 1, tab.Y + 1, tab.Width - 3, tab.Height - 3).RoundifyCorners(_borderRadius > 1 ? _borderRadius - 1 : _borderRadius, i == 0, i == TabPages.Count - 1, i == 0, i == TabPages.Count - 1));
						}
						else
						{
							e.Graphics.FillRectangle((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(), tab);
							e.Graphics.FillRectangle(Parent.BackColor.Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(),
								tab.X + 1, tab.Y + 1, tab.Width - 2, tab.Height - 2);
						}
						break;
					case TabStyle.RadialUnderline:
						if (_borderRadius > 0)
						{
							e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
							e.Graphics.FillPath((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(),
								new Rectangle(tab.X + 5, tab.Y + 1, tab.Width - 11, tab.Height - 3).Roundify(_borderRadius));
							e.Graphics.FillPath(Parent.BackColor.Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(),
								new Rectangle(tab.X + 4, tab.Y, tab.Width - 9, tab.Height - 5).Roundify(_borderRadius));
						}
						else e.Graphics.FillRectangle((activeTab ? _activeTabColor : _tabColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(),
							tab.X + 4, tab.Y + tab.Height - 2, tab.Width - 8, 2);
						break;
				}

				switch (_tabStyle)
				{
					case TabStyle.Block:
					case TabStyle.Box:
					case TabStyle.RadialBlock:
					case TabStyle.RadialBox:
						e.Graphics.DrawString(TabPages[i].Text, Font, (activeTab ? _activeTabForeColor : _tabForeColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(),
							new Rectangle(tab.X, tab.Y + 1, tab.Width, tab.Height),
							Constants.CenterAlign);
						break;
					case TabStyle.Underline:
					case TabStyle.RadialUnderline:
						e.Graphics.DrawString(TabPages[i].Text, Font, (activeTab ? _activeTabForeColor : _tabForeColor).Lighter(_mouseHover && containsMouse ? 0.1f : 0).Darker(_mouseDown && containsMouse ? 0.1f : 0).ToBrush(),
							new Rectangle(tab.X, tab.Y - 2, tab.Width, tab.Height),
							Constants.CenterAlign);
						break;
				}
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;

				return cp;
			}
		}
	}
}
