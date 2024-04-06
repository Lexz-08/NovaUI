using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Enums;
using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(TrackBar))]
	[DefaultEvent("ValueChanged")]
	public class NovaSlider : Control
	{
		private Color _trackColor = Constants.SecondaryColor;
		private Color _knobColor = Constants.BorderColor;
		private Color _dragColor = Constants.AccentColor;
		private int _value = 50;
		private int _defValue = 50;
		private int _maximum = 100;
		private int _minimum = 0;
		private bool _showValue = false;
		private SliderStyle _sliderStyle = SliderStyle.SolidTrack;
		private SliderKnobStyle _knobStyle = SliderKnobStyle.SolidKnob;
		private bool _useUserSchemeCursor = true;
		private Cursor _originalCrsr = Cursors.Hand;

		private Rectangle _knob;
		private bool _canDrag = false;
		private int _offsetX = 0;

		private bool _mouseHover = false;
		private bool _mouseDown = false;

		/// <summary>
		/// Occurs when the value of the <see cref="TrackColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the TrackColor property changes.")]
		public event EventHandler TrackColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="KnobColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the KnobColor property changes.")]
		public event EventHandler KnobColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="DragColor"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the DragColor property changes.")]
		public event EventHandler DragColorChanged;

		/// <summary>
		/// Occurs when the value of the <see cref="Value"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the Value property changes.")]
		public event EventHandler ValueChanged;

		/// <summary>
		/// Occurs when the user drags the knob along the slider track.
		/// </summary>
		[Category("Behavior"), Description("Occurs when the user drags the knob along the slider track.")]
		public event EventHandler SliderDrag;

		/// <summary>
		/// Raises the <see cref="TrackColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnTrackColorChanged(EventArgs e)
		{
			TrackColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="KnobColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnKnobColorChanged(EventArgs e)
		{
			KnobColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="DragColorChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnDragColorChanged(EventArgs e)
		{
			DragColorChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected void OnValueChanged(EventArgs e)
		{
			ValueChanged?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Raises the <see cref="SliderDrag"/> event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnSliderDrag(EventArgs e)
		{
			SliderDrag?.Invoke(this, e);
			Invalidate();
		}

		/// <summary>
		/// Gets or sets the background color of the control slider track.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the control slider track.")]
		public Color TrackColor
		{
			get => _trackColor;
			set { _trackColor = value; OnTrackColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the background color of the control slider knob.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the control slider knob.")]
		public Color KnobColor
		{
			get => _knobColor;
			set { _knobColor = value; OnKnobColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the background color of the control slider knob when it's selected.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the background color of the control slider knob when it's selected.")]
		public Color DragColor
		{
			get => _dragColor;
			set { _dragColor = value; OnDragColorChanged(EventArgs.Empty); }
		}

		/// <summary>
		/// Gets or sets the value associated with this control slider.
		/// </summary>
		[Category("Slider"), Description("Gets or sets the value associated with this control slider.")]
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
		/// Gets or sets the default value associated with this control slider.
		/// </summary>
		[Category("Slider"), Description("Gets or sets the default value associated with this control slider.")]
		public int DefaultValue
		{
			get => _defValue;
			set
			{
				if (_maximum < value) value = _maximum;
				else if (_minimum > value) value = _minimum;
				_defValue = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the maximum value the control slider can be at.
		/// </summary>
		[Category("Slider"), Description("Gets or sets the maximum value the control slider can be at.")]
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
		/// Gets or sets the minimum value the control slider can be at.
		/// </summary>
		[Category("Slider"), Description("Gets or sets the minimum value the control slider can be at.")]
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
		/// Gets or sets whether the control slider renders as a solid track or a hollow track.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets whether the control slider renders as a solid track or a hollow track.")]
		public SliderStyle SliderStyle
		{
			get => _sliderStyle;
			set { _sliderStyle = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets whether the control slider knob renders as a solid knob or a hollow knob.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets whether the control slider knob renders as a solid knob or a hollow knob.")]
		public SliderKnobStyle KnobStyle
		{
			get => _knobStyle;
			set { _knobStyle = value; Invalidate(); }
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The Text property is replaced by the combination of the Value and ShowValue properties.", true)]
		public new string Text => string.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The BackColor property is not needed.", true)]
		public new Color BackColor => Color.Empty;

		public NovaSlider()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			ForeColor = Constants.TextColor;
			Width = 200;
			Height = 16;
		}

		/// <summary>
		/// Resets, really just "sets", the value of the <see cref="Value"/> property to the value of the <see cref="DefaultValue"/> property.
		/// </summary>
		public void ResetValue()
		{
			_value = _defValue;

			CalculateKnobPosition();
			OnValueChanged(EventArgs.Empty);
		}

		private void CalculateValue(MouseEventArgs e)
		{
			_knob.X = Math.Max(Height / 2, Math.Min(e.X - _offsetX, Width - (Height / 2) - 1));
			double width = Width - Height;
			double knobX = _knob.X - (Height / 2);
			double percent = knobX / width;

			_value = (int)Math.Round((percent * (_maximum - _minimum)) + _minimum, 0);
			OnValueChanged(EventArgs.Empty);
		}
		
		private void CalculateKnobPosition()
		{
			double width = Width - Height;
			double num = _value - _minimum;
			double den = _maximum - _minimum;
			_knob.X = Math.Max((Height / 2) + 1, Math.Min((int)((num / den * width) + (Height / 2)), Width - (Height / 2) - 1));

			Invalidate();
		}

		private void UpdateSlider()
		{
			_knob.Height = Height;
			_knob.Width = _knob.Height;

			CalculateKnobPosition();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			UpdateSlider();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			UpdateSlider();
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
			_canDrag = true;
			_offsetX = e.X - _knob.X;

			int cX = _knob.X;
			int cY = _knob.Y + (_knob.Height / 2);
			int radius = Height / 2;

			if (!(Math.Pow(e.X - cX, 2) + Math.Pow(e.Y - cY, 2) <= Math.Pow(radius, 2)))
			{
				_offsetX = 0;
				_knob.X = e.X - _offsetX;
			}

			CalculateValue(e);
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			_mouseDown = false;
			_canDrag = false;
			_offsetX = 0;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (_canDrag)
			{
				CalculateValue(e);
				CalculateKnobPosition();
				OnSliderDrag(EventArgs.Empty);
			}

			Invalidate();
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

			Rectangle slider = new Rectangle(Height / 2, Height / 4, Width - Height, Height / 2);
			int radius = slider.Height / 2;

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			if (_sliderStyle == SliderStyle.SolidTrack)
				e.Graphics.FillPath(_trackColor.ToBrush(), slider.Roundify(radius));
			else if (_sliderStyle == SliderStyle.BorderTrack)
				e.Graphics.DrawPath(new Pen(_trackColor, 2f), slider.Roundify(radius));
			if (_knobStyle == SliderKnobStyle.SolidKnob)
				e.Graphics.FillEllipse((_mouseHover ? (_mouseDown ? _dragColor : _knobColor.Lighter(0.1f)) : _knobColor).ToBrush(),
					_knob.X - (_knob.Width / 2), _knob.Y - 1, _knob.Width, _knob.Height);
			else if (_knobStyle == SliderKnobStyle.BorderKnob)
				e.Graphics.DrawEllipse(new Pen(_mouseHover ? (_mouseDown ? _dragColor : _knobColor.Lighter(0.1f)) : _knobColor, 2f),
					_knob.X - (_knob.Width / 2), _knob.Y - 1, _knob.Width, _knob.Height);

			if (_showValue)
			{
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

				e.Graphics.DrawString(_value.ToString(), new Font(Font.Name, (float)(_knob.Height / 3f)), ForeColor.ToBrush(),
					new Rectangle(_knob.X - (_knob.Width / 2), _knob.Y + (_knob.Height % 2 == 0 ? 0 : 1), _knob.Width, _knob.Height), Constants.CenterAlign);
			}
		}
	}
}
