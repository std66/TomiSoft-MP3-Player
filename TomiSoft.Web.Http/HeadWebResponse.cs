using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TomiSoft.Web.Http {
	public class HeadWebResponse : WebResponse {
		private uint statusCode;

		public uint StatusCode {
			get { return statusCode; }
		}
		
		public bool IsRedirect {
			get {
				return this.statusCode >= 300 && this.statusCode <= 399;
			}
		}

		private HeadWebResponse(uint StatusCode) {
			this.statusCode = StatusCode;
		}

		public static HeadWebResponse Parse(string Response) {
			Regex r = new Regex(@"\r?\n");
			string[] Lines = r.Split(Response);
			
			HeadWebResponse Result = new HeadWebResponse(
				StatusCode: Convert.ToUInt32(Lines[0].Split(" ".ToCharArray(), 3).ElementAt(1))
			);

			for (int i = 1; i < Lines.Length; i++) {
				string[] Parts = Lines[i].Split(':');
				
			}

			return Result;
		}
	}
}
