using System;
using System.Collections.Generic;
using System.Linq;

namespace Lski.Toolbox.Objects {

	[Obsolete("Deprecated: Use the Lski.Toolbox.Collection namespace instead, it will be removed in a future version")]
	public static class IEnumerableExtensions {

		[Obsolete("Deprecated: Use the Lski.Toolbox.Collection namespace instead, it will be removed in a future version")]
        public static T MaxOrDefault<T>(this IEnumerable<T> e) {
			return Collections.IEnumerableExtensions.MaxOrDefault(e);
        }

		[Obsolete("Deprecated: Use the Lski.Toolbox.Collection namespace instead, it will be removed in a future version")]
		public static T MinOrDefault<T>(this IEnumerable<T> e) {
			return Collections.IEnumerableExtensions.MinOrDefault(e);
		}
    }
}