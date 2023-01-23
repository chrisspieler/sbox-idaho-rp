using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdahoRP;

[Title( "Idahomie" ), Icon( "emoji_people" )]
public partial class IdahoRP : BaseGameManager
{
	[ClientInput] public Vector3 InputDirection { get; protected set; }

	public override void ClientJoined( IClient cl )
	{
		base.ClientJoined( cl );

		Log.Info( $"Player joined: {cl.SteamId}\\{cl.Name}");

		var pawn = new Idahoid();
		cl.Pawn = pawn;

		var allSpawnPoints = Entity.All.OfType<SpawnPoint>();
		var randomSpawnPoint = allSpawnPoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		pawn.Position = randomSpawnPoint.Position.WithZ(randomSpawnPoint.Position.z + 32f);
		pawn.Respawn();
	}

	public override void Shutdown()
	{
		// NOP
	}
}
