using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Web.Http {
	public class HeadRequest {
		private uint maxRedirects;

		public uint MaxRedirects {
			get { return maxRedirects; }
			set {
				maxRedirects = value;
			}
		}
		
		public bool AllowRedirects {
			get;
			set;
		}

		public async Task<HeadWebResponse> SendRequest(string Location) {
			return await this.SendRequest(new Uri(Location));
		}

		public async Task<HeadWebResponse> SendRequest(Uri Location) {
			HeadWebResponse Result = null;
			uint Redirects = 0;

			do {
				TcpClient Client = new TcpClient(Location.Host, Location.Port);
				using (StreamWriter sw = new StreamWriter(Client.GetStream())) {
					StreamReader sr = new StreamReader(Client.GetStream());

					await sw.WriteAsync(this.PrepareRequest(Location));

					StringBuilder sb = new StringBuilder();
					string CurrentLine;
					do {
						CurrentLine = await sr.ReadLineAsync();
						sb.AppendLine(CurrentLine);
					}
					while (!String.IsNullOrWhiteSpace(CurrentLine));

					Result = HeadWebResponse.Parse(sb.ToString());

					if (Result.IsRedirect) {
						Redirects++;
						
					}
				}
			}
			while (Result.IsRedirect && Redirects < this.maxRedirects && this.AllowRedirects);

			return Result;
		}

		private string PrepareRequest(Uri u) {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"HEAD {u.PathAndQuery} HTTP/1.0");
			sb.AppendLine($"User-Agent: TomiSoft.Web.Http.HeadRequest ({Assembly.GetExecutingAssembly().ToString()})");
			sb.AppendLine();

			return sb.ToString();
		}
	}
}
