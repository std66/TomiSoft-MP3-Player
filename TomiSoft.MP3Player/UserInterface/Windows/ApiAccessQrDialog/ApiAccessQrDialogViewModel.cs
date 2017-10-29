using Newtonsoft.Json;
using QRCoder;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media;
using TomiSoft.MP3Player.Utils.Extensions;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.UserInterface.Windows.ApiAccessQrDialog {
	public class ApiAccessQrDialogViewModel : INotifyPropertyChanged {
		private class QrCodeData {
			public string IPAddress { get; private set; }
			public int ID { get; private set; }
			public int Port { get; private set; }

			public QrCodeData(string IPAddress, int Port, int ID) {
				this.IPAddress = IPAddress;
				this.Port = Port;
				this.ID = ID;
			}
		}

		public ImageSource QrCode {
			get;
			private set;
		}

		public int ID {
			get;
			private set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ApiAccessQrDialogViewModel() {
			this.ID = new Random().Next(1000);
			QrCodeData Data = new QrCodeData(GetLocalIPAddress(), App.Config.ServerPort, this.ID);
			
			string QrData = JsonConvert.SerializeObject(Data);

			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode(QrData, QRCodeGenerator.ECCLevel.L);
			QRCode qrCode = new QRCode(qrCodeData);
			this.QrCode = qrCode.GetGraphic(20).ToImageSource();

			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QrCode)));
		}

		private string GetLocalIPAddress() {
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList) {
				if (ip.AddressFamily == AddressFamily.InterNetwork) {
					return ip.ToString();
				}
			}
			throw new Exception("No network adapters with an IPv4 address in the system!");
		}
	}
}
