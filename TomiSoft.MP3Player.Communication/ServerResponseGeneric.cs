using System;

namespace TomiSoft.MP3Player.Communication {
	/// <summary>
	/// Represents a response message from a server with a data
	/// associated to it.
	/// </summary>
	/// <typeparam name="T">The type of the data associated to the response.</typeparam>
    [Serializable]
    public class ServerResponse<T> : ServerResponse {
		/// <summary>
		/// Gets or sets the data associated with the response.
		/// </summary>
        public T Result {
            get;
            set;
        }

		/// <summary>
		/// Creates a new instance of the <see cref="ServerResponse{T}"/> class.
		/// </summary>
        public ServerResponse() {

        }
        
		/// <summary>
		/// Creates a new instance of the <see cref="ServerResponse{T}"/> class
		/// with a successful state, a given data to be associated to the response and a
		/// message.
		/// </summary>
		/// <param name="Result">The data that will be associated to the result object.</param>
		/// <param name="Message">The message that will be associated to the result object.</param>
		/// <returns>The constructed <see cref="ServerResponse{T}"/> object.</returns>
        public static ServerResponse<T> GetSuccess(T Result, string Message = "") {
            return new ServerResponse<T> {
                RequestSucceeded = true,
                Result = Result,
                Message = Message
            };
        }

		/// <summary>
		/// Creates a new instance of the <see cref="ServerResponse{T}"/> class
		/// with a failed state, and a given message to be associated to the response.
		/// </summary>
		/// <param name="Message">The message that will be associated to the result object.</param>
		/// <returns>The constructed <see cref="ServerResponse{T}"/> object.</returns>
		public static ServerResponse<T> GetFailed(string Message) {
            return new ServerResponse<T> {
                RequestSucceeded = false,
                Result = default(T),
                Message = Message
            };
        }
    }
}
