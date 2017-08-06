using System;

namespace TomiSoft.MP3Player.Communication {
    public class ServerResponse {
        public bool RequestSucceeded {
            get;
            set;
        }

        public string Message {
            get;
            set;
        }

        internal void Check() {
            if (!this.RequestSucceeded)
                throw new Exception($"Request failed. Server message: {this.Message}");
        }
    }
}
