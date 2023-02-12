using IdahoRP.Bots;
using Sandbox;
using static Sandbox.Event;

namespace IdahoRP;

public class GoToAction : IBotAction
{
	public bool IsCompleted { get; private set; } = false;

	public Vector3 TargetPosition { get; }
	public float ArriveDistance { get; }
	[ConVar.Server( "debug_nav" )]
	private static bool _debugNav { get; set; } = false;
	private NavPath _path;
	private int _currentPathIdx = 0;
	private bool HasReachedEndOfPath => _currentPathIdx >= _path.Count - 1;

	public GoToAction( CitizenBot bot, Vector3 targetPosition, float arriveDistance = 40.0f)
	{
		TargetPosition = targetPosition;
		ArriveDistance = arriveDistance;

		var pawn = (Idahoid)bot.Client.Pawn;
		Vector3 startPos = pawn.Position;


		_path = NavMesh.PathBuilder( startPos )
			.WithStepHeight( 18.0f )
			.Build( targetPosition );
		if (_path == null )
		{
			Log.Info( $"No path could be found between {bot.Client.Position} and {targetPosition}" );
			IsCompleted = true;
		}
		else
		{
			Log.Trace($"Built nav path with {_path.Count} elements. Start position: {startPos}, End position: {targetPosition}");
		}
	}

	public void Tick( CitizenBot bot )
	{
		if ( _path == null || HasReachedEndOfPath || IsCompleted )
			return;
		Vector3 currentPos = bot.Client.Pawn.Position;
		NavPathSegment currentSegment = _path.Segments[_currentPathIdx];
		bot.MoveTargetPosition = currentSegment.GetEndPosition();
		// By default, bots will look forward and slightly down as they walk.
		bot.LookTargetPosition = bot.MoveTargetPosition.Value.WithZ( 0f );
		float distanceToNextPos = currentPos.Distance( bot.MoveTargetPosition.Value );
		if ( distanceToNextPos <= ArriveDistance )
		{
			_currentPathIdx++;
			if ( HasReachedEndOfPath )
			{
				IsCompleted = true;
				return;
			}
		}
		if ( _debugNav && !HasReachedEndOfPath )
		{
			Vector3 dbgLineStartPos = currentPos;
			for ( int i = _currentPathIdx; i < _path.Count; i++ )
			{
				DebugOverlay.Line( dbgLineStartPos, _path.Segments[i].GetEndPosition(), Color.Green );
				dbgLineStartPos = _path.Segments[i].GetEndPosition();
			}
		}
	}
}
