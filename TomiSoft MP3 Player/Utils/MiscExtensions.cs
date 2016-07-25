using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TomiSoft_MP3_Player {
	static class MiscExtensions {
		/// <summary>
		/// Creates an System.Windows.Media.ImageSource from the given System.Drawing.Image.
		/// </summary>
		/// <param name="b">The System.Drawing.Image to convert.</param>
		/// <returns>The System.Windows.Media.ImageSource instance or null if b is null.</returns>
		public static ImageSource ToImageSource(this Image b) {
			if (b == null)
				return null;

			BitmapImage img = new BitmapImage();
			using (MemoryStream ms = new MemoryStream()) {
				b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
				ms.Position = 0;
				ms.Seek(0, SeekOrigin.Begin);

				img.BeginInit();
				img.CacheOption = BitmapCacheOption.OnLoad;
				img.StreamSource = ms;
				img.EndInit();
			}

			return img;
		}
	}
}
