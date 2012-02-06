using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Lski.Txt;
using System.Windows.Forms;

namespace Lski.Windows.Forms.Controls
{

	/// <summary>
	/// Extends the toolstripmenuitem so that instead of storing simply a string object to display to the user, it can hold anything.
	/// </summary>
	/// <remarks>Extends the toolstripmenuitem so that instead of storing simply a string object to display to the user, it can hold anything.
	/// 
	/// Note: The text display is then created from the 'ToString' method of that object.
	/// </remarks>
	public class ObjectMenuItem : ToolStripMenuItem
	{


		protected object _value;
		#region "Constructors"

		public ObjectMenuItem() : base()
		{
		}

		public ObjectMenuItem(object value) : base()
		{
			this.Value = value;
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// Holds the object that is used to display the text.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public object Value {
			get { return _value; }
			set { _value = value; }
		}

		public override string Text {
			get { return (Value).ToString(); }
			set { _value = value; }
		}

		#endregion

	}

}


