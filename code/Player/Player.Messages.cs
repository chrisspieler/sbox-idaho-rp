using Sandbox;

namespace IdahoRP;

public partial class Idahoid
{
	public enum MessageLevel
	{
		Info,
		Error
	}

	[ClientRpc] public void ShowModalMessage(string message ) => Log.Info( $"Modal message: {message}" );
	[ClientRpc] public void ShowToastMessage( string message, MessageLevel level = MessageLevel.Info ) => Log.Trace( $"Toast message: {message}" );
}
