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

public static partial class Commands
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

	[ConCmd.Server("respawn")]
	public static void Respawn()
	{
		var client = ConsoleSystem.Caller?.Pawn as Idahoid;
		if (client == null )
		{
			Log.Info( "Respawn command not valid for non-players or clients without pawns." );
			return;
		}
		client.Respawn();
	}

	[ConCmd.Admin("devcam")]
	public static void ToggleDevCam()
	{
		var client = ConsoleSystem.Caller;
		var camera = client.Components.Get<DevCamera>( true );

		if ( camera == null )
		{
			camera = new DevCamera();
			client.Components.Add( camera );
			return;
		}

		camera.Enabled = !camera.Enabled;
	}

	[ConCmd.Server("setname")]
	public static void SetName(long steamId, string name )
	{
		//var client = ConsoleSystem.Caller.Client;
		//if (  client == null )
		//{
		//	Log.Info( $"Command {nameof( SetName ).ToLower()} must be called by a client." );
		//	return;
		//}
		CitizenData.GetData( steamId ).Name = name;
		Log.Trace( $"{ConsoleSystem.Caller.Client} - Name set to \"{name}\"." );
	}

	[ConCmd.Server("setgender")]
	public static void SetGender(string gender )
	{
		var client = ConsoleSystem.Caller.Client;
		if ( client == null )
		{
			Log.Info( $"Command {nameof( SetGender ).ToLower()} must be called by a client." );
			return;
		}
		var genderRes = ResourceLibrary
			.GetAll<Gender>()
			.FirstOrDefault( g => g.Name.ToLower() == gender.ToLower() );
		if (genderRes == null )
		{
			Log.Info( $"No gender found with the name \"{gender}\". Feel free to add it yourself through the character info screen!" );
			return;
		}
		CitizenData.GetData( client.SteamId ).Gender = genderRes;
		Log.Trace( $"{client} - Gender set to \"{genderRes.Name}\"." );
	}

	[ConCmd.Server("setgender")]
	public static void SetGender(long steamId, int genderResourceId )
	{
		//var client = ConsoleSystem.Caller.Client;
		//if ( client == null )
		//{
		//	Log.Info( $"Command {nameof( SetGender ).ToLower()} must be called by a client." );
		//	return;
		//}
		var genderRes = ResourceLibrary
			.GetAll<Gender>()
			.FirstOrDefault( g => g.ResourceId == genderResourceId );
		if (genderRes == null)
		{
			Log.Info( "No gender found with the specified resource ID." );
			return;
		}
		CitizenData.GetData( steamId ).Gender = genderRes;
		Log.Info( $"{ConsoleSystem.Caller.Client} - Gender set to \"{genderRes.Name}\"." );
	}

	[ConCmd.Server( "givemoney" )]
	public static void GiveMoney(string name, float amount, string description = "Console Command" )
	{
		var client = ClientByName( name );
		((Idahoid)client.Pawn).GiveCash( amount, description );
		Log.Info( $"{ConsoleSystem.Caller?.Client} - Gave {amount.ToString( "C2" )} to {client}" );
	}
}
