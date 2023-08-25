using NovaUI.Enums;
using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(ProgressBar))]
	public class NovaProgressBar : Control
	{
		private Color _borderColor = Constants.BorderColor;
		private Color _progressColor = Constants.AccentColor;
		private int _borderWidth = 2;
		private int _borderRadius = 6;
		private int _value = 0;
		private int _defValue = 50;
		private int _maximum = 100;
		private int _minimum = 0;
		private bool _showValue = false;
		private ProgressValueAlignment _valueAlign = ProgressValueAlignment.Center;
		private string _font = "Segoe UI";

		/// <summary>
		/// Occurs when the value of the <see cref="BorderColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the BorderColor property changes.")]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="ProgressColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the ProgressColor property changes.")]
		public event EventHandler ProgressColorChanged;

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
		/// Occurs when the value of the <see cref="Value"/> property changes.
		/// </summary>
		[Category("Value"), Description("Occurs when the value of the Value property changes.")]
		public event EventHandler ValueChanged;

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
		/// Raises the <see cref="ProgressColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnProgressColorChanged(EventArgs e)
		{
			ProgressColorChanged?.Invoke(this, e);
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
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnValueChanged(EventArgs e)
		{
			ValueChanged?.Invoke(this, e);
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
		/// Gets or sets the background color of the control progress indicator.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the control progress indicator.")]
		public Color ProgressColor
		{
			get => _progressColor;
			set { _progressColor = value; OnProgressColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the border width of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border width of the control.")]
		public int BorderWidth
		{
			get => _borderWidth;
			set { _borderWidth = value; OnBorderWidthChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the border radius of the control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the border radius of the control.")]
		public int BorderRadius
		{
			get => _borderRadius;
			set { _borderRadius = value; OnBorderRadiusChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the value associated with this control progress bar.
		/// </summary>
		[Category("Progress Indicator"), Description("Gets or sets the value associated with this control progress bar.")]
		public int Value
		{
			get => _value;
			set
			{
				if (_maximum < value) value = _maximum;
				else if (_minimum > value) value = _minimum;
				_value = value;
				OnValueChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the default value associated with this control progress bar.
		/// </summary>
		[Category("Progress Indicator"), Description("Gets or sets the default value associated with this control progress bar.")]
		public int DefaultValue
		{
			get => _defValue;
			set
			{
				if (_maximum < value) value = _maximum;
				else if (_minimum > value) value = _minimum;
				_value = value;
				OnValueChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the maximum value the control progress bar can bet at.
		/// </summary>
		[Category("Progress Indicator"), Description("Gets or sets the maximum value the control progress bar can be at.")]
		public int Maximum
		{
			get => _maximum;
			set
			{
				if (value < _value)
				{
					_value = value;
					OnValueChanged(EventArgs.Empty);
				}
				if (value < _minimum) _minimum = value - 2;
				_maximum = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the minimum value the control progress bar can be at.
		/// </summary>
		[Category("Progress Indicator"), Description("Gets or sets the minimum value the control progress bar can be at.")]
		public int Minimum
		{
			get => _minimum;
			set
			{
				if (value > _value)
				{
					_value = value;
					OnValueChanged(EventArgs.Empty);
				}
				if (value > _maximum) _maximum = value + 2;
				_minimum = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control will display its current value.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets a value indicating whether the control will display its current value.")]
		public bool ShowValue
		{
			get => _showValue;
			set { _showValue = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets where the control's current value is displayed, if <see cref="ShowValue"/> is set to <see langword="true"/>.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets where the control's current value is displayed, if ShowValue is set to true.")]
		public ProgressValueAlignment ValueAlign
		{
			get => _valueAlign;
			set { _valueAlign = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the font name associated with the control font.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the font name associated with the control font.")]
		public new string Font
		{
			get => _font;
			set { _font = value; OnFontChanged(EventArgs.Empty); Invalidate(); }
		}

		/// <summary>
		/// Replaced by the <see cref="Value"/> property.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The Text property is replaced by the Value property.", true)]
		public new string Text => string.Empty;

		public NovaProgressBar()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = "Segoe UI";
			BackColor = Constants.PrimaryColor;
			ForeColor = Constants.TextColor;
			Size = new Size(200, 12);
		}

		/// <summary>
		/// Resets, really just "sets", the value of the <see cref="Value"/> property to the value of the <see cref="DefaultValue"/> property.
		/// </summary>
		public void ResetValue()
		{
			_value = _defValue;
			OnValueChanged(EventArgs.Empty);
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.Clear(Parent.BackColor);
			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, _borderRadius, _borderRadius));

			Rectangle border = new Rectangle(0, 0, Width - 1, Height - 1);
			if (_showValue)
				switch (_valueAlign)
				{
					case ProgressValueAlignment.Left:
						border.X = 30;
						border.Width -= 30;
						break;
					case ProgressValueAlignment.Right:
						border.Width -= 30;
						break;
				}

			float percent = _value / (float)(_maximum - _minimum);
			Rectangle progress = new Rectangle(border.X + _borderWidth + 1, border.Y + _borderWidth + 1, (int)((border.Width - (_borderWidth * 2)) * percent) - 2, border.Height - (_borderWidth * 2) - 2);

			if (_borderRadius > 0)
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

				int radius;
				if (_value > 2)
				{
					if (_borderRadius > 1)
						radius = _borderRadius - 1;
					else radius = _borderRadius;
				}
				else radius = 0;

				e.Graphics.FillPath(_borderColor.ToBrush(), border.Roundify(_borderRadius));
				e.Graphics.FillPath(BackColor.ToBrush(),
					new Rectangle(border.X + _borderWidth, border.Y + _borderWidth, border.Width - (_borderWidth * 2), border.Height - (_borderWidth * 2)).Roundify(_borderRadius > 1 ? _borderRadius - 1 : 0));
				if (radius > 0)
					e.Graphics.FillPath(_progressColor.ToBrush(), progress.Roundify(radius));
				else
					e.Graphics.FillRectangle(_progressColor.ToBrush(), progress);
			}
			else
			{
				e.Graphics.FillRectangle(_borderColor.ToBrush(), border);
				e.Graphics.FillRectangle(BackColor.ToBrush(),
					new Rectangle(border.X + _borderWidth, border.Y + _borderWidth, border.Width - (_borderWidth * 2), border.Height - (_borderWidth * 2)));
				e.Graphics.FillRectangle(_progressColor.ToBrush(), progress);
			}
		}
	}
}
