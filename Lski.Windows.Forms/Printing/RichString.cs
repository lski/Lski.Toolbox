using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Lski.Windows.Forms.Printing {

	/// <summary>
	/// Simply holds a string and the font associated with that string to display. The string is retrieved via ToString().
	/// </summary>
	/// <remarks></remarks>
	public class RichString {

		private Brush _defaultBrush = Brushes.Black;

		private Font _defaultFont = SystemFonts.MessageBoxFont;
		private string _string;
		private Font _font;
		private System.Drawing.StringFormat _format;

		private Brush _brush;
		public RichString(string @string) {
			this.Init(@string, _defaultFont, null, _defaultBrush);
		}

		public RichString(string @string, Font font) {
			this.Init(@string, font, null, _defaultBrush);
		}

		public RichString(string @string, Font font, StringFormat format) {
			this.Init(@string, font, format, _defaultBrush);
		}

		public RichString(string @string, Font font, StringFormat format, Brush brush) {
			this.Init(@string, font, format, brush);
		}

		private void Init(string str, Font font, StringFormat format, Brush brush) {
			_string = str;
			_font = font;
			_format = format;
			_brush = brush;
		}

		public StringFormat Format {
			get { return this._format; }
			set { this._format = value; }
		}

		public Font Font {
			get { return _font; }
			set { _font = value; }
		}

		public string String {
			get { return _string; }
			set { _string = value; }
		}

		public Brush Brush {
			get { return _brush; }
			set { _brush = value; }
		}

		public Color Color {
			get { return BrushToColor(_brush); }
			set { _brush = ColorToBrush(value); }
		}

		public override string ToString() {
			return _string;
		}

		public Brush ColorToBrush(string hexColor) {
			return new SolidBrush(Color.FromArgb(int.Parse(hexColor.Substring(1), System.Globalization.NumberStyles.HexNumber)));
		}

		public Brush ColorToBrush(Color c) {
			return new SolidBrush(c);
		}

		public Color BrushToColor(Brush b) {

			if ((b is SolidBrush)) return ((SolidBrush)b).Color;

			throw new ArgumentException("When converting a Brush object to a color object, the brush must be a SolidBrush");
		}

		/// <summary>
		/// If the passed RichString object is null or the length is zero this returns true, because it is empty.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static bool IsNullOrEmpty(RichString str) {
			return (str) == null || string.IsNullOrEmpty(str.String) ? true : false;
		}


	}
}
