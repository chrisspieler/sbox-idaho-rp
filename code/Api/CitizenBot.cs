using Sandbox;

namespace IdahoRP.Api;

public partial class CitizenBot : Bot
{
	public Vector3 TargetMoveDir { get; set; }
	public IEntity FollowTarget { get; set; }
	public float FollowDistance { get; set; } = 80f;
	public IEntity LookTarget { get; set; }

	[ConVar.Server( "debug_bot_input" )]
	private static bool _debugBotInput { get; set; } = false;

	public CitizenBot( string name ) : base( name )
	{
		
	}

	public override void Tick()
	{
		var pawn = Client.Pawn as Idahoid;
		if ( FollowTarget != null )
		{
			DoFollow( pawn );
		}
		else
		{
			TargetMoveDir = Vector3.Zero;
		}

		if ( LookTarget != null )
		{
			pawn.LookInput = GetLookDir( pawn, LookTarget.Position );
		}
		else
		{
			pawn.LookInput = Angles.Zero;
		}
	}

	private void DoFollow( Idahoid pawn )
	{
		var dstTarget = Client.Position.Distance( FollowTarget.Position );
		// Try not to overlap the follow target.
		if ( dstTarget < FollowDistance )
		{
			TargetMoveDir = Vector3.Zero;
			return;
		}
		TargetMoveDir = GetMoveDir( pawn, FollowTarget.Position );
	}

	private Vector3 GetMoveDir( Idahoid pawn, Vector3 moveToPos )
	{
		var localMoveToPos = pawn.Transform.PointToLocal( moveToPos );
		Vector3 dirToTarget = localMoveToPos;
		dirToTarget = dirToTarget.Normal.WithZ( 0 );
		if ( _debugBotInput )
			DebugOverlay.DrawVector( Client.Position, Client.Position + pawn.Transform.NormalToWorld( dirToTarget ) * 20f, Color.Green );
		return dirToTarget;
	}

	private Angles GetLookDir( Idahoid pawn, Vector3 lookAtPos )
	{
		Vector3 dirToTarget = lookAtPos - Client.Position;
		dirToTarget = dirToTarget.Normal;
		var rot = Rotation.LookAt( dirToTarget ).Angles();
		if ( _debugBotInput )
		{
			Vector3 eyePosition = Client.Position + Vector3.Zero.WithZ( pawn.Controller.CurrentEyeHeight );
			DebugOverlay.Line(
				start: eyePosition,
				end: eyePosition + dirToTarget * FollowDistance,
				color: Color.Blue );
		}
		return rot;
	}

	public override void BuildInput()
	{
		var pawn = Client.Pawn as Idahoid;
		Input.AnalogMove = TargetMoveDir;
		// Making an aimbot-style simulation of Input.AnalogLook would be excessively annoying
		// since we can just access the LookInput of the pawn directly elsewhere.
		pawn.BuildInput();
	}
}
