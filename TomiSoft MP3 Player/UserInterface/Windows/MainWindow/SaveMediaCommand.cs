using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TomiSoft.MP3Player.Playback;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.UserInterface.Windows.MainWindow {
	/// <summary>
	/// Represents the command for the "Save media" button.
	/// </summary>
	class SaveMediaCommand : ICommand {
		/// <summary>
		/// Stores the <see cref="IPlaybackManager"/> instance.
		/// </summary>
		private IPlaybackManager playbackManager;

		/// <summary>
		/// Gets or sets the <see cref="IPlaybackManager"/> instance.
		/// </summary>
		public IPlaybackManager PlaybackManager {
			get { return playbackManager; }
			set {
				playbackManager = value;
				this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// This event occures when the return value of CanExecute is changed.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Gets if the command can be executed in the current state.
		/// </summary>
		/// <param name="parameter">An <see cref="object"/></param>
		/// <returns>True if the command can be executed, false if not.</returns>
		public bool CanExecute(object parameter) {
			return this.playbackManager is ISavable;
		}

		/// <summary>
		/// Executes the command. If the media represented by the <see cref="PlaybackManager"/>
		/// can be saved, asks the user for a path to the target file and
		/// copies the media.
		/// </summary>
		/// <param name="parameter">An <see cref="object"/></param>
		public async void Execute(object parameter) {
			ISavable SavableMedia = this.playbackManager as ISavable;
			
			#region Error checking
			if (SavableMedia == null)
				return;
			#endregion
			
			string Filename = String.Empty;
			if (this.ShowSaveDialog(SavableMedia, out Filename)) {
				bool Result = false;

				try {
					using (Stream s = File.OpenWrite(Filename)) {
						Result = await SavableMedia.SaveToAsync(s);
					}
				}
				catch (IOException) {
					Result = false;
				}

				if (!Result) {
					MessageBox.Show(
						caption: App.Name,
						messageBoxText: "A fájlt nem sikerült elmenteni.",
						button: MessageBoxButton.OK,
						icon: MessageBoxImage.Error
					);
				}
			}
		}

		/// <summary>
		/// Displays a save dialog to ask the user for the path of the new file.
		/// </summary>
		/// <param name="SavableMedia">An <see cref="ISavable"/> instance.</param>
		/// <param name="Filename">A variable where the path of the new file will be stored.</param>
		/// <returns>True if the user aggreed to save the file, false if cancelled it.</returns>
		private bool ShowSaveDialog(ISavable SavableMedia, out string Filename) {
			#region Error checking
			if (SavableMedia == null) {
				Filename = String.Empty;
				return false;
			}
			#endregion
			
			SaveFileDialog Dialog = new SaveFileDialog() {
				Filter = $"Médiafájl|*{Path.GetExtension(SavableMedia.OriginalSource)}",
				FileName = SavableMedia.RecommendedFilename ?? SavableMedia.OriginalFilename
			};
			
			bool Result = Dialog.ShowDialog() ?? false;
			Filename = Dialog.FileName;

			return Result;
		}
	}
}
