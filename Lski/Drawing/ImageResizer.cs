using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Lski.Drawing {
	
	/// <summary>
	/// Simple class to provide resizing of images, unlike the default standard the settings are set to high quality to maintain a good quality image.
	/// </summary>
	public class ImageResizer {

		public InterpolationMode InterpolationMode { get; set; }
		public SmoothingMode SmoothingMode { get; set; }
		public CompositingQuality CompositingQuality { get; set; }
		public PixelOffsetMode PixelOffsetMode { get; set; }

		public ImageResizer() {
			InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
		}

		/// <summary>
		/// Resizes the width of the image, if preserveAspectRatio then it will resize the height accordingly.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="maxWidth"></param>
		/// <returns></returns>
		public Image ResizeWidth(Image image, int width, bool preserveAspectRatio = true) {

			Size newSize;

			if (preserveAspectRatio) {
				decimal ratio = decimal.Divide(width, image.Width);
				newSize = new Size(width, (int)(image.Height * ratio));
			}
			else {
				newSize = new Size(width, image.Height);
			}

			return image.Resize(newSize, false);
		}

		/// <summary>
		/// Reduces the size of the image so that the largest dimension of the original image is not bigger than the max stated size, resizing the
		/// other deimension proportionally to the larger dimension to maintain aspect ratio.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public Image Resize(Image image, int size) {

			if (image.Width == size && image.Height == size) {
				return image;
			}

			Size newSize;

			if (image.Width > image.Height) {
				decimal ratio = decimal.Divide(size, image.Width);
				newSize = new Size(size, (int)(image.Height * ratio));
			}
			else {
				decimal ratio2 = decimal.Divide(size, image.Height);
				newSize = new Size((int)(image.Width * ratio2), size);
			}

			return image.Resize(newSize, false);
		}

		public Image Resize(Image image, Size size, bool preserveAspectRatio = true) {

			int newWidth;
			int newHeight;

			if (preserveAspectRatio) {
				int originalWidth = image.Width;
				int originalHeight = image.Height;
				float percentWidth = (float)size.Width / (float)originalWidth;
				float percentHeight = (float)size.Height / (float)originalHeight;
				float percent = (percentHeight < percentWidth) ? percentHeight : percentWidth;
				newWidth = (int)((float)originalWidth * percent);
				newHeight = (int)((float)originalHeight * percent);
			}
			else {
				newWidth = size.Width;
				newHeight = size.Height;
			}

			Image newImage = new Bitmap(newWidth, newHeight);

			using (Graphics gfx = Graphics.FromImage(newImage)) {

				gfx.InterpolationMode = InterpolationMode;
				gfx.SmoothingMode = SmoothingMode;
				gfx.CompositingQuality = CompositingQuality;
				gfx.PixelOffsetMode = PixelOffsetMode;
				gfx.DrawImage(image, 0, 0, newWidth, newHeight);
			}
			return newImage;
		}

		/// <summary>
		/// Very basic method for finding the save format for an image, useful if the imported image type is not known.
		/// </summary>
		/// <param name="img"></param>
		/// <returns></returns>
		public ImageFormat SaveFormat(Image img) {

			if (ImageFormat.Jpeg.Equals(img.RawFormat)) {
				return ImageFormat.Jpeg;
			}
			if (ImageFormat.Png.Equals(img.RawFormat)) {
				return ImageFormat.Png;
			}
			if (ImageFormat.Gif.Equals(img.RawFormat)) {
				return ImageFormat.Gif;
			}
			if (ImageFormat.Bmp.Equals(img.RawFormat)) {
				return ImageFormat.Bmp;
			}
			return ImageFormat.Jpeg;
		}
	}
}
