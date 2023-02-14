using IdahoRP.Api;
using IdahoRP.UI;
using Sandbox;
using Sandbox.UI;

namespace IdahoRP;

public partial class IdahoGame : BaseGameManager
{
	[ClientInput] public Vector3 InputDirection { get; protected set; }

	private bool _botsInitialized = false;
	private RootPanel _welcomePage;

	private static IdahoGame _instance;

	public IdahoGame()
	{
		_instance = this;
		if (!Game.IsClient)
		{
			JobManager.Initialize();
		}
	}

	public override void ClientJoined( IClient cl )
	{
		base.ClientJoined( cl );

		if ( cl.IsBot )
		{
			JoinGame( cl );
		}
		else
		{
			var citizen = CitizenData.GetData( cl.SteamId );
			ShowWelcomePage(To.Single(cl), citizen.Name, citizen.Gender.ResourceId);
		}
	}

	private void JoinGame(IClient cl )
	{
		var pawn = new Idahoid( cl );
		cl.Pawn = pawn;

		pawn.Respawn();

		CloseWelcomePage(To.Single(cl));
		pawn.ShowHud();
	}

	[ClientRpc]
	private void ShowWelcomePage(string name, int genderId) 
	{
		// I can't use a BaseNetworkable as a parameter to a client RPC, and 
		// when I tried using a struct, it complained about the string not being
		// a value type, so using as many parameters as there are data to show the
		// client will have to do for now.
		_welcomePage = new WelcomePage(name, genderId);
	}

	[ClientRpc]
	private void CloseWelcomePage()
	{
		_welcomePage.Delete();
	}

	[ConCmd.Server( "joingame" )]
	private static void JoinGameCmd()
	{
		_instance.JoinGame( ConsoleSystem.Caller );
	}

	public override void Shutdown()
	{
		// NOP
	}
}
