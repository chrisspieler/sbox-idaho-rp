using Sandbox;

namespace IdahoRP;

public partial class Idahoid : AnimatedEntity
{
	[ClientInput] public Vector3 MoveInput { get; set; }
	[ClientInput] public Angles LookInput { get; set; }

	/// <summary>
	/// Position a player should be looking from in world space.
	/// </summary>
	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	/// <summary>
	/// Position a player should be looking from in local to the entity coordinates.
	/// </summary>
	[Net, Predicted]
	public Vector3 EyeLocalPosition { get; set; }

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
	/// </summary>
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted]
	public Rotation EyeLocalRotation { get; set; }

	/// <summary>
	/// Override the aim ray to use the player's eye position and rotation.
	/// </summary>
	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );

	public override void BuildInput()
	{
		MoveInput = Input.AnalogMove;

		LookInput = CalculateLookInput( Input.AnalogLook );
	}

	internal Angles CalculateLookInput(Angles input )
	{
		var lookInput = (LookInput + input).Normal;
		return lookInput.WithPitch( lookInput.pitch.Clamp( -89f, 89f ) );
	}
}
