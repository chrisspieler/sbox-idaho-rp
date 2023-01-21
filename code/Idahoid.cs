using Sandbox;
using System;
using System.Linq;

namespace IdahoRP;

/// <summary>
/// A pawn representing a player in IdahoRP.
/// </summary>
partial class Idahoid : AnimatedEntity
{
	public float Speed = 200f;
	[ClientInput] public Vector3 InputDirection { get; set; }
	[ClientInput] public Angles ViewAngles { get; set; }
	public Vector3 EyePosition => Transform.PointToWorld(EyeLocalPosition);
	public Vector3 EyeLocalPosition => Vector3.Up * (EyeHeight * Scale);
	public float EyeHeight = 64.0f;
	public float LookMaxPitchAngle = 89.0f;
	[Net] public float BodyHeight { get; set; } = 72f;
	[Net] public float BodyGirth { get; set; } = 24f;
	[Net] public float StepSize { get; set; } = 18.0f;


	public override void Spawn()
	{
		base.Spawn();

		SetModel("models/citizen/citizen.vmdl");

		EnableHideInFirstPerson = true;
	}

	/// <summary>
	/// This is temporary, get the hull size for the player's collision
	/// </summary>
	public BBox GetHull()
	{
		var girth = BodyGirth * 0.5f;
		var mins = new Vector3( -girth, -girth, 0 );
		var maxs = new Vector3( +girth, +girth, BodyHeight );

		return new BBox( mins, maxs );
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Rotation = Rotation.Angles().WithYaw( ViewAngles.yaw ).ToRotation();
		var moveDirection = InputDirection.Normal;

		Velocity = moveDirection * Speed * ViewAngles.WithPitch(0).ToRotation();
		var mover = new MoveHelper(Position, Velocity);
		mover.Trace.Size(GetHull()).Ignore(this);
		mover.TryMoveWithStep(Time.Delta, StepSize);

		Position = mover.Position;
		Velocity = mover.Velocity;

		Position += Velocity * Time.Delta;

		var animationHelper = new CitizenAnimationHelper( this );
		animationHelper.WithVelocity( Velocity );
	}

	public override void FrameSimulate( IClient cl )
	{
		Camera.Rotation = ViewAngles.ToRotation();
		Camera.Position = EyePosition;
		Camera.FirstPersonViewer = this;
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
	}

	public override void BuildInput()
	{
		base.BuildInput();

		InputDirection = Input.AnalogMove;

		var deltaLook = Input.AnalogLook;

		var viewAngles = ViewAngles;
		viewAngles += deltaLook;
		viewAngles.pitch = viewAngles.pitch.Clamp(-LookMaxPitchAngle, LookMaxPitchAngle);
		// Just in case someone twists their mouse.
		viewAngles.roll = 0f;
		ViewAngles = viewAngles.Normal;
	}
}
