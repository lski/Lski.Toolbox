using Lski.Toolbox.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lski.Toolbox.Txt {

	/// <summary>
	/// Used to create a list of characters that meet a certain criteria, that can be used with RandomString.
	/// </summary>
	public static class CharacterList {

		/// <summary>
		/// Creates a char list containing characters that match the types requested. Also has to option to exclude specific characters from the generated list
		/// </summary>
		/// <param name="characters">A selection of <see cref="CharacterListOptions"/> that states what to include. E.g. UppercaseLetters or UppercaseLetters and Numerics etc</param>
		/// <param name="exclude">An optional enumerable list of characters to exclude from the final output.</param>
		/// <returns></returns>
		public static char[] Generate(CharacterListOptions characters, string exclude) {

			return Generate(characters, exclude.ToCharArray());
		}

		/// <summary>
		/// Creates a char list containing characters that match the types requested. Also has to option to exclude specific characters from the generated list
		/// </summary>
		/// <param name="characters">A selection of <see cref="CharacterListOptions"/> that states what to include. E.g. UppercaseLetters or UppercaseLetters and Numerics etc</param>
		/// <param name="exclude">An optional enumerable list of characters to exclude from the final output.</param>
		/// <returns></returns>
		public static char[] Generate(CharacterListOptions characters, IEnumerable<char> exclude) {

			return Generate(characters, exclude.ToArray());
		}

		/// <summary>
		/// Creates a char list containing characters that match the types requested. Also has to option to exclude specific characters from the generated list
		/// </summary>
		/// <param name="characters">A selection of <see cref="CharacterListOptions"/> that states what to include. E.g. UppercaseLetters or UppercaseLetters and Numerics etc</param>
		/// <param name="exclude">An optional enumerable list of characters to exclude from the final output.</param>
		/// <returns></returns>
		public static char[] Generate(CharacterListOptions characters = CharacterListOptions.All, char[] exclude = null) {

			// Create a list of all the ascii characters that are allowed, at first ignoring the chars to exclude
			var list = Enumerable.Empty<char>();

			exclude = exclude ?? new char[] { };

			// 65 -> 90
			if (characters.Has(CharacterListOptions.UppercaseLetters)) {

				list = list.Concat(from s in CharacterListPredefined.UppercaseLetters where !exclude.Contains(s) select s);
			}

			// 97 -> 122
			if (characters.Has(CharacterListOptions.LowercaseLetters)) {

				list = list.Concat(from s in CharacterListPredefined.LowercaseLetters where !exclude.Contains(s) select s);
			}

			// 48 -> 57
			if (characters.Has(CharacterListOptions.Numbers)) {

				list = list.Concat(from s in CharacterListPredefined.Numbers where !exclude.Contains(s) select s);
			}

			// 45
			if (characters.Has(CharacterListOptions.Hyphens)) {

				if (!exclude.Contains('-')) {
					list = list.Concat(new[] { '-' });
				}
			}

			// 95
			if (characters.Has(CharacterListOptions.Underscores)) {

				if (!exclude.Contains('_')) {
					list = list.Concat(new[] { '_' });
				}
			}

			if (characters.Has(CharacterListOptions.OtherPunctuation)) {

				list = list.Concat(from s in CharacterListPredefined.OtherPunctuation where !exclude.Contains(s) select s);
			}

			return list.ToArray();
		}
	}
}