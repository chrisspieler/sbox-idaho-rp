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
	private Vector3[] _path;
	private int _currentPathIdx = 0;
	private bool HasReachedEndOfPath => _currentPathIdx >= _path.Length - 1;

	public GoToAction( CitizenBot bot, Vector3 targetPosition, float arriveDistance = 40.0f)
	{
		TargetPosition = targetPosition;
		ArriveDistance = arriveDistance;

		Vector3 startPos = bot.Client.Pawn.Position;

		_path = NavMesh.BuildPath( startPos, targetPosition );
		if (_path == null )
		{
			Log.Info( $"No path could be found between {bot.Client.Position} and {targetPosition}" );
			IsCompleted = true;
		}
		else
		{
			Log.Info($"Built nav path with {_path.Length} elements. Start position: {startPos}, End position: {targetPosition}");
		}
	}

	public void Tick( CitizenBot bot )
	{
		if ( _path == null || HasReachedEndOfPath || IsCompleted )
			return;
		Vector3 currentPos = bot.Client.Pawn.Position;
		Vector3 nextPos = _path[_currentPathIdx];
		bot.MoveTargetPosition = nextPos;
		// By default, bots will look forward and slightly down as they walk.
		bot.LookTargetPosition = nextPos.WithZ( 0f );
		float distanceToNextPos = currentPos.Distance( nextPos );
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
			for ( int i = _currentPathIdx; i < _path.Length; i++ )
			{
				DebugOverlay.Line( dbgLineStartPos, _path[i], Color.Green );
				dbgLineStartPos = _path[i];
			}
		}
	}
}
