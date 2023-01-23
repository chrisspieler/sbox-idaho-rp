using IdahoRP.Mechanics;
using Sandbox;
using System;
using System.Linq;

namespace IdahoRP;

/// <summary>
/// A pawn representing a player in IdahoRP.
/// </summary>
public partial class Idahoid : AnimatedEntity
{
	[BindComponent] public PlayerController Controller { get; }

	[BindComponent] public IdahoidStats Stats { get; }

	public override void Spawn()
	{
		Predictable = true;

		SetModel("models/citizen/citizen.vmdl");

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableLagCompensation = true;
		EnableHitboxes = true;

		Tags.Add( "player" );

		CreateComponents();
	}

	public void Respawn()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, 16, 0 ), new Vector3( 16, 16, 72 ) );

		Health = 100;
		LifeState = LifeState.Alive;
		EnableAllCollisions = true;
		EnableDrawing = false;

		Children.OfType<ModelEntity>()
			.ToList()
			.ForEach( x => x.EnableDrawing = true );

		ResetInterpolation();
	}

	private void CreateComponents()
	{
		Components.Create<PlayerController>();

		Components.RemoveAny<PlayerControllerMechanic>();

		Components.Create<WalkMechanic>();
		Components.Create<JumpMechanic>();
		Components.Create<AirMoveMechanic>();
		Components.Create<SprintMechanic>();
		Components.Create<CrouchMechanic>();
		Components.Create<InteractionMechanic>();

		// Initializing stats depends on the mechanic components already existing.
		Components.Create<IdahoidStats>();
	}

	public override void Simulate( IClient cl )
	{
		Controller?.Simulate( cl );
	}

	public override void FrameSimulate( IClient cl )
	{
		Rotation = LookInput.WithPitch( 0f ).ToRotation();

		Controller?.FrameSimulate( cl );

		Camera.Rotation = EyeRotation;
		Camera.Position = EyePosition;
		Camera.FirstPersonViewer = this;
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);
	}
}
