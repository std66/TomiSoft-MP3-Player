using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Utils.Extensions;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.UserInterface.Windows.AboutWindow {
    /// <summary>
    /// Represents the data associated with the about window.
    /// </summary>
    internal class AboutWindowViewModel : INotifyPropertyChanged {
        /// <summary>
        /// This event is fired when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public string Version {
            get {
                return App.Version.ToString();
            }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string Name {
            get {
                return App.Name;
            }
        }

        /// <summary>
        /// Gets the website of the application.
        /// </summary>
        public string Website {
            get {
                return App.Website.ToString();
            }
        }

        /// <summary>
        /// Gets the name of the application's author.
        /// </summary>
        public string Author {
            get {
                return App.Author;
            }
        }

        /// <summary>
        /// Gets the icon of the application.
        /// </summary>
        public ImageSource Icon {
            get {
                return Properties.Resources.AbstractAlbumArt.ToImageSource();
            }
        }

        /// <summary>
        /// Gets the version of the BASS library.
        /// </summary>
        public string BassVersion {
            get {
                if (!BassManager.BassLoaded)
                    return "Ismeretlen";

                return BassManager.BassVersion.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the AboutWindowViewModel class.
        /// </summary>
        public AboutWindowViewModel() {
            this.NotifyAll();
        }

        /// <summary>
        /// Notifies the subscribed members of a property change.
        /// </summary>
        /// <param name="PropertyName">The property's name that is changed</param>
        private void NotifyPropertyChanged(string PropertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /// <summary>
        /// Notifies the subscribed members of all properties change.
        /// </summary>
        private void NotifyAll() {
            string[] Properties = { "Version", "Name", "Website", "Author", "Icon", "BassVersion" };
            foreach (var Property in Properties) {
                this.NotifyPropertyChanged(Property);
            }
        }
    }
}
