namespace Lski.Toolbox.Objects {

    /// <summary>
    /// Wrapper for an object of type T, which fires events when the value changes. Fires a changing event as well as changed event that can cancel the changing of the value.
    /// </summary>
    public class Observable<T> {

        private T _value;

        public event ValueChangingEventHandler Changing;

        public delegate void ValueChangingEventHandler(object sender, ChangingArgs e);

        public event ValueChangedEventHandler Changed;

        public delegate void ValueChangedEventHandler(object sender, ChangedArgs e);

        public static implicit operator T(Observable<T> value) {
            return value.Value;
        }

        public static implicit operator Observable<T>(T value) {
            return new Observable<T>(value);
        }

        /// <summary>
        /// Creates a new variable object with a null internal value assigned. Does NOT raise any events. NOTE: If a scalar type (or Date)
        /// it will hold the default for that type, rather than null. E.g. Int32 = 0 and String = Nothing
        /// </summary>
        /// <remarks></remarks>
        public Observable() {
            _value = default(T);
        }

        /// <summary>
        /// Creates a new variable object assigning value passed to the object. Does NOT raise any events
        /// </summary>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public Observable(T value) {
            _value = value;
        }

        /// <summary>
        /// Holds the internal value of the variable, prior to changing it throws a new value changing event, that can be used to
        /// cancel the change and throws a value changed event when the value has in fact been changed.
        /// </summary>
        public T Value {
            get {
                return _value;
            }
            set {

                if (!value.Equals(_value)) {

                    var args = new ChangingArgs(_value, value);

                    if (Changing != null) {
                        Changing(this, args);
                    }

                    // Only update if the subscriber doesnt want to cancel
                    if (!args.Cancel) {

                        if (Changed != null) {
                            Changed(this, args);
                        }

                        _value = value;
                    }
                }
            }
        }

        public class ChangedArgs : System.EventArgs {

            private readonly T _value;
            private readonly T _original;

            public ChangedArgs(T original, T value) {

                _value = value; 
                _original = original;
            }

            public T Value { get { return _value; } }

            /// <summary>
            /// The original value prior to changing
            /// </summary>
            public T Original { get { return _original; } }
        }

        public class ChangingArgs : ChangedArgs {

            public ChangingArgs(T original, T value) : base(original, value) {

                Cancel = false;
            }

            /// <summary>
            /// States whether one of the subscribers has said to cancel the value from changing
            /// </summary>
            public bool Cancel { get; set; }
        }

    }
}