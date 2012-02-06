using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Lski.Txt;
using System.Drawing;
using System.Windows.Forms;

namespace Lski.Windows.Forms.Controls
{
	public class AdvGroupBox : GroupBox
	{

		private Color _titleColor;
		private Color _borderColor;
		private Int32 _cornerDepth;

		private Color _innerBackColor;

		public const Int32 DefaultCornerDepth = 8;

		public AdvGroupBox() : base()
		{
			this.BorderColor = Color.FromArgb(208, 208, 191);
			this.CornerDepth = DefaultCornerDepth;
			this.InnerBackColor = Color.Transparent;
			this.InnerBackColor = Color.Transparent;
			this.TitleColor = Color.FromArgb(0, 70, 213);

		}

		public Color BorderColor {
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		public Color InnerBackColor {
			get { return _innerBackColor; }
			set { _innerBackColor = value; }
		}

		public Color TitleColor {
			get { return _titleColor; }
			set { _titleColor = value; }
		}

		public Int32 CornerDepth {
			get { return _cornerDepth; }
			set { _cornerDepth = value; }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var borderPen = new Pen(this.BorderColor);
			var fontBrush = new SolidBrush(this.TitleColor);
			var innerBackBrush = new SolidBrush(this.InnerBackColor);
			var backBrush = new SolidBrush(this.BackColor);

			Size tSize = TextRenderer.MeasureText(this.Text, this.Font);

			var borderRect = new Rectangle(new Point(0, 0), this.ClientSize);

			// 1. If there is text then adjust the size of the rectangle to draw
			// 2. Other wise just drop the width and height by 1
			if (string.IsNullOrEmpty(this.Text)) {
				borderRect.Y = (borderRect.Y + (int)Math.Round(tSize.Height / 2d));
				borderRect.Height = ((borderRect.Height - 1) - (int)Math.Round(tSize.Height / 2d));
			} else {
				borderRect.Height -= 1;

			}

			// Either way drop width by one
			borderRect.Width -= 1;

			// Draw the rounded rectangle and border
			FillRoundedRectangle(e.Graphics, borderRect, CornerDepth, innerBackBrush);
			DrawRoundedRectangle(e.Graphics, borderRect, CornerDepth, borderPen);

			// If text is not empty, then place it on the drawing

			if (string.IsNullOrEmpty(this.Text)) {
				StringFormat sF = new StringFormat();
				sF.LineAlignment = StringAlignment.Center;
				sF.Alignment = StringAlignment.Center;

				Rectangle textRect = new Rectangle(new Point(0, 0), this.ClientSize);
				textRect.X = (textRect.X + 2 + Convert.ToInt32(this.CornerDepth / 2));
				textRect.Width = tSize.Width + 2;
				textRect.Height = tSize.Height;

				e.Graphics.FillRectangle(backBrush, textRect);
				e.Graphics.DrawString(this.Text, this.Font, fontBrush, textRect, sF);

				// If the back colour does not match the innerBackColor, then a half line separator needs to be drawn to separate it

				if ((!this.InnerBackColor.Equals(this.BackColor)) && (!this.InnerBackColor.Equals(Color.Transparent))) {
					// Horizontal
					e.Graphics.DrawLine(borderPen, textRect.X, (textRect.Y + textRect.Height), textRect.X + textRect.Width, (textRect.Y + textRect.Height));
					//Left vertical
					e.Graphics.DrawLine(borderPen, textRect.X, textRect.Y + Convert.ToInt32(textRect.Height / 2), textRect.X, textRect.Y + textRect.Height);

					//right vertical
					e.Graphics.DrawLine(borderPen, textRect.X + textRect.Width, textRect.Y + Convert.ToInt32(textRect.Height / 2), textRect.X + textRect.Width, textRect.Y + textRect.Height);
				}
			}
		}


		public void DrawRoundedRectangle(Graphics g, Rectangle r, int d, Pen p)
		{
			g.DrawLine(p, Convert.ToInt32(r.X + d / 2), r.Y, Convert.ToInt32(r.X + r.Width - d / 2), r.Y);
			g.DrawLine(p, r.X, Convert.ToInt32(r.Y + d / 2), r.X, Convert.ToInt32(r.Y + r.Height - d / 2));
			g.DrawLine(p, r.X + r.Width, Convert.ToInt32(r.Y + d / 2), r.X + r.Width, Convert.ToInt32(r.Y + r.Height - d / 2));
			g.DrawLine(p, Convert.ToInt32(r.X + d / 2), r.Y + r.Height, Convert.ToInt32(r.X + r.Width - d / 2), r.Y + r.Height);

			g.DrawArc(p, r.X, r.Y, d, d, 180, 90);
			g.DrawArc(p, r.X + r.Width - d, r.Y, d, d, 270, 90);
			g.DrawArc(p, r.X, r.Y + r.Height - d, d, d, 90, 90);
			g.DrawArc(p, r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);

		}


		public static void FillRoundedRectangle(Graphics g, Rectangle r, int d, Brush b)
		{
			// anti alias distorts fill so remove it (temp)

			System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

			g.FillPie(b, r.X, r.Y, d, d, 180, 90);
			g.FillPie(b, r.X + r.Width - d, r.Y, d, d, 270, 90);
			g.FillPie(b, r.X, r.Y + r.Height - d, d, d, 90, 90);
			g.FillPie(b, (r.X + r.Width - d), (r.Y + r.Height - d), d, d, 0, 90);

			g.FillRectangle(b, Convert.ToInt32(r.X + d / 2), r.Y, r.Width - d, Convert.ToInt32(d / 2));
			g.FillRectangle(b, r.X, Convert.ToInt32(r.Y + d / 2), r.Width, r.Height - d);
			g.FillRectangle(b, Convert.ToInt32(Math.Floor((double)r.X + d / 2)), Convert.ToInt32(Math.Floor((double)r.Y + r.Height - d / 2)), r.Width - d, Convert.ToInt32(Math.Ceiling((double)d / 2)));

			g.SmoothingMode = mode;

		}


	}


}

