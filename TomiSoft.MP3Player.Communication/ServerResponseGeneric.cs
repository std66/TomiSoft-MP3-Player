using System;

namespace TomiSoft.MP3Player.Communication {
    [Serializable]
    public class ServerResponse<T> : ServerResponse {
        public T Result {
            get;
            set;
        }

        public ServerResponse() {

        }
        
        public static ServerResponse<T> GetSuccess(T Result, string Message = "") {
            return new ServerResponse<T> {
                RequestSucceeded = true,
                Result = Result,
                Message = Message
            };
        }

        public static ServerResponse<T> GetFailed(string Message) {
            return new ServerResponse<T> {
                RequestSucceeded = false,
                Result = default(T),
                Message = Message
            };
        }
    }
}
