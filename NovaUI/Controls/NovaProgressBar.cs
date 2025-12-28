using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NovaUI.Helpers;
using NovaUI.Helpers.LibMain;

namespace NovaUI.Controls
{
	[ToolboxBitmap(typeof(ProgressBar))]
	[DefaultEvent("ValueChanged")]
	public class NovaProgressBar : Control
	{
		private readonly Pen borderPen = Color.Transparent.ToPen();
		private readonly SolidBrush backBrush = Color.Transparent.ToBrush();
		private readonly Pen backPen = Color.Transparent.ToPen();
		private readonly SolidBrush progressBrush = Color.Transparent.ToBrush();
		private readonly Pen progressPen = Color.Transparent.ToPen();

		private Color borderColor = Constants.BorderColor;
		private Color progressColor = Constants.AccentColor;
		private int borderWidth = 1;
		private int borderRadius = 6;
		private int value = 50;
		private int defaultValue = 50;
		private int maximum = 100;
		private int minimum = 0;

		private float percent;

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
		/// Occurs when the value of the <see cref="ValueChanged"/> property changes.
		/// </summary>
		[Category("Property"), Description("Occurs when the value of the ValueChanged property changes.")]
		public event EventHandler ValueChanged;

		protected virtual void OnBorderColorChanged(EventArgs e)
		{
			BorderColorChanged?.Invoke(this, e);
			Invalidate();
		}

		protected virtual void OnProgressColorChanged(EventArgs e)
		{
			ProgressColorChanged?.Invoke(this, e);
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

		protected virtual void OnValueChanged(EventArgs e)
		{
			ValueChanged?.Invoke(this, e);
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
				if (borderPen.Color != value) borderPen.Color = value;
				OnBorderColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the background color of the control progress indicator.")]
		public Color ProgressColor
		{
			get => progressColor;
			set
			{
				progressColor = value;
				if (progressBrush.Color != value)
				{
					progressBrush.Color = value;
					progressPen.Color = value;
				}
				OnProgressColorChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance"), Description("Gets or sets the border width of the control.")]
		public int BorderWidth
		{
			get => borderWidth;
			set
			{
				borderWidth = value;
				if (borderPen.Width != value) borderPen.Width = value;
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
				if (value != borderRadius)
					Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, value, value));
				borderRadius = value;
				OnBorderRadiusChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Progress Indicator"), Description("Gets or sets the value associated with the control progress bar.")]
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
		[Category("Progress Indicator"), Description("Gets or sets the default value associated with the control progress bar.")]
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
		[Category("Progress Indicator"), Description("Gets or sets the maximum value the control progress bar can be at.")]
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
		[Category("Progress Indicator"), Description("Gets or sets the minimum value the control progress bar can be at.")]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font => new Font("Segoe UI", 0.1f);

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The Text property is replaced by the Value property.", true)]
		public new string Text => string.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("The ForeColor property is not needed with absence of text.", true)]
		public new Color ForeColor => Color.Empty;

		public NovaProgressBar()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;

			borderPen = new Pen(borderColor, borderWidth);
			backPen = new Pen(backBrush = (BackColor = Constants.PrimaryColor).ToBrush());
			progressPen = new Pen(progressBrush = progressColor.ToBrush());
		}

		public void ResetValue()
		{
			value = defaultValue;
			OnValueChanged(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			Region = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, borderRadius, borderRadius));
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			base.OnParentBackColorChanged(e);
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
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.Clear(Parent != null ? Parent.BackColor : Color.Transparent);

			percent = (value - minimum) / (float)(maximum - minimum);

			if (borderRadius > 0)
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if (borderWidth > 0)
				{
					e.Graphics.FillPath(backBrush,
						new Rectangle(borderWidth - 1, borderWidth - 1, Width - (borderWidth * 2) + 1, Height - (borderWidth * 2) + 1).Round(Math.Max(1, borderRadius - borderWidth)));
					e.Graphics.DrawPath(borderPen,
						new RectangleF(borderWidth / 2f - 0.5f, borderWidth / 2f - 0.5f, Width - borderWidth, Height - borderWidth).Round(borderRadius));

					e.Graphics.SetClip(new Rectangle(borderWidth, borderWidth, (int)((Width - (borderWidth * 2)) * percent), Height - (borderWidth * 2)));
					e.Graphics.FillPath(progressBrush,
						new Rectangle(borderWidth + 1, borderWidth + 1, Width - (borderWidth * 2) - 3, Height - (borderWidth * 2) - 3).Round(borderRadius - borderWidth));
					e.Graphics.DrawPath(progressPen,
						new Rectangle(borderWidth + 1, borderWidth + 1, Width - (borderWidth * 2) - 3, Height - (borderWidth * 2) - 3).Round(borderRadius - borderWidth));
				}
				else
				{
					e.Graphics.FillPath(backBrush,
						new Rectangle(0, 0, Width - 1, Height - 1).Round(borderRadius));
					e.Graphics.DrawPath(backPen,
						new Rectangle(0, 0, Width - 1, Height - 1).Round(borderRadius));

					e.Graphics.SetClip(new Rectangle(0, 0, (int)(Width * percent), Height));
					e.Graphics.FillPath(progressBrush,
						new Rectangle(0, 0, Width - 1, Height - 1).Round(borderRadius));
					e.Graphics.DrawPath(progressPen,
						new Rectangle(0, 0, Width - 1, Height - 1).Round(borderRadius));
				}
			}
			else
			{
				if (borderWidth > 0)
				{
					e.Graphics.FillRectangle(backBrush,
						new Rectangle(borderWidth, borderWidth, Width - (borderWidth * 2), Height - (borderWidth * 2)));
					e.Graphics.DrawRectangle(borderPen,
						borderWidth / 2f, borderWidth / 2f, Width - borderWidth, Height - borderWidth);

					e.Graphics.SetClip(new Rectangle(borderWidth, borderWidth, (int)((Width - (borderWidth * 2)) * percent), Height - (borderWidth * 2)));
					e.Graphics.FillRectangle(progressBrush,
						new Rectangle(borderWidth + 1, borderWidth + 1, Width - ((borderWidth + 1) * 2), Height - ((borderWidth + 1) * 2)));
				}
				else
				{
					e.Graphics.FillRectangle(backBrush, new Rectangle(0, 0, Width, Height));

					e.Graphics.SetClip(new Rectangle(0, 0, (int)(Width * percent), Height));
					e.Graphics.FillRectangle(progressBrush, new Rectangle(0, 0, Width, Height));
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				borderPen?.Dispose();
				backBrush?.Dispose();
				backPen?.Dispose();
				progressBrush?.Dispose();
				progressPen?.Dispose();
			}
		}
	}
}
