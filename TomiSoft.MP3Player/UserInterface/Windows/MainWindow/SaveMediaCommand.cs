using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Utils;
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
		/// Stores the view model for the main window that is used for hiding
		/// the menu when this command is executed.
		/// </summary>
		private readonly MainWindowViewModel ViewModel;
		
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
		/// This event occures when the return value of <see cref="CanExecute"/> is changed.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="SaveMediaCommand"/> class.
		/// </summary>
		/// <param name="ViewModel">
		///		The view model for the main window that is used for hiding the menu when this command is executed.
		/// </param>
		public SaveMediaCommand(MainWindowViewModel ViewModel) {
			this.ViewModel = ViewModel;
		}

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

			this.ViewModel.MenuVisible = false;

			string Filename = String.Empty;
			if (this.ShowSaveDialog(SavableMedia, out Filename)) {
				bool Result = false;

                Progress<LongOperationProgress> Progress = new Progress<LongOperationProgress>();

                ProgressBarDialog.ProgressBarDialog dlg = new ProgressBarDialog.ProgressBarDialog(
                    Title: "TomiSoft MP3 Player",
                    Text: "Zene mentése...",
                    Progress: Progress
                );
                
                dlg.Show();

				try {
					using (Stream s = File.OpenWrite(Filename)) {
						Result = await SavableMedia.SaveToAsync(s, Progress);
					}
				}
				catch (IOException) {
					Result = false;
				}

                dlg.Close();

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

            string filename = SavableMedia.RecommendedFilename ?? SavableMedia.OriginalFilename;

            SaveFileDialog Dialog = new SaveFileDialog() {
				Filter = $"Médiafájl|*{Path.GetExtension(filename)}",
				FileName = filename
			};
			
			bool Result = Dialog.ShowDialog() ?? false;
			Filename = Dialog.FileName;

			return Result;
		}
	}
}
