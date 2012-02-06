using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lski.IO {

	[DataContract(Namespace="")]
	public abstract class DataMap {

		private List<DataMapLink> _DataMapLinks;
		/// <summary>
		/// The data links between the source and the the target, and the tranformations that happen in between.
		/// </summary>
		/// <remarks>Not included within the DataContract, this is so it can be alias'ed in the subclass and included in the serialisation WITH ORDER there, because
		/// if ordered her it will always appear first
		/// </remarks>
		public List<DataMapLink> DataMapLinks {
			get { return _DataMapLinks; }
			set { _DataMapLinks = value; }
		}

		public DataMap() {
			this._DataMapLinks = new List<DataMapLink>(0);
		}
	}
}
