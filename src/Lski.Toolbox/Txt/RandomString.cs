using System;
using System.Linq;
using System.Text;

namespace Lski.Toolbox.Txt {

	public class RandomString {

		/// <summary>
		/// Used to prevent the milliseconds being the same through twice because the clock is too fast for the processor
		/// </summary>
		private Int32 _seedCounter = 0;

		/// <summary>
		/// Length of the string to be generated, can be overidden when generating the string
		/// </summary>
		public int Size { get; set; }

		private char[] _characaters;

		/// <summary>
		/// The list of characters that is to be used when creating the 
		/// </summary>
		public char[] Characters {
			get {
				return _characaters ?? (_characaters = RandomStringHelper.All.ToCharArray());
			}
			set {
				_characaters = value;
			}
		}

		public RandomString() 
			: this(10) {
		}

		public RandomString(int size) 
			: this(size, RandomStringHelper.All) {
		}

		public RandomString(int size, string characters) 
			: this(size, characters.ToCharArray()) {
		}

		public RandomString(int size, char[] characters) {

			Size = size;
			Characters = characters;
		}

		/// <summary>
		/// Generates a random string
		/// </summary>
		public string Generate() {
			return _Generate(Characters, Size);
		}

		/// <summary>
		/// Generates a random string
		/// </summary>
		/// <param name="size">Overrides the size property just for this call</param>
		/// <returns></returns>
		public string Generate(Int32 size) {
			return _Generate(Characters, size);
		}

		/// <summary>
		/// Creates a random ascii string using the options passed. Also offers the ability to exclude certain characters
		/// </summary>
		/// <param name="characters">The list of characters that can be used to </param>
		/// <param name="size">The amount of characters in the generated string</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string _Generate(char[] characters, Int32 size = 10) {

			var code = new StringBuilder(size);
			var rand = new Random((int)(DateTime.Now.Ticks % (Int32.MaxValue + _seedCounter++)));

			// Cache the count, to avoid recalling it
			var charListCount = characters.Count();

			// Now run through and create the string
			for (var i = 0; i < size; i++) {
				code.Append(characters[rand.Next(charListCount)]);
			}

			// If the _seedcounter is above its maxAmount then reset to zero
			_seedCounter = (_seedCounter > 10000 ? 0 : _seedCounter + 3);

			return code.ToString();
		}
	}
}