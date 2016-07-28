using System.ComponentModel;
using System.Windows.Media;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// View model for MainWindow.
	/// </summary>
	public class MainWindowViewModel : INotifyPropertyChanged {
		/// <summary>
		/// Fires when a property is changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private string title = "Nincs zene betöltve";
		private string lyrics = string.Empty;
		private ImageSource albumImage;

		/// <summary>
		/// The album image.
		/// </summary>
		public ImageSource AlbumImage {
			get { return albumImage; }
			set {
				albumImage = value == null ? Properties.Resources.AbstractAlbumArt.ToImageSource() : value;
				this.NotifyPropertyChanged("AlbumImage");
			}
		}

		/// <summary>
		/// The title of the song.
		/// </summary>
		public string Title {
			get { return title; }
			set {
				title = value == null ? "Nincs cím" : value;
				this.NotifyPropertyChanged("Title");
			}
		}

		/// <summary>
		/// The current line of lyrics.
		/// </summary>
		public string Lyrics {
			get { return lyrics; }
			set {
				lyrics = value;
				this.NotifyPropertyChanged("Lyrics");
			}
		}

		private void NotifyPropertyChanged(string PropertyName) {
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
		}
	}
}
