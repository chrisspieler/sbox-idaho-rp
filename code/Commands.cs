using IdahoRP.Api;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

	[ConCmd.Server("setname")]
	public static void SetName(string name )
	{
		((Idahoid)Game.LocalPawn).RpName = name;
	}

	[ConCmd.Server("setgender")]
	public static void SetGender(string gender )
	{
		var genderRes = ResourceLibrary
			.GetAll<Gender>()
			.FirstOrDefault( g => g.Name.ToLower() == gender.ToLower() );
		if (genderRes == null )
		{
			Log.Info( $"No gender found with the name \"{gender}\". Feel free to add it yourself through the character info screen!" );
			return;
		}
		Log.Info( $"{ConsoleSystem.Caller.Client} - Gender set to \"{genderRes.Name}\"." );
		((Idahoid)Game.LocalPawn).Gender = genderRes;
	}

	[ConCmd.Server("setgender")]
	public static void SetGender(int genderResourceId )
	{
		var genderRes = ResourceLibrary
			.GetAll<Gender>()
			.FirstOrDefault( g => g.ResourceId == genderResourceId );
		if (genderRes == null)
		{
			Log.Info( "No gender found with the specified resource ID." );
			return;
		}
		Log.Info( $"{ConsoleSystem.Caller.Client} - Gender set to \"{genderRes.Name}\"." );
		((Idahoid)Game.LocalPawn).Gender = genderRes;
	}

	[ConCmd.Server( "givemoney" )]
	public static void GiveMoney(string name, float amount )
	{
		var client = ClientByName( name );
		((Idahoid)client.Pawn).GiveCash( amount );
		Log.Info( $"{ConsoleSystem.Caller.Client} - Gave {amount.ToString( "C2" )} to {client}" );
	}
}
