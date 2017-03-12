using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Toolbox.Txt
{
    /// <summary>
    /// A generator that creates random strings using a character set supplied.
    /// </summary>
    public class RandomString
    {
        public const int DEFAULT_SIZE = 10;

        /// <summary>
        /// Used to prevent the milliseconds being the same through twice because the clock is too fast for the processor
        /// </summary>
        private int _seedCounter;

        /// <summary>
        /// Length of the string to be generated, can be overidden when generating the string
        /// </summary>
        public int Size { get; set; }

        private char[] _characaters;

        /// <summary>
        /// The list of characters that is to be used when creating the random string
        /// </summary>
        public char[] Characters
        {
            get => _characaters;
            set => _characaters = value ?? new char[] { };
        }

        /// <summary>
        /// Creates a random string generator that will be based on the characters supplied
        /// </summary>
        /// <param name="characters">The characters to select from</param>
        public RandomString(IEnumerable<char> characters)
        {
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }

            Init(DEFAULT_SIZE, characters.ToArray());
        }

        /// <summary>
        /// Creates a random string generator that will be based on the characters supplied
        /// </summary>
        /// <param name="characters">The characters to select from</param>
        public RandomString(string characters)
        {
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }

            Init(DEFAULT_SIZE, characters.ToCharArray());
        }

        /// <summary>
        /// Creates a random string generator that will be based on the characters supplied
        /// </summary>
        /// <param name="characters">The characters to select from</param>
        public RandomString(char[] characters)
        {
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }

            Init(DEFAULT_SIZE, characters);
        }

        /// <summary>
        /// Creates a random string generator of the size supplied that will be based on the characters supplied
        /// </summary>
        /// <param name="characters">The characters to select from</param>
        /// <param name="size">The size of the string being output</param>
        public RandomString(int size, string characters)
        {
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }

            Init(size, characters.ToCharArray());
        }

        /// <summary>
        /// Creates a random string generator of the size supplied that will be based on the characters supplied
        /// </summary>
        /// <param name="characters">The characters to select from</param>
        /// <param name="size">The size of the string being output</param>
        public RandomString(int size, char[] characters)
        {
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }

            Init(size, characters);
        }

        /// <summary>
        /// Utility for the contructors to state initial state
        /// </summary>
        /// <param name="size"></param>
        /// <param name="characters"></param>
        protected void Init(int size, char[] characters)
        {
            Size = size;
            Characters = characters;
        }

        /// <summary>
        /// Generates a random string
        /// </summary>
        public string Generate() => _Generate(Characters, Size);

        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="size">Overrides the size property just for this call</param>
        public string Generate(int size) => _Generate(Characters, size);

        /// <summary>
        /// Creates a random ascii string using the options passed. Also offers the ability to exclude certain characters
        /// </summary>
        /// <param name="characters">The list of characters that can be used to </param>
        /// <param name="size">The amount of characters in the generated string</param>
        private string _Generate(char[] characters, int size = 10)
        {
            var code = new StringBuilder(size);
            var rand = new Random((int)(DateTime.Now.Ticks % (int.MaxValue + _seedCounter++)));

            // Cache the count, to avoid recalling it
            var charListCount = characters.Length;

            // Now run through and create the string
            for (var i = 0; i < size; i++)
            {
                code.Append(characters[rand.Next(charListCount)]);
            }

            // If the _seedcounter is above its maxAmount then reset to zero
            _seedCounter = (_seedCounter > 10000 ? 0 : _seedCounter + 3);

            return code.ToString();
        }
    }
}