using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using System.Data.OleDb;

namespace Lski.Data.Common {

	public class FoxProFactory : OleFactory {

		public FoxProFactory() 
			: base() { }
		
		public new const string DatabaseName = "FoxPro";

		public override string DatabaseType {
			get { return DatabaseName; }
		}

		#region "Methods"

		/// <summary>
		/// Returns the passed date with the correct formatting for inserting it into a database
		/// </summary>
		/// <param name="dat">The date as a string that you want to insert into the database</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public override string ToSqlDate(string dat) { 
			return "{^" + dat + "}"; 
		}
		public override string ToSqlDateTime(string dat) { 
			return "{^" + dat + "}"; 
		}
		public override string ToSqlFieldName(string fieldName) {
			return fieldName;
		}

		#endregion

	}

}
