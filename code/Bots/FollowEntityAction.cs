using IdahoRP;
using IdahoRP.Bots;
using Sandbox;

namespace IdahoRP;

public class FollowEntityAction : IBotAction
{
	public bool IsCompleted { get; private set; } = false;
	public IEntity TargetEntity { get; }
	public bool MakeBeeline { get; }

	public void Tick( CitizenBot bot )
	{

	}
}
