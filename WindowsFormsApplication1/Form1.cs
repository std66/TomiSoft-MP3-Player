using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApplication1 {
	public partial class Form1 : Form {
		private readonly string ApiKey = "AIzaSyDNI5Vo1MT3n9Dy4z4vuUGFtEkjlnkNZ-E";

		public Form1() {
			InitializeComponent();
		}

		private string GetVideoID(string url) {
			Uri u = new Uri(url);
			return HttpUtility.ParseQueryString(u.Query).GetValues("v").FirstOrDefault();
		}

		private async void button1_Click(object sender, EventArgs e) {
			YouTubeService s = new YouTubeService(new BaseClientService.Initializer() {
				ApiKey = this.ApiKey,
				ApplicationName = "TomiSoft MP3 Player"
			});

			VideosResource.ListRequest Request = s.Videos.List("contentDetails,snippet");
			Request.Id = this.GetVideoID(textBox1.Text);

			VideoListResponse Response = await Request.ExecuteAsync();
			label1.Text = Response.Items[0].Snippet.Title;
			label2.Text = XmlConvert.ToTimeSpan(Response.Items[0].ContentDetails.Duration).ToString("hh\\:mm\\:ss");
		}
	}
}
