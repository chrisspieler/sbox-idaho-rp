using Sandbox;

namespace IdahoRP;

[GameResource("Player Stat Modifier", "statmod", "Defines a modifier applied to player stats by items, genetics, or some other source.")]
public partial class PlayerStatModifier : GameResource
{
	/// <summary>
	/// A brief, player-facing name for the stat modifier.
	/// </summary>
	public string Name { get; set; }
	/// <summary>
	/// A player-facing description of the stat modifier.
	/// </summary>
	public string Description { get; set; }
	/// <summary>
	/// The stat that shall be offset by <c>Value</c>.
	/// </summary>
	public PlayerStat TargetStat { get; set; }
	/// <summary>
	/// The offset that shall be applied to the player stat targeted by <c>TargetStat</c>.
	/// </summary>
	public float Value { get; set; }
}
