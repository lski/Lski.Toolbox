using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Lski.Objects {

	/// <summary>
	/// Simple variable class, works as a wrapper to any standard variable, but offers events to inform of a change of value OR to inform that a value will change and can be cancelled
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Variable<T> {

		private T _value;

		#region "Events"

		public event ValueChangingEventHandler ValueChanging;
		public delegate void ValueChangingEventHandler(object sender, ValueChangingArgs e);
		public event ValueChangedEventHandler ValueChanged;
		public delegate void ValueChangedEventHandler(object sender, ValueChangedArgs e);

		#endregion

		#region "Constructors"

		/// <summary>
		/// Creates a new variable object with a null internal value assigned. Does NOT raise any events. NOTE: If a scalar type (or Date)
		/// it will hold the default for that type, rather than null. E.g. Int32 = 0 and String = Nothing
		/// </summary>
		/// <remarks></remarks>
		public Variable() {	_value = default(T); }

		/// <summary>
		/// Creates a new variable object assigning value passed to the object. Does NOT raise any events
		/// </summary>
		/// <param name="value"></param>
		/// <remarks></remarks>
		public Variable(T value) { _value = value; }

		#endregion

		#region "Properties"

		/// <summary>
		/// Holds the internal value of the variable, prior to changing it throws a new value changing event, that can be used to
		/// cancel the change and throws a value changed event when the value has in fact been changed.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public T Value {
			get { return _value; }
			set {

				if (!value.Equals(_value)) {
					ValueChangingArgs args = new ValueChangingArgs(_value, value);

					if (ValueChanging != null) 	ValueChanging(this, args);

					// Only update if the user doesnt want to cancel
					if (!args.Cancel) {
						
						_value = value;
						if (ValueChanged != null) ValueChanged(this, new ValueChangedArgs(_value));
					}

				}

			}
		}

		#endregion

		#region "Inner Classes - Event Args"

		public class ValueChangedArgs : System.EventArgs {

			private T _value;

			public ValueChangedArgs(T value) { _value = value; }
			public T Value { get { return _value; } }
		}


		public class ValueChangingArgs : System.EventArgs {

			private T _oldValue;
			private T _newValue;
			private bool _cancel;

			public ValueChangingArgs(T oldValue, T newValue) {
				_oldValue = oldValue;
				_newValue = newValue;
				_cancel = false;
			}

			/// <summary>
			/// States whether one of the subscribers has said to cancel the value from changing
			/// </summary>
			public bool Cancel {
				get { return _cancel; }
				set { _cancel = value; }
			}

			/// <summary>
			/// The original value prior to changing
			/// </summary>
			public T OldValue { get { return _oldValue; } }

			/// <summary>
			/// The new value to replace the original value
			/// </summary>
			public T NewValue {	get { return _newValue; } }

		}

		#endregion

	}

}

