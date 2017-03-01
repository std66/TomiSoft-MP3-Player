using System;

namespace TomiSoft.MP3Player.Communication {
	/// <summary>
	/// This attribute marks a method as a server command.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ServerCommandAttribute : Attribute {
	}
}
