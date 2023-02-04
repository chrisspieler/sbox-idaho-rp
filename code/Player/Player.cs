using IdahoRP.Mechanics;
using IdahoRP.UI;
using Sandbox;
using System;
using System.Linq;

namespace IdahoRP;

[Title( "Idahomie" ), Icon( "emoji_people" )]
/// <summary>
/// A pawn representing a player in IdahoRP.
/// </summary>
public partial class Idahoid : AnimatedEntity
{
	[BindComponent] public PlayerController Controller { get; }

	public Idahoid()
	{
		InitializeStats();
	}

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
		Log.Info( "Hello, from Spawn!");
	}

	public void Respawn()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, 16, 0 ), new Vector3( 16, 16, 72 ) );

		Health = 100;
		LifeState = LifeState.Alive;
		EnableAllCollisions = true;

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
	}

	[ConCmd.Admin( "sethealth" )]
	public static void SetHealth( float value ) => ConsoleSetStat( PlayerStat.Health, value );

	[ConCmd.Admin( "setmagic" )]
	public static void SetMagic( float value ) => ConsoleSetStat( PlayerStat.Magic, value );

	[ConCmd.Admin("setstat")]
	public static void SetStat(string statName, float value )
	{
		if (Enum.TryParse(typeof(PlayerStat), statName, true, out object stat))
		{
			ConsoleSetStat( (PlayerStat)stat, value );
		}
		else
		{
			Log.Error( $"Invalid stat name: \"{statName}\"" );
		}
	}

	private static void ConsoleSetStat(PlayerStat stat, float value)
	{
		var player = ConsoleSystem.Caller.Pawn as Idahoid;
		if ( player == null )
			return;
		player.SetStat( stat, value );
	}

	public override void Simulate( IClient cl )
	{
		Controller?.Simulate( cl );

		TickRegen();
		TickStatChanges();
		ExecuteMagicTest();
	}

	private TimeUntil _canLeftClick;
	private TimeUntil _canRightClick;

	private void ExecuteMagicTest()
	{
		if ( Game.IsServer )
		{
			if ( Input.Pressed( InputButton.PrimaryAttack ) && _canLeftClick && TrySpendMagic(5.0f) )
			{
				LaunchModel( "models/sbox_props/watermelon/watermelon.vmdl_c" );
				_canLeftClick = 1.0f;
			}
			if ( Input.Pressed( InputButton.SecondaryAttack ) && _canRightClick && TrySpendMagic(20.0f) )
			{
				LaunchModel( "models/citizen/citizen.vmdl" );
				_canRightClick = 3.0f;
			}
			
		}

		bool TrySpendMagic(float amount )
		{
			var currentMagic = GetStat( PlayerStat.Magic );
			if ( currentMagic < amount )
				return false;
			OffsetStat( PlayerStat.Magic, -amount );
			return true;
		}

		void LaunchModel(string modelPath )
		{
			var model = new ModelEntity();
			model.SetModel( modelPath );
			model.Position = EyePosition + EyeRotation.Forward * 40;
			model.Rotation = Rotation.LookAt( Vector3.Random.Normal );
			model.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			model.PhysicsGroup.Velocity = EyeRotation.Forward * 1000;
		}
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
