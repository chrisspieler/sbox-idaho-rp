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
	/// Sprint has a higher priority than other mechanics.
	/// </summary>
	public override int SortOrder => 10;
	public override float? WishSpeed => SprintSpeed;

	protected override bool ShouldStart()
	{
		if ( !Input.Down( InputButton.Run ) ) return false;
		if ( Player.MoveInput.Length == 0 ) return false;

		return true;
	}
}
