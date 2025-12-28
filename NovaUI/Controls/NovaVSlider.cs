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
	public class NovaVSlider : Control
	{
		private readonly SolidBrush trackBrush = Color.Transparent.ToBrush();
		private readonly Pen trackPen = new Pen(Color.Transparent, 2);
		private readonly SolidBrush knobNormalBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush knobHoverBrush = Color.Transparent.ToBrush();
		private readonly SolidBrush knobDownBrush = Color.Transparent.ToBrush();
		private readonly Pen knobNormalPen = Color.Transparent.ToPen();
		private readonly Pen knobHoverPen = Color.Transparent.ToPen();
		private readonly Pen knobDownPen = Color.Transparent.ToPen();
		private readonly SolidBrush textBrush = Color.Transparent.ToBrush();
		private Font knobFont;

		private Color trackColor = Constants.SecondaryColor;
		private Color knobColor = Constants.BorderColor;
		private Color dragColor = Constants.AccentColor;
		private int value = 50;
		private int defaultValue = 50;
		private int maximum = 100;
		private int minimum = 0;
		private bool showValue = false;
		private SliderStyle sliderStyle = SliderStyle.SolidTrack;
		private SliderKnobStyle knobStyle = SliderKnobStyle.SolidKnob;
		private bool useUserSchemeCursor = true;
		private Cursor originalCursor = Cursors.Hand;

		private Rectangle knob;
		private bool canDrag = false;
		private int offsetY = 0;

		private bool mouseHover = false;
		private bool mouseDown = false;

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

		protected virtual void OnTrackColorChanged(EventArgs e)
		{
			TrackColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnKnobColorChanged(EventArgs e)
		{
			KnobColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnDragColorChanged(EventArgs e)
		{
			DragColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnValueChanged(EventArgs e)
		{
			ValueChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnSliderDrag(EventArgs e)
		{
			SliderDrag?.Invoke(this, e);
			Invalidate();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the background color of the control slider track.")]
		public Color TrackColor
		{
			get => trackColor;
			set
			{
				trackColor = value;
				if (trackBrush.Color != value)
				{
					trackBrush.Color = value;
					trackPen.Color = value;
				}
				OnTrackColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the background color of the control slider knob.")]
		public Color KnobColor
		{
			get => knobColor;
			set
			{
				knobColor = value;
				if (knobNormalBrush.Color != value)
				{
					knobNormalBrush.Color = value;
					knobHoverBrush.Color = value.Lighter(0.1f);
					knobNormalPen.Color = value;
					knobHoverPen.Color = value.Lighter(0.1f);
				}
				OnKnobColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the background color of the control slider knob when it's selected.")]
		public Color DragColor
		{
			get => dragColor;
			set
			{
				dragColor = value;
				if (knobHoverBrush.Color != value)
				{
					knobDownBrush.Color = value;
					knobDownPen.Color = value;
				}
				OnDragColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Slider"), Description("Gets or sets the value associated with this control slider.")]
		public int Value
		{
			get => value;
			set
			{
				value = Math.Max(minimum, Math.Min(value, maximum));
				this.value = value;
				OnValueChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Slider"), Description("Gets or sets the default value associated with this control slider.")]
		public int DefaultValue
		{
			get => defaultValue;
			set
			{
				value = Math.Max(minimum, Math.Min(value, maximum));
				defaultValue = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Slider"), Description("Gets or sets the maximum value the control slider can be at.")]
		public int Maximum
		{
			get => maximum;
			set
			{
				if (value < this.value)
				{
					this.value = value;
					OnValueChanged(EventArgs.Empty);
				}
				if (value < minimum) minimum = value - 2;
				maximum = value;
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Slider"), Description("Gets or sets the minimum value the control slider can be at.")]
		public int Minimum
		{
			get => minimum;
			set
			{
				if (value > this.value)
				{
					this.value = value;
					OnValueChanged(EventArgs.Empty);
				}
				if (value > maximum) maximum = value + 2;
				minimum = value;
				Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets a value indicating whether the control will display its current value.")]
		public bool ShowValue
		{
			get => ShowValue;
			set { showValue = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets whether the control slider renders as a solid track or a hollow track.")]
		public SliderStyle SliderStyle
		{
			get => sliderStyle;
			set { sliderStyle = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets whether the control slider knob renders as a solid knob or a hollow knob.")]
		public SliderKnobStyle KnobStyle
		{
			get => knobStyle;
			set { knobStyle = value; Invalidate(); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Behavior"), Description("Gets or sets a value indicating whether the control will the user-selected system scheme cursor.")]
		public bool UseUserSchemeCursor
		{
			get => useUserSchemeCursor;
			set { useUserSchemeCursor = value; Invalidate(); }
		}

		[Category("Appearance"), Description("Gets or sets the cursor that is displayed when the mouse pointer is over the control.")]
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
		[Obsolete("The Text property is replaced by the combination of the Value and ShowValue properties.", true)]
		public new string Text => string.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The BackColor property is not needed.", true)]
		public new Color BackColor => Color.Empty;

		public NovaVSlider()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			Font = new Font("Segoe UI", 9f);
			ForeColor = Constants.TextColor;
			Width = 16;
			Height = 200;
		}

		public void ResetValue()
		{
			value = defaultValue;

			CalculateKnobPosition();
			OnValueChanged(EventArgs.Empty);
		}

		private void CalculateValue(MouseEventArgs e)
		{
			knob.Y = Math.Max(Width / 2, Math.Min(e.Y - offsetY, Height - (Width / 2) - 1));
			double height = Height - Width;
			double knobY = knob.Y - (Width / 2);
			double percent = knobY / height;

			value = (int)Math.Round((percent * (maximum - minimum)) + minimum, 0);
			OnValueChanged(EventArgs.Empty);
		}

		private void CalculateKnobPosition()
		{
			double height = Height - Width;
			double num = value - minimum;
			double den = maximum - minimum;
			knob.Y = Math.Max((Width / 2) + 1, Math.Min((int)((num / den * height) + (Width / 2)), Height - (Width / 2) - 1));

			Invalidate();
		}

		private void UpdateSlider()
		{
			knob.Width = Width;
			knob.Height = knob.Width;
			knobFont = new Font(Font.Name, knob.Height / 3f);

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

			mouseHover = true;
			if (useUserSchemeCursor) Win32.GetRegistryCursor(Win32.RegistryCursor.Hand, this);
			else Cursor = originalCursor;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			mouseHover = false;
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			mouseDown = true;
			canDrag = true;
			offsetY = e.Y - knob.Y;

			int cX = knob.X + (knob.Width / 2);
			int cY = knob.Y;
			int radius = Width / 2;

			if (!(Math.Pow(e.X - cX, 2) + Math.Pow(e.Y - cY, 2) <= Math.Pow(radius, 2)))
			{
				offsetY = 0;
				knob.Y = e.Y - offsetY;
			}

			CalculateValue(e);
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			mouseDown = false;
			canDrag = false;
			offsetY = 0;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (canDrag)
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

			e.Graphics.Clear(Parent != null ? Parent.BackColor : Color.Transparent);

			Rectangle slider = new Rectangle(Width / 4, Width / 2, Width / 2, Height - Width);
			int radius = slider.Width / 2;

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			if (sliderStyle == SliderStyle.SolidTrack)
				e.Graphics.FillPath(trackBrush, slider.Round(radius));
			else if (sliderStyle == SliderStyle.BorderTrack)
				e.Graphics.DrawPath(trackPen, slider.Round(radius));
			if (knobStyle == SliderKnobStyle.SolidKnob)
				e.Graphics.FillEllipse(mouseHover ? (mouseDown ? knobDownBrush : knobHoverBrush) : knobNormalBrush,
					knob.X - 1, knob.Y - (knob.Height / 2), knob.Width, knob.Height);
			else if (knobStyle == SliderKnobStyle.BorderKnob)
				e.Graphics.DrawEllipse(mouseHover ? (mouseDown ? knobDownPen : knobHoverPen) : knobNormalPen,
					knob.X - 1, knob.Y - (knob.Height / 2), knob.Width, knob.Height);

			if (showValue)
			{
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

				e.Graphics.DrawString(value.ToString(), knobFont, textBrush,
					new Rectangle(knob.X - 1, knob.Y - (knob.Height / 2) + (knob.Height % 2 == 0 ? 0 : 1), knob.Width, knob.Height), Constants.CenterAlign);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				trackBrush?.Dispose();
				trackPen?.Dispose();
				knobNormalBrush?.Dispose();
				knobHoverBrush?.Dispose();
				knobDownBrush?.Dispose();
				knobNormalPen?.Dispose();
				knobHoverPen?.Dispose();
				knobDownPen?.Dispose();
				textBrush?.Dispose();
			}
		}
	}
}
