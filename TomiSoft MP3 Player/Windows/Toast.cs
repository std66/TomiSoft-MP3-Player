using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Egy Toast értesítést állít be és hoz létre
	/// </summary>
	public class Toast {
		private string assemblyName;

		/// <summary>
		/// A Toast címe
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// A Toast tartalma
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// A Toast-hoz rendelt kép.
		/// </summary>
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
			if (this.Image == null) {
				this.ShowBasicToast();
			}
			else {
				this.ShowImageToast();
			}
		}

		/// <summary>
		/// Előkészíti a Toast XML dokumentumát a megadott sablon alapján és ráilleszti a szövegeket.
		/// </summary>
		/// <param name="TemplateType">A Toast sablon típusa</param>
		/// <returns>A Toast XML dokumentuma</returns>
		private XmlDocument PrepareXml(ToastTemplateType TemplateType) {
			XmlDocument doc = ToastNotificationManager.GetTemplateContent(TemplateType);

			XmlNodeList textNodes = doc.GetElementsByTagName("text");
			textNodes[0].AppendChild(doc.CreateTextNode(this.Title));
			textNodes[1].AppendChild(doc.CreateTextNode(this.Content));

			return doc;
		}

		/// <summary>
		/// Megjeleníti a csak szöveget tartalmazó Toast-ot.
		/// </summary>
		private void ShowBasicToast() {
			XmlDocument doc = this.PrepareXml(ToastTemplateType.ToastText02);

			ToastNotification notify = new ToastNotification(doc);

			ToastNotificationManager.CreateToastNotifier(this.assemblyName).Show(notify);
		}

		/// <summary>
		/// Megjeleníti a szöveget és a képet tartalmazó toast-ot.
		/// </summary>
		private void ShowImageToast() {
			XmlDocument doc = this.PrepareXml(ToastTemplateType.ToastImageAndText02);

			string ImageFile = Path.GetTempPath() + "\\ToastImage.png";
			Image.Save(ImageFile, System.Drawing.Imaging.ImageFormat.Png);

			XmlNodeList imageNodes = doc.GetElementsByTagName("image");
			imageNodes[0].Attributes.GetNamedItem("src").NodeValue = ImageFile;

			ToastNotification notify = new ToastNotification(doc);

			ToastNotificationManager.CreateToastNotifier(this.assemblyName).Show(notify);
		}
	}
}
