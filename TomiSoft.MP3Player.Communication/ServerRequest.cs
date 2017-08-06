using System;
using System.Collections.Generic;

namespace TomiSoft.MP3Player.Communication {
    /// <summary>
    /// Represents a request to the server
    /// </summary>
    [Serializable]
    public class ServerRequest {
        /// <summary>
        /// Gets or sets the name of the server module that will receive
        /// the command.
        /// </summary>
        public string Module {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the command that the module will
        /// execute.
        /// </summary>
        public string Command {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the arguments for the command.
        /// </summary>
        public List<string> Arguments {
            get;
            set;
        }

        public ServerRequest() {

        }

        public ServerRequest(string Module, string Command)
            : this(Module, Command, null) {

        }

        public ServerRequest(string Module, string Command, List<string> Arguments) {
            this.Module = Module;
            this.Command = Command;
            this.Arguments = Arguments;
        }
    }
}
