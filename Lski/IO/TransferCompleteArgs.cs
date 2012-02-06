using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO {

	public class TransferCompleteArgs : ValueChangedArgs {

		/// <summary>
		/// Creates a new event arguement object which contains the information raised by the event
		/// </summary>
		/// <param name="quantity">The amount imported/exported during the event</param>
		/// <param name="type">States the type of value change, either imported or exported</param>
		/// <remarks></remarks>
		public TransferCompleteArgs(Int32 quantity, ValueType type) : base(quantity, type) {}
	}
}
