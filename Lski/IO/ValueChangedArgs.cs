using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Lski.IO {

	public class ValueChangedArgs : System.EventArgs {

		public enum ValueType {
			Imported = 1,
			Exported = 2,
			Transfer = 4
		}

		private ValueType _type;

		private Int32 _quantity;
		/// <summary>
		/// Creates a new event arguement object which contains the information raised by the event
		/// </summary>
		/// <param name="quantity">The amount imported/exported during the event</param>
		/// <param name="type">States the type of value change, either imported or exported</param>
		/// <remarks></remarks>

		public ValueChangedArgs(Int32 quantity, ValueType type) {
			_type = type;
			_quantity = quantity;

		}

		/// <summary>
		/// Gets the amount imported/exported during the event
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public Int32 Quantity {
			get { return _quantity; }
			set { _quantity = value; }
		}

		/// <summary>
		/// Returns the type of value change this is, e.g. Imported or Exported
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public ValueType Type {
			get { return _type; }
			set { _type = value; }
		}

	}

}
