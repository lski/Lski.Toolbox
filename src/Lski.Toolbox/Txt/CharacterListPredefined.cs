using System;

namespace Lski.Toolbox.Txt {

	/// <summary>
	/// A selection of predfined character lists
	/// </summary>
	public static class CharacterListPredefined {

		/// <summary>
		/// Uppercase letters
		/// </summary>
		public const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Lowercase letters
		/// </summary>
		public const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

		/// <summary>
		/// Numbers 0-9
		/// </summary>
		public const string Numbers = "0123456789";

		/// <summary>
		/// Underscores
		/// </summary>
		public const string Underscores = "_";

		/// <summary>
		/// Hyphens
		/// </summary>
		public const string Hyphens = "-";

		/// <summary>
		/// Other punctuation not typically included in a random string
		/// </summary>
		public const string OtherPunctuation = @"{}[]()/\'""`~,;:.<>";

		/// <summary>
		/// Characters that look very similar so would sometimes be useful to remove to avoid confusion
		/// </summary>
		public const string SimilarCharacters = @"iIlLoO10";

		/// <summary>
		/// Include both lower and upper case letters
		/// </summary>
		public const string Letters = UppercaseLetters + LowercaseLetters;

		/// <summary>
		/// Include all upper and lower case letters and numbers
		/// </summary>
		public const string AlphaNumerics = (Letters + Numbers);

		/// <summary>
		/// Include all options
		/// </summary>
		public const string All = (AlphaNumerics + Underscores + Hyphens + OtherPunctuation);
	}
}