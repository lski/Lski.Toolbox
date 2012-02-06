using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Lski.Data.ORM.Schema {

	public interface IFieldSchema {

		ITableSchema ParentTable { get; set; }
		String PropertyName { get; set; }
		String FieldName { get; set; }
		DbType FieldType { get; set; }
		Boolean IsReadOnly { get; set; }
		Boolean IsPrimary { get; set; }
		Boolean IsNullable { get; set; }
		Boolean IsAutoIncrement { get; set; }
		String ParameterName { get; }
		String FullQualifiedName { get; }
		String SqlFieldName { get; }
	}
}
