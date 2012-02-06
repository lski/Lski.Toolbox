using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Lski.Web {
	public class WebEvents {

		/// <summary>
		/// An event that when the pager of a gridView is loaded, that it added a page of total pages indicator into that pager row
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void GridViewRowCompleted(object sender, GridViewRowEventArgs e) {

			var gv = (sender as GridView);

			if (e.Row.RowType == DataControlRowType.Pager) {

				var lbl = new Label();
				lbl.Text = (gv.PageIndex + 1) + " of " + gv.PageCount;
				e.Row.Cells[0].Controls.Add(lbl);
			}

		}

	}
}
