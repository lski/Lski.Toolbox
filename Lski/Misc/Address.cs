using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Lski.Txt;

namespace Lski.Misc {
	
    /// <summary>
    /// A class representing a basic postal address.
    /// </summary>
	[DataContract()]
	public class Address {

		[DataMember()]
		public String Name { get; set; }
		[DataMember()]
		public String LineOne { get; set; }
		[DataMember()]
		public String LineTwo { get; set; }
		[DataMember()]
		public String LineThree { get; set; }
		[DataMember()]
		public String City { get; set; }
		[DataMember()]
		public String County { get; set; }
		[DataMember()]
		public String Postcode { get; set; }

        /// <summary>
        /// Returns the full address with a newline separate. Uses StringExt.ConcatWS to combine, therefore if any field is null/empty a separator is not added.
        /// </summary>
        /// <returns></returns>
		public String ToFullAddress() {

			return StringExt.ConcatWS(Environment.NewLine,
									  Name,
									  LineOne,
									  LineTwo,
									  LineThree,
									  City,
									  County,
									  Postcode); 
		}

        /// <summary>
        /// Simply returns the full address
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.ToFullAddress();
        }
	}
}
