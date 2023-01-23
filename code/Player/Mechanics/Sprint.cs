using Sandbox;
using System;

namespace IdahoRP.Mechanics;

/// <summary>
/// The basic sprinting mechanic for players.
/// It shouldn't, though.
/// </summary>
public partial class SprintMechanic : PlayerControllerMechanic
{
	public float SprintSpeed { get; set; } = 160f;
	/// <summary>
	/// Used in the <c>MaxSprintableAngle</c> calculation. Determines what portion of the player's 
	/// <c>MaxClimbAngle</c> is also sprintable.
	/// </summary>
	public float MaxSprintableAngleFactor { get; set; } = 0.5f;
	public float MaxSprintableAngle => Player.Stats.GetStat( PlayerStat.MaxClimbAngle ) * MaxSprintableAngle;
	/// <summary>
	/// Sprint has a higher priority than other mechanics.
	/// </summary>
	public override int SortOrder => 10;
	public override float? WishSpeed => SprintSpeed;

	protected override bool ShouldStart()
	{
		if ( !Input.Down( InputButton.Run ) ) return false;
		if ( Player.MoveInput.Length == 0 ) return false;
		if ( Player.Controller.CurrentGroundAngle > MaxSprintableAngle ) return false;
		return true;
	}
}
