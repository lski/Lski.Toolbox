using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Lski.Data.ORM.DataObjects {

	public enum DataObjectState {

		Unchanged = DataRowState.Unchanged,
		Modified = DataRowState.Modified,
		Added = DataRowState.Added,
		Deleted = DataRowState.Deleted,
		ToDelete
	}
}
