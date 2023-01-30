using Sandbox;

namespace IdahoRP;

public partial class Idahoid
{
	public bool IsAlive => LifeState == LifeState.Alive;
	public bool IsDead => !IsAlive;
}
