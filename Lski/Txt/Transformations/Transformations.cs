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

		public Transformations() {}

		public Transformations(params Transformation[] trans) {
			
			foreach (var tran in trans) {
				this.Add(tran);
			}
		}

		public string Process(string value) {

			foreach (var i in this) {
				value = i.Process(value);
			}

			return value;
		}

		public object Clone() {

			Transformations avc = new Transformations();

			foreach (var i in this) {
				avc.Add((Transformation)i.Clone());
			}

			return avc;
		}

	}
}