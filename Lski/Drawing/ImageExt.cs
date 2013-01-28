using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Lski.Drawing {

	public static class ImageExt {

		public static Image Resize(this Image image, int size) {

			Size newSize;

			if (image.Width == size && image.Height == size) {
				return image;
			}

			if (image.Width > image.Height) {

				var ratio = Decimal.Divide(size, image.Width);
				newSize = new Size(size, (int)(image.Height * ratio));
			}
			else {

				var ratio = Decimal.Divide(size, image.Height);
				newSize = new Size((int)(image.Width * ratio), size);
			}

			return Resize(image, newSize, false);
		}

		public static Image Resize(this Image image, Size size, bool preserveAspectRatio = true) {

			int newWidth;
			int newHeight;

			if (preserveAspectRatio) {
				int originalWidth = image.Width;
				int originalHeight = image.Height;
				float percentWidth = (float)size.Width / (float)originalWidth;
				float percentHeight = (float)size.Height / (float)originalHeight;
				float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
				newWidth = (int)(originalWidth * percent);
				newHeight = (int)(originalHeight * percent);
			}
			else {
				newWidth = size.Width;
				newHeight = size.Height;
			}
			Image newImage = new Bitmap(newWidth, newHeight);

			using (Graphics graphicsHandle = Graphics.FromImage(newImage)) {
				graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
			}

			return newImage;
		}

	}
}
