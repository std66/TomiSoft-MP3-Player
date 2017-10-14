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

		/// <summary>
		/// Throws an exception if the request has failed.
		/// </summary>
		/// <exception cref="Exception">when the request has failed</exception>
        internal void Check() {
            if (!this.RequestSucceeded)
                throw new Exception($"Request failed. Server message: {this.Message}");
        }
    }
}
