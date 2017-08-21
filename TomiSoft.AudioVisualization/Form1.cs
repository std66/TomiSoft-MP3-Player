using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TomiSoft.MP3Player.Common.Playback;
using TomiSoft.MP3Player.Communication;

namespace TomiSoft.AudioVisualization {
	public partial class Form1 : Form {
		private PlayerClient Client;
		private Timer Timer;

		private List<int> LeftPeaks = new List<int>();
		private List<int> RightPeaks = new List<int>();

		private int Resolution = 150;

		public Form1() {
			if (!PlayerClient.IsServerRunning(22613)) {
				MessageBox.Show(
					caption: "TomiSoft MP3 Player",
					text: "A TomiSoft MP3 Player nem fut.",
					buttons: MessageBoxButtons.OK,
					icon: MessageBoxIcon.Error	
				);

				Environment.Exit(0);
			}

			InitializeComponent();
			Client = new PlayerClient(22613);
			Client.KeepAlive();

			this.DoubleBuffered = true;

			this.Timer = new Timer() {
				Enabled = true,
				Interval = 25
			};

			this.Timer.Tick += (o, e) => {
				this.GetNextPeak();
				this.Invalidate();
			};
		}

		private void GetNextPeak() {
			IAudioPeakMeter Peak = this.Client.Playback.PeakLevel;
			
			this.LeftPeaks.Add(Peak.LeftPeak);
			this.RightPeaks.Add(Peak.RightPeak);

			if (this.LeftPeaks.Count == Resolution + 1)
				this.LeftPeaks.RemoveAt(0);

			if (this.RightPeaks.Count == Resolution + 1)
				this.RightPeaks.RemoveAt(0);
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			e.Graphics.Clear(Color.Black);
			this.DrawGrid(e.Graphics, this.ClientRectangle.Width, this.ClientRectangle.Height, 20);

			float Gap = (float)this.ClientRectangle.Width / this.Resolution;
			float Center = this.ClientRectangle.Height / 2;

			PointF PreviousLeft = new Point();
			PointF PreviousRight = new Point();

			float X = this.ClientRectangle.Width;
			for (int i = this.LeftPeaks.Count - 1; i >= 0; i--) {
				//Draw left peak
				float Y = Remap(Center, this.LeftPeaks[i]);
				X -= Gap;
				
				e.Graphics.DrawLine(Pens.Yellow, PreviousLeft, new PointF(X, Y));

				PreviousLeft.X = X;
				PreviousLeft.Y = Y;

				//Draw right peak
				Y = Remap(Center, this.RightPeaks[i]) + Center;
				
				e.Graphics.DrawLine(Pens.Yellow, PreviousRight, new PointF(X, Y));

				PreviousRight.X = X;
				PreviousRight.Y = Y;
			}
		}

		private void DrawGrid(Graphics g, int Width, int Height, int Division) {
			#region Error checking
			if (g == null)
				return;
			#endregion

			for (int X = 0; Width / 2 + X * Division < Width; X++) {
				g.DrawLine(
					Pens.DarkGreen,
					Width / 2 + X * Division, 0,
					Width / 2 + X * Division, Height
				);

				g.DrawLine(
					Pens.DarkGreen,
					Width / 2 - X * Division, 0,
					Width / 2 - X * Division, Height
				);
			}

			for (int X = 0; Width / 2 + X * Division < Width; X++) {
				g.DrawLine(
					Pens.DarkGreen,
					0,		Height / 2 + X * Division,
					Width,  Height / 2 + X * Division
				);

				g.DrawLine(
					Pens.DarkGreen,
					0,		Height / 2 - X * Division,
					Width,  Height / 2 - X * Division
				);
			}

			g.DrawLine(
				Pens.WhiteSmoke,
				0, Height / 2,
				Width, Height / 2
			);

			g.DrawLine(
				Pens.WhiteSmoke,
				Width / 2, 0,
				Width / 2, Height
			);
		}

		private float Remap(float NewMaximum, int Value) {
			double Diff = Value / 32768d;
			return (float)(NewMaximum - (NewMaximum * Diff));
		}

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			this.Timer.Stop();
			this.Client.Dispose();
		}
	}
}
