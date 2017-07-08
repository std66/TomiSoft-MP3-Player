using System;
using System.IO;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.Encoder.Lame {
    public class Lame {
        public string ExecutablePath {
            get {
                return Path.Combine(
                    App.Path,
                    "Encoder",
                    "Lame",
                    Environment.Is64BitProcess ? "x86" : "x64",
                    "lame.exe"
                );
            }
        }

        public string InputFile {
            get;
            private set;
        }

        public string OutputFile {
            get;
            private set;
        }

        public bool InputIsRawPCM {
            get;
            set;
        }

        public Lame() : this("-", "-") {
            this.InputIsRawPCM = true;
        }

        public Lame(string InputFile, string OutputFile) {
            this.InputFile = InputFile;
            this.OutputFile = OutputFile;
        }

        public string GetCommandLine() {
            return $"{ExecutablePath} -h --flush {(InputIsRawPCM ? "-r" : "")} {InputFile} {OutputFile}";
        }
    }
}
