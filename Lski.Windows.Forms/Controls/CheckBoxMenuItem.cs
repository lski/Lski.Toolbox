using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
namespace Lski.Windows.Forms.Controls
{

	/// <summary>
	/// Simply a normal menuitem, that toggles being 'ticked' when clicked
	/// </summary>
	/// <remarks></remarks>
	public class CheckBoxMenuItem : System.Windows.Forms.ToolStripMenuItem
	{

		public CheckBoxMenuItem() : base()
		{
			Click += CheckBoxMenuItem_Click;
		}

		public CheckBoxMenuItem(string text) : base(text)
		{
			Click += CheckBoxMenuItem_Click;
		}

		private void CheckBoxMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Checked = !(this.Checked);
		}
	}

}
