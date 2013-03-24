using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Drawing {

    /// <summary>
    /// Represents a line (using decimal values) on a drawing pane. Can be used to simply represent two points.
    /// </summary>
	public class LineF {

		public System.Drawing.PointF PointOne { get; set; }
		public System.Drawing.PointF PointTwo { get; set; }

		public LineF(System.Drawing.Point p1, System.Drawing.Point p2) {
			PointOne = p1;
			PointTwo = p2;
		}

		public LineF(System.Drawing.PointF p1, System.Drawing.PointF p2) {
			PointOne = p1;
			PointTwo = p2;
		}

		public LineF(Int32 x1, Int32 y1, Int32 x2, Int32 y2) {
			PointOne = new System.Drawing.Point(x1, y1);
			PointTwo = new System.Drawing.Point(x2, y2);
		}

		public LineF(Single x1, Single y1, Single x2, Single y2) {
			PointOne = new System.Drawing.PointF(x1, y1);
			PointTwo = new System.Drawing.PointF(x2, y2);
		}

		public Single Width {
			get {
				return (this.PointOne.X - this.PointTwo.X) * -1;
			}
		}
	}
}
