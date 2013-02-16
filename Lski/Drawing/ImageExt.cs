using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace Lski.Drawing {
	public static class ImageExt {

		static ImageExt() {
			_resizer = new ImageResizer();
		}

		static ImageResizer _resizer { get; set; }
		
		public static Image Resize(this Image image, int size) {
			return _resizer.Resize(image, size);
		}

		public static Image Resize(this Image image, Size size, bool preserveAspectRatio = true) {
			return _resizer.Resize(image, size, preserveAspectRatio);
		}

		public static ImageFormat SaveFormat(this Image img) {
			return _resizer.SaveFormat(img);
		}
	}
}
