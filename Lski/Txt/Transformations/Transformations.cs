using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;


namespace Lski.Txt.Transformations {

	public class Transformations : List<Transformation>, ICloneable {

		public Transformations() { }

		public string Process(string value) {

			foreach (var i in this) {
				value = i.Process(value);
			}

			return value;
		}

		public object Clone() {

			TransformValues avc = new TransformValues();

			foreach (var i in this) {
				avc.Add((Transformation)i.Clone());
			}

			return avc;
		}

	}

	//[DataContract()]
	//public class TransformValues : ICloneable {

	//    private List<TransformValue> _Translations;
	//    /// <summary>
	//    /// Holds the list of translations for the passed value in the Process method.
	//    /// </summary>
	//    /// <remarks>
	//    /// Holds the list of translations for the passed value in the Process method. Held in a property rather than overriden collection
	//    /// to provide a cleaner xml export
	//    /// </remarks>
	//    [DataMember()]
	//    public List<TransformValue> Translations {
	//        get { return _Translations; }
	//        set { _Translations = value ?? new List<TransformValue>(); } 
	//    }

	//    public TransformValues() {
	//        Translations = new List<TransformValue>();
	//    }

	//    public string Process(string value) {

	//        foreach (var i in this.Translations) {
	//            value = i.Process(value);
	//        }

	//        return value;
	//    }

	//    public Int32 Add(TransformValue tv) {
	//        this.Translations.Add(tv);
	//        return this.Translations.Count - 2;
	//    }

	//    public object Clone() {

	//        TransformValues avc = new TransformValues();

	//        foreach (var i in this.Translations) {
	//            avc.Translations.Add((TransformValue)i.Clone());
	//        }

	//        return avc;
	//    }

	//}
}