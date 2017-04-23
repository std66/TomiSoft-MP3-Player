using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TomiSoft.MP3Player.Utils.Extensions {
    /// <summary>
    /// Contains extension methods for images.
    /// </summary>
	public static class ImageExtensions {
        /// <summary>
        /// Creates an <see cref="ImageSource"/> from the given <see cref="Image"/>.
        /// </summary>
        /// <param name="b">The <see cref="Image"/> to convert.</param>
        /// <returns>The <see cref="ImageSource"/> instance or null if b is null.</returns>
        public static ImageSource ToImageSource(this Image b) {
            #region Error checking
            if (b == null)
				return null;
            #endregion

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
