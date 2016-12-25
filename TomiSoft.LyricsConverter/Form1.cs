using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TomiSoft.Music.Lyrics;
using TomiSoft.Music.Lyrics.Xml;

namespace TomiSoft.LyricsConverter {
	public partial class Form1 : Form {
		private ILyricsReader LyricsReader = null;

		public Form1() {
			InitializeComponent();
		}

		private void btnOpen_Click(object sender, EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog() {
				Filter = "Dalszöveg fájlok|*.lrc;*.xml"
			};

			if (dlg.ShowDialog() == DialogResult.OK) {
				this.LyricsReader = LyricsLoader.LoadFile(dlg.FileName);

				if (this.LyricsReader == null) {
					MessageBox.Show("Úgy tűnik, ez a fájl nem dalszöveg-fájl.");
				}
				else {
					lFilename.Text = new FileInfo(dlg.FileName).Name;
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e) {
			#region Error checking
			if (this.LyricsReader == null)
				return;
			#endregion

			SaveFileDialog dlg = new SaveFileDialog() {
				Filter = "LRC dalszöveg-fájl|*.lrc|XML dalszöveg-fájl|*.xml"
			};

			ILyricsWriter Writer = null;

			if (dlg.ShowDialog() == DialogResult.OK) {
				switch (new FileInfo(dlg.FileName).Extension) {
					case ".lrc":
						MessageBox.Show("LRC-be mentés még nem támogatott.");
						break;

					case ".xml":
						Writer = XmlLyricsWriter.CreateFromReader(this.LyricsReader);
						break;
				}

				if (Writer != null)
					File.AppendAllText(dlg.FileName, Writer.Build());
			}
		}
	}
}
