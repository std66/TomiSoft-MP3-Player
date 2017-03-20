using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace TomiSoft.MP3Player.Communication {
	/// <summary>
	/// This class provides informations about a remote media resource.
	/// </summary>
	public class RemoteResourceInfo {
		/// <summary>
		/// Gets if the remote resource is an internet radio stream.
		/// </summary>
		public bool IsInternetRadioStream {
			get;
			private set;
		}

		/// <summary>
		/// Gets if the request was successful or not.
		/// </summary>
		public bool RequestSucceeded {
			get;
			private set;
		}

		/// <summary>
		/// Gets the length of the resource in bytes. Null is returned when
		/// the length of the resource is unknown.
		/// </summary>
		public long? Length {
			get;
			private set;
		}

		/// <summary>
		/// Gets the file's name represented by the remote resource. Null or empty string is
		/// returned when unknown.
		/// </summary>
		public string Filename {
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoteResourceInfo"/> class.
		/// </summary>
		private RemoteResourceInfo() {}

		/// <summary>
		/// Determines the type of the given resource asynchronously that is represented
		/// with it's URI.
		/// </summary>
		/// <param name="Uri">The URI that locates the remote resource.</param>
		/// <returns>A <see cref="RemoteResourceInfo"/> instance that holds informations about the remote resource.</returns>
		public async static Task<RemoteResourceInfo> GetUriInfoAsync(string Uri) {
			#region Error checking
			if (String.IsNullOrWhiteSpace(Uri))
				throw new ArgumentException($"{nameof(Uri)} cannot be null or empty string.");
			#endregion
			
			RemoteResourceInfo Result = new RemoteResourceInfo();

			var uri = new System.Uri(Uri);
			HttpRequestMessage Message = new HttpRequestMessage(HttpMethod.Get, uri);
			Task<HttpResponseMessage> RequestTask = (new HttpClient()).SendAsync(Message, HttpCompletionOption.ResponseHeadersRead);
			HttpResponseMessage Response = await RequestTask;

			Result.RequestSucceeded = Response.IsSuccessStatusCode && !RequestTask.IsFaulted;

			if (Result.RequestSucceeded) {
				Result.IsInternetRadioStream = Response.Headers.Where(x => x.Key.ToLower().StartsWith("icy-")).Any();
				Result.Length = Response.Content?.Headers.ContentLength ?? Response.Content?.Headers.ContentDisposition?.Size;
				Result.Filename = Response.Content?.Headers.ContentDisposition?.Name ?? Path.GetFileName(uri.LocalPath);
			}

			return Result;
		}
	}
}
