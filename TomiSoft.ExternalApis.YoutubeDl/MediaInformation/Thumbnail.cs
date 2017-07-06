using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TomiSoft.ExternalApis.YoutubeDl.MediaInformation {
	public class Thumbnail {
		public int ID { get; internal set; }
		public string Uri { get; internal set; }

		public async Task<Image> DownloadAsImageAsync() {
			using (MemoryStream ms = new MemoryStream(await this.DownloadAsByteArrayAsync()))
			using (Bitmap Result = new Bitmap(ms)) { 
				return new Bitmap(Result);
			}
		}

		public async Task<ImageSource> DownloadAsImageSourceAsync() {
			byte[] imageData = await this.DownloadAsByteArrayAsync();
			
			var image = new BitmapImage();
			using (var mem = new MemoryStream(imageData)) {
				mem.Position = 0;
				image.BeginInit();
				image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.UriSource = null;
				image.StreamSource = mem;
				image.EndInit();
			}
			image.Freeze();

			return image;
		}

		public Task<byte[]> DownloadAsByteArrayAsync() {
			using (WebClient cl = new WebClient())
				return cl.DownloadDataTaskAsync(Uri);
		}

		public async Task SaveAsync(Stream Target, ImageFormat Format) {
			(await this.DownloadAsImageAsync()).Save(Target, Format);
		}
	}
}
