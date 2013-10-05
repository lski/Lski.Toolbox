using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Lski.Toolbox.Data.Extensions {

	/// <summary>
	/// A module designed to add functions to the DataTable class
	/// </summary>
	/// <remarks></remarks>
	public static class DataTableExt {

		/// <summary>
		/// Imports a row from the passed row into the currently held dataTable, as a new DataRow. It will only add the value if a column 
		/// with a matching column name is in both rows.
		/// </summary>
		/// <param name="rowToImport">The datarow to be imported</param>
		/// <returns></returns>
		/// <remarks>Imports a row from the passed row into the currently held dataTable, as a new DataRow. It will only add the value if a column 
		/// with a matching column name is in both rows.
		/// 
		/// During the import the datatType of the columns with matching names are NOT checked and will therefore throw an error if they are not
		/// compatible. It does do a DBNull check however. If a column allows DBNull then regardless it will try and copy the value, if the 
		/// column doesnt allow DBNull values however it will only copy its value if the imported value is NOT null.
		/// 
		/// If there is an error uploading a particular value to a particular column in the row, that error is thrown in the normal
		/// behavior way.
		/// 
		/// NOTE: leaves the row in RowState = Added
		/// </remarks>
		public static DataRow ImportNewRow(this DataTable dataTable, ref DataRow rowToImport) {

			DataColumn rowToImportCol = null;
			DataColumnCollection rowToImportColumns = rowToImport.Table.Columns;

			DataRow newRow = dataTable.NewRow();
			DataColumnCollection newRowColumns = dataTable.Columns;

			// Loop through each of the columns in the newRow which has been created for this DataTable
			foreach (DataColumn newRowCol in newRowColumns) {

				// Check the importedRow has a relevant column then try to add the value to it
				if (rowToImportColumns.Contains(newRowCol.ColumnName)) {
					
					rowToImportCol = rowToImportColumns[newRowCol.ColumnName];

					if (newRowCol.AllowDBNull || !DBNull.Value.Equals(rowToImport[rowToImportCol]))
						newRow[newRowCol.ColumnName] = rowToImport[newRowCol.ColumnName];
				}

			}

			dataTable.Rows.Add(newRow);

			return newRow;
		}

		/// <summary>
		/// Ensures the column name passed is in fact unique for this table. If a column of that name already exists, it will add an underscore followed by
		/// a version number. This loops until a unique column name is produced and returns it.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static String UniqueColumnName(this DataTable table, String columnName) {

			if (!table.Columns.Contains(columnName))
				return columnName;

			var i = 1;

			while (Convert.ToBoolean(i++)) {

				var newColumnName = columnName + "_" + i.ToString();

				if (!table.Columns.Contains(newColumnName))
					return newColumnName;
			}

			return null;
		}

		/// <summary>
		/// Returns a list of column names from the currently held data table (No additional calls to the database)
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public static IEnumerable<string> ColumnNames(this DataTable dataTable) {

			foreach (DataColumn col in dataTable.Columns) {
				yield return col.ColumnName;
			}
		}

		/// <summary>
		/// Returns an array list of each value stored in the specified column within the current dataTable.
		/// </summary>
		/// <param name="column"></param>
		/// <exception cref="TableColumnNotFoundException">Returned when the specified column is not found in the
		/// current table.</exception>
		/// <returns></returns>
		/// <remarks></remarks>
		public static IEnumerable<object> ColumnToArrayList(this DataTable dataTable, Int32 column) {

			if (dataTable.Columns.Count > column || column < 0)	
				throw new ArgumentException(String.Format("Column {0} was requested from table {1} which only has {1} columns", column, dataTable.TableName, dataTable.Columns.Count));

			foreach (DataRow row in dataTable.Rows) {
				yield return row[column];
			}

		}

		/// <summary>
		/// Returns an array list of each value stored in the specified column within the current dataTable.
		/// </summary>
		/// <param name="columnName"></param>
		/// <exception cref="TableColumnNotFoundException">Returned when the specified column is not found in the
		/// current table.</exception>
		/// <returns></returns>
		/// <remarks></remarks>
		public static IEnumerable<object> ColumnToArrayList(this DataTable dataTable, string columnName) {

			if (!(dataTable.Columns.Contains(columnName)))
				throw new ArgumentException(String.Format("Column {0} does not exist in table {1}",columnName, dataTable.TableName));

			foreach (DataRow row in dataTable.Rows) {
				yield return row[columnName];
			}

		}

		/// <summary>
		/// Appends the rows from the dataTable object passed into the current table. This method can be used
		/// instead of merge as it does not view the primary key value.
		/// </summary>
		/// <param name="tableToImport">Tha tableControl ovejct whose rows are added to the current table</param>
		/// <exception cref="Exception">When a type of value is attempted to be added to a row position whose type it
		/// can not be cast to.</exception>
		/// <remarks>Appends the rows from the table in the TableControl object passed into the current table.
		/// 
		/// When attempting to add each new row it compares to the column names of the table being imported to the
		/// column names of the currently held table. A new row is created and the values are assigned where the names 
		/// of the columns match.
		/// </remarks>
		public static void AppendRows(this DataTable dataTable, ref DataTable tableToImport) {

			// Loop through each row in tableFrom and add it to tableTo using TableControl.ImportRow rather than dataTable.insertRow
			foreach (DataRow row in tableToImport.Rows) {
				dataTable.ImportRow(row);
			}

		}

	}

}
