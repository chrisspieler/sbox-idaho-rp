using IdahoRP.Api;
using IdahoRP.Bots;
using Sandbox;

namespace IdahoRP;

public partial class CitizenBot : Bot
{
	/// <summary>
	/// The direction of simulated movement inputs.
	/// </summary>
	public Vector3 InputDirection { get; private set; }
	/// <summary>
	/// The worldspace position towards which this bot shall attempt to move. If set to
	/// <c>Vector3.Zero</c>, the bot shall stay in place.
	/// </summary>
	public Vector3? MoveTargetPosition { get; set; } = null;
	/// <summary>
	/// The position to which this bot's gaze shall point.
	/// </summary>
	public Vector3 LookTargetPosition { get; set; }
	/// <summary>
	/// The immediate behavior of this bot.
	/// </summary>
	public IBotAction CurrentAction { get; set; }

	[ConVar.Server( "debug_bot_input" )]
	private static bool _debugBotInput { get; set; } = false;

	public CitizenBot( string name ) : base( name ) { }

	public override void Tick()
	{
		if ( CurrentAction?.IsCompleted == true )
		{
			Log.Info( $"Navigation completed. Deleting bot: {Client.Name}" );
			BotManager.DeleteBot( Client.GetBotId() );
			return;
		}

		CurrentAction?.Tick( this );

		InputDirection = GetInputDir( Client.Pawn.Transform );
		((Idahoid)Client.Pawn).LookInput = GetLookDir( LookTargetPosition );
	}

	private Vector3 GetInputDir( Transform transform )
	{
		if ( MoveTargetPosition != null )
		{
			return PosToDir( transform, MoveTargetPosition.Value );
		}
		else
		{
			return Vector3.Zero;
		}

	}

	private Angles GetLookDir( Vector3 lookAtPos )
	{
		if ( lookAtPos == Vector3.Zero )
			return Angles.Zero;
		Vector3 dirToTarget = lookAtPos - Client.Position;
		dirToTarget = dirToTarget.Normal;
		var rot = Rotation.LookAt( dirToTarget ).Angles();
		if ( _debugBotInput )
		{
			Vector3 eyePosition = Client.Position + Vector3.Zero.WithZ( 64.0f );
			DebugOverlay.Line(
				start: eyePosition,
				end: eyePosition + dirToTarget * 80.0f,
				color: Color.Blue );
		}
		return rot;
	}

	private Vector3 PosToDir( Transform transform, Vector3 moveToPos )
	{
		var localMoveToPos = transform.PointToLocal( moveToPos );
		Vector3 dirToTarget = localMoveToPos;
		dirToTarget = dirToTarget.Normal.WithZ( 0 );
		if ( _debugBotInput )
		{
			Vector3 startPos = transform.Position;
			Vector3 endPos = startPos + transform.NormalToWorld( dirToTarget ) * 20f;
			DebugOverlay.DrawVector( startPos, endPos, Color.Green );
		}
		return dirToTarget;
	}

	public override void BuildInput()
	{
		Input.AnalogMove = InputDirection;
		((Entity)Client.Pawn).BuildInput();
	}
}
