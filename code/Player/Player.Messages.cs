using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP;

public partial class Idahoid
{
	public enum MessageLevel
	{
		Info,
		Error
	}

	[ClientRpc] public void ShowModalMessage(string message ) => Log.Info( $"Modal message: {message}" );
	[ClientRpc] public void ShowToastMessage( string message, MessageLevel level = MessageLevel.Info ) => Log.Info( $"Toast message: {message}" );
}
