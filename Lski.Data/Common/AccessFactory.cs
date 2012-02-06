using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lski.Data.Common;

namespace Lski.Data.Common {

	public class AccessFactory : OleFactory {

		public AccessFactory() : base() { }

		public new const string DatabaseName = "Access";

		public override string DatabaseType {
			get { return DatabaseName; }
		}

		public override bool SupportsMars {
			get { return false; }
		}

		/// <summary>
		/// The sql server style is supported by Access so return to that style over the strict ole style
		/// </summary>
		protected override string ParameterChar {
			get { return "@"; }
		}

		/// <summary>
		/// Although access uses an OLE provider, it does also support the much more readable sql server syntax, which is @paramName, same as
		/// the methods ToParameterName
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public override string ToParameterPlaceholder(string name) {
			return this.ToParameterName(name);
		}
	}
}
