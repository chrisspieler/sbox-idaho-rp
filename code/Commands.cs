using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sandbox.Event;

namespace IdahoRP;

public static class Commands
{
	private static IClient ClientByName(string name )
	{
		var searchName = name.ToLower();
		var client = Game
			.Clients
			.Where(
					cl => cl.Name.ToLower().Contains( name.ToLower() )
				)
			.FirstOrDefault();
		if (client == null )
		{
			Log.Info( $"No client found by the name: {name}" );
		}
		return client;
	}

	[ConCmd.Server( "givemoney" )]
	public static void GiveMoney(string name, float amount )
	{
		var client = ClientByName( name );
		((Idahoid)client.Pawn).GiveCash( amount );
		Log.Info( $"Gave {amount.ToString( "C2" )} to {client}" );
	}
}
