using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Lski.Toolbox.Txt;
using System.Text;

namespace Lski.Toolbox.Data {

	/// <summary>
	/// Takes a string that it can attempt to breakdown into parts so that it can be used to provide multiple order bys. Also accepts a mapping dictionary
	/// for on-the-fly name conversions (The mapping is used to direct transistion between object properties and Database column names that might be different)
	/// </summary>
	public class SortExpression : List<SortExpression.SortData> {


		public string ParsedExpression {

			get {
				StringBuilder sb = new StringBuilder();

				foreach (var sortItem in this) {
					sb.Append(sortItem.OriginalFieldName).Append(" ").Append(sortItem.Direction).Append(",");
				}

				if (sb.Length > 0) 
					return sb.ToString(0, sb.Length - 1);

				return "";
			}
		}

		public string MappedExpression {

			get {
				StringBuilder sb = new StringBuilder();

				foreach (var sortItem in this) {
					sb.Append(sortItem.ToString()).Append(",");
				}

				if (sb.Length > 0) 
					return sb.ToString(0, sb.Length - 1);

				return "";

			}
		}

		private string _originalExpression;
		/// <summary>
		/// The original un altered expression (usually containing the object property names prior to mapping)
		/// </summary>
		public string OriginalExpression {
			get { return _originalExpression; }
			set { _originalExpression = value; }
		}

		/// <summary>
		/// There is only sort and its already calculated, so add it to the list.
		/// </summary>
		/// <param name="sortData"></param>
		public SortExpression(SortData sortData) {
			this.Add(sortData);
		}

		/// <summary>
		/// Just takes the expression, without any mapping and splits it into its different parts
		/// </summary>
		/// <param name="originalExpression"></param>
		public SortExpression(string originalExpression) {
			this.Init(originalExpression, null);
		}

		/// <summary>
		/// Takes both the original expression and a mapping for converting the property names (key) to original data source names (values)
		/// </summary>
		/// <param name="originalExpression"></param>
		/// <param name="propertyNameMappings"></param>
		public SortExpression(string originalExpression, Dictionary<string, string> propertyNameMappings) {
			this.Init(originalExpression, propertyNameMappings);
		}


		protected void Init(string originalExpression, Dictionary<string, string> propertyNameMappings) {
			_originalExpression = originalExpression.Trim();
			this.ParseExpression(propertyNameMappings);

		}

		/// <summary>
		/// Takes the original expression and converts it so that it changes any properties to field names in the data source
		/// </summary>
		/// <param name="propertyNameMappings"></param>
		protected void ParseExpression(Dictionary<string, string> propertyNameMappings) {

			if (string.IsNullOrEmpty(this.OriginalExpression)) 
				return;

			String[] parts = this._originalExpression.Split(',');
			string lowerCaseVersion = null;
			string direction = null;
			string mappedFieldName = null;
			string part = null;
			Int32 i = 0;
			Int32 n = parts.Count();

			while (i < n) {

				// Reset up variables
				part = parts[i].Trim();
				lowerCaseVersion = part.ToLowerInvariant();

				// If ends with the letters desc remove and store

				if (lowerCaseVersion.EndsWith(" desc")) {

					part = part.SubStringAdv(0, part.Length - 5).Trim();
					lowerCaseVersion = lowerCaseVersion.SubStringAdv(0, lowerCaseVersion.Length - 5).Trim();
					direction = "DESC";

				} else {

					if (lowerCaseVersion.EndsWith(" asc")) {
						part = part.SubStringAdv(0, part.Length - 4).Trim();
						lowerCaseVersion = lowerCaseVersion.SubStringAdv(0, lowerCaseVersion.Length - 4).Trim();
					} else {
						part = part.Trim();
						lowerCaseVersion = lowerCaseVersion.Trim();
					}

					direction = "ASC";
				}

				// If there is a field name mapping object then use it to add the mapping

				if (propertyNameMappings != null) {

					// Reset the mapping field, as this will simply remain nothing if not running this code section
					mappedFieldName = null;

					// Loop through the mappings to see if one of them matches this item

					foreach (var mappingItem in propertyNameMappings) {

						if (string.Compare(lowerCaseVersion, mappingItem.Key.ToLowerInvariant()) == 0) {

							mappedFieldName = mappingItem.Value;
							break;
						}
					}

				}

				// Add the new sort part
				this.Add(new SortData(part, mappedFieldName, direction));

				// Iterate to the next item in parts
				i += 1;

			}


		}

		#region "Inner Classes"

		/// <summary>
		/// The class designed for indvidual sorts, including original (with property names) and mapped (with the field names)
		/// </summary>
		public class SortData {


			public SortData(string originalFieldName, string direction) {
				this.OriginalFieldName = originalFieldName;
				this.Direction = direction;

			}


			public SortData(string originalFieldName, string mappedFieldName, string direction) {
				this.OriginalFieldName = originalFieldName;
				this.MappedFieldName = mappedFieldName;
				this.Direction = direction;

			}

			private string _originalFieldName;
			public string OriginalFieldName {
				get { return _originalFieldName; }
				set { _originalFieldName = value; }
			}

			private string _mappedFieldName;
			public string MappedFieldName {
				get {
					if (string.IsNullOrEmpty(_mappedFieldName))
						return this.OriginalFieldName;
					return _mappedFieldName;
				}
				set { _mappedFieldName = value; }
			}

			private string _direction;
			public string Direction {
				get { return _direction; }
				set { _direction = value; }
			}

			public override string ToString() {
				return this.MappedFieldName + " " + this.Direction;
			}

		}

		#endregion

	}

}
