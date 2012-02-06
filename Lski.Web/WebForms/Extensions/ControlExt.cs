using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Lski.Web.WebForms.Extensions {
	
	public static class ControlExt {

		/// This was taken from http://leadingthenextinquisition.wordpress.com/2006/08/07/c-custom-findcontrol-implementation/
		/// 
		/// Has some adaptations by extending the control itself and allowing for searches on that controls sub controls
		/// </summary>
		/// <param name="id"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public static Control FindControlAdv(this Control control, string id) {
			
			foreach (Control c in control.Controls) {
			
				Control child = FindControlRecursive(c, id);

				if (child != null)
					return child;
			}

			return null;
		}

		/// <summary>
		/// The recursive helper method for FindControl
		/// </summary>
		/// <param name="root"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		private static Control FindControlRecursive(Control root, string id) {

			if (root.ID != null && root.ID == id)
				return root;

			foreach (Control c in root.Controls) {

				Control rc = FindControlRecursive(c, id);

				if (rc != null)
					return rc;
			}

			return null;
		}

		/// <summary>
		/// Returns the a list of all controls, based on the type rather than ID. Based on code http://leadingthenextinquisition.wordpress.com/2006/08/07/custom-findcontrol-implementation-c-part-ii/
		/// 
		/// Has some adaptations by extending the control itself and allowing for searches on that controls sub controls
		/// </summary>
		/// <param name="type"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public static List<Control> FindControlsAdv(this Control control, Type type) {

			List<Control> list = new List<Control>();

			foreach (Control c in control.Controls) {

				if (c.GetType() == type)
					list.Add(c);
				else
					FindControlsRecursive(c, type, ref list);
			}
			return list;
		}

		/// <summary>
		/// Helper function for FindControls
		/// </summary>
		/// <param name="root"></param>
		/// <param name="type"></param>
		/// <param name="list"></param>
		private static void FindControlsRecursive(Control root, Type type, ref List<Control> list) {

			if (root.Controls.Count != 0) {

				foreach (Control c in root.Controls) {

					if (c.GetType() == type)
						list.Add(c);
					else if (c.HasControls())
						FindControlsRecursive(c, type, ref list);
				}
			}
		}
	}
}
