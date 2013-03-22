using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Changes the original string passed string by replacing the text in the selected positions with the inserted text
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public class InsertText : Transformation {

		/// <summary>
		/// The start position of the text to remove
		/// </summary>
		public int Start { get; set; }
		/// <summary>
		/// The length of the string to remove
		/// </summary>
		public int Length { get; set; }
		/// <summary>
		/// The text to exchange for the text at the selected position
		/// </summary>
		public string Text { get; set; }

		public override string Process(string value) {

			// If the start position is greater (or equal) to the length of the string then return original string
			if (Start >= value.Length) 
				return value;

			// Otherwise return the calc string,
			return value.SubStringAdv(0, Start) + Text + value.SubStringAdv(Start + Text.Length);
		}

		public override object Clone() {
			return new InsertText() {
				Start = this.Start,
				Length = this.Length,
				Text = this.Text
			};
		}

	}
}
