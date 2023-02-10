using IdahoRP.Api;
using IdahoRP.UI;
using Sandbox;
using Sandbox.UI;

namespace IdahoRP;

public partial class IdahoRP : BaseGameManager
{
	[ClientInput] public Vector3 InputDirection { get; protected set; }

	private bool _botsInitialized = false;
	private RootPanel _welcomePage;

	private static IdahoRP _instance;

	public IdahoRP()
	{
		_instance = this;
		if (Game.IsClient)
		{
			_welcomePage = new WelcomePage();
		}
		else
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
	}

	private void JoinGame(IClient cl )
	{
		var pawn = new Idahoid( cl );
		cl.Pawn = pawn;

		pawn.Respawn();

		CloseWelcomePage();
		pawn.ShowHud();
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
