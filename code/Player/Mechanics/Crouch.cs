using Sandbox;

namespace IdahoRP.Mechanics;

/// <summary>
/// The basic crouch mechanic for players.
/// </summary>
public partial class CrouchMechanic : PlayerControllerMechanic
{
	public float CrouchWalkSpeed { get; set; } = 70f;

	public override int SortOrder => 9;
	public override float? WishSpeed => CrouchWalkSpeed;
	public override float? EyeHeight => 40f;

	protected override bool ShouldStart()
	{
		if ( !Input.Down( InputButton.Duck ) ) return false;
		if ( !Controller.GroundEntity.IsValid() ) return false;
		if ( Controller.IsMechanicActive<SprintMechanic>() ) return false;

		return true;
	}
}
