using System;

namespace Lski.Windows.Forms.Misc {

	/// <summary>
	/// Provides some basic event handlers
	/// </summary>
	/// <remarks></remarks>
	public class Handlers {

		/// <summary>
		/// Prevents anything other than a numeric value being placed into a field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>

		public static void ExcludeNonNumerics(System.Object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if ((!char.IsDigit(e.KeyChar)) && (e.KeyChar != '.')) {
				e.Handled = true;
			}

		}

		/// <summary>
		/// Prevents any key from being pressed in a control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		public static void SuppressKeyPressOnKeyDown(System.Object sender, System.Windows.Forms.KeyEventArgs e) {
			e.SuppressKeyPress = true;
		}

	}
}
