using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.Common;

namespace Lski.Data.Common {

	public class SqlServer2005Factory : SqlServerFactory {

		public SqlServer2005Factory() 
			: base() {}
		

		public new const String DatabaseName = "SqlServer2005";

		public override string DatabaseType {
			get {
				return DatabaseName;
			}
		}

		public override object Clone() {
			return new SqlServer2005Factory();
		}

		public override string LimitSelect(string selectQry, int start, int size, string orderBy) {
	
			return String.Format(@"
select * from
(
	select a.*, row_number() over(order by {3}) as a_random_row_id_name from
	(
		{2}
	) as a
) as b
where a_random_row_id_name > {0} and a_random_row_id_name < ({1} + {0})", (start < 0 ? 0 : start), (size < 0 ? Int32.MaxValue : size), selectQry, orderBy);

		}
	}

}
