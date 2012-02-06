using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Lski.Web.WebForms.Extensions {
	
	public static class TreeViewExt {
		
		/// <summary>
		/// Removes all the nodes from the tree
		/// </summary>
		/// <param name="tree"></param>
		public static void Clear(this TreeView tree) {

			for(Int32 i = tree.Nodes.Count - 1, n = 0; i >= n; i--) {
				tree.Nodes.RemoveAt(i);
			}
		}
	}
}
