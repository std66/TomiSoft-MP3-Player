using TomiSoft.LyricsEditor.Commands;

namespace TomiSoft.LyricsEditor.ViewModels {
    class MainWindowViewModel {
        private readonly ExitApplicationCommand exitApplicationCommand = new ExitApplicationCommand();
        private readonly NewLyricsCommand newLyricsCommand = new NewLyricsCommand();
        private readonly OpenLyricsCommand openLyricsCommand = new OpenLyricsCommand();
        private readonly SaveLyricsCommand saveLyricsCommand = new SaveLyricsCommand();

        public ExitApplicationCommand ExitApplicationCommand {
            get {
                return exitApplicationCommand;
            }
        }

        public NewLyricsCommand NewLyricsCommand {
            get {
                return newLyricsCommand;
            }
        }

        public OpenLyricsCommand OpenLyricsCommand {
            get {
                return openLyricsCommand;
            }
        }

        public SaveLyricsCommand SaveLyricsCommand {
            get {
                return saveLyricsCommand;
            }
        }
    }
}
