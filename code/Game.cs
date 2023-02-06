using IdahoRP.Api;
using IdahoRP.UI;
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

	[ConCmd.Server("add_citizen_bot")]
	public static void AddCitizenBot()
	{
		var bot = new CitizenBot( "Timmy Shitspittle" );
	}

	public override void ClientJoined( IClient cl )
	{
		base.ClientJoined( cl );

		Log.Info( $"Player joined: {cl.SteamId}\\{cl.Name}");

		var pawn = new Idahoid(cl);
		cl.Pawn = pawn;

		var allSpawnPoints = Entity.All.OfType<SpawnPoint>();
		var randomSpawnPoint = allSpawnPoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		pawn.Position = randomSpawnPoint.Position.WithZ(randomSpawnPoint.Position.z + 32f);
		pawn.Respawn();

		if ( !_botsInitialized )
		{
			_botsInitialized = true;
			AddCitizenBot();
		}
	}

	public override void Shutdown()
	{
		// NOP
	}
}
