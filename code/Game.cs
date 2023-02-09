using IdahoRP.Api;
using IdahoRP.UI;
using Sandbox;

namespace IdahoRP;

public partial class IdahoRP : BaseGameManager
{
	[ClientInput] public Vector3 InputDirection { get; protected set; }

	private bool _botsInitialized = false;

	public IdahoRP()
	{
		if (Game.IsClient)
		{
			_ = new Hud();
		}
		else
		{
			JobManager.Initialize();
		}
	}

	public override void ClientJoined( IClient cl )
	{
		base.ClientJoined( cl );

		var pawn = new Idahoid(cl);
		cl.Pawn = pawn;

		pawn.Respawn();

		if ( !_botsInitialized )
		{
			_botsInitialized = true;
			BotManager.AddCitizenBot();
		}
	}

	public override void Shutdown()
	{
		// NOP
	}
}
