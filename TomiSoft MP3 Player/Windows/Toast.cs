using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Drawing;
using System.IO;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Egy Toast értesítést állít be és hoz létre
	/// </summary>
	public class Toast {
		private string assemblyName;

		public string Title { get; set; }
		public string Content { get; set; }
		public Image Image { get; set; }

		/// <summary>
		/// Létrehozza a TomiSoft.Windows.Notifications.Toast egy új példányát
		/// </summary>
		/// <param name="AssemblyName">A Toast-ot megjelenítő szoftver szerelvényének neve</param>
		public Toast(string AssemblyName) {
			this.assemblyName = AssemblyName;
		}

		/// <summary>
		/// Megjeleníti az értesítést
		/// </summary>
		public void Show() {
			ToastTemplateType template = (this.Image == null) ? ToastTemplateType.ToastText02 : ToastTemplateType.ToastImageAndText02;
			XmlDocument doc = ToastNotificationManager.GetTemplateContent(template);

			XmlNodeList textNodes = doc.GetElementsByTagName("text");
			textNodes[0].AppendChild(doc.CreateTextNode(this.Title));
			textNodes[1].AppendChild(doc.CreateTextNode(this.Content));

			if (this.Image != null) {
				string ImageFile = Path.GetTempPath() + "\\tsmp3_albumart.png";
				Image.Save(ImageFile, System.Drawing.Imaging.ImageFormat.Png);

				XmlNodeList imageNodes = doc.GetElementsByTagName("image");
				imageNodes[0].Attributes.GetNamedItem("src").NodeValue = ImageFile;
			}

			ToastNotification notify = new ToastNotification(doc);

			ToastNotificationManager.CreateToastNotifier(this.assemblyName).Show(notify);
		}
	}
}
