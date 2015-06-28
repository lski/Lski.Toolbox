using System;
using System.Collections;

namespace Lski.Toolbox.Objects {

	[Obsolete("Deprecated: Use the Lski.Toolbox.Collection namespace instead, it will be removed in a future version")]
	public static class IListExtensions {

		[Obsolete("Deprecated: Use the Lski.Toolbox.Collection namespace instead, it will be removed in a future version")]
		public static IList MoveUp(this IList list, Int32 index) {
			return Collections.IListExtensions.MoveUp(list, index);
		}

		[Obsolete("Deprecated: Use the Lski.Toolbox.Collection namespace instead, it will be removed in a future version")]
		public static IList MoveDown(this IList list, Int32 index) {
			return Collections.IListExtensions.MoveDown(list, index);
		}

		[Obsolete("Deprecated: Use the Lski.Toolbox.Collection namespace instead, it will be removed in a future version")]
		public static IList MoveFirst(this IList list, Int32 index) {
			return Collections.IListExtensions.MoveFirst(list, index);
		}

		[Obsolete("Deprecated: Use the Lski.Toolbox.Collection namespace instead, it will be removed in a future version")]
		public static IList MoveEnd(this IList list, Int32 index) {
			return Collections.IListExtensions.MoveEnd(list, index);
        }
    }
}