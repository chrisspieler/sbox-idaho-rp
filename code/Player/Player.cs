using IdahoRP.Api;
using IdahoRP.Mechanics;
using IdahoRP.UI;
using Sandbox;
using Sandbox.UI;
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

	[Net] public string ClientAvatarData { get; private set; }

	public Idahoid()
	{
		InitializeStats();
	}

	public Idahoid(IClient cl) : this()
	{
		ClientAvatarData = cl.GetClientData( "avatar" );
		Log.Info( $"{cl} - Loaded avatar data: {ClientAvatarData}" );
		if ( !Game.IsClient )
		{
			JobManager.SetJob( "job_neet", this );
		}
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
	}

	public override void ClientSpawn()
	{
		var worldInfoPanel = new WorldPlayerInfo()
		{
			Player = this
		};
		float panelZOffset = Controller.CurrentEyeHeight;
		WorldPanelTracker.AddWorldPanel( worldInfoPanel, this, Vector3.Up * panelZOffset );
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

		Clothing.DressEntity( this );
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
		SimulateAnimation();
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

	void SimulateAnimation()
	{

		// where should we be rotated to
		var turnSpeed = 0.02f;

		Rotation rotation;

		rotation = LookInput.ToRotation();

		var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
		Rotation = Rotation.Slerp( Rotation, idealRotation, Controller.GetWishVelocity().Length * Time.Delta * turnSpeed );
		Rotation = Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction

		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( this );

		animHelper.WithWishVelocity( Controller.GetWishVelocity() );
		animHelper.WithVelocity( Controller.GetWishVelocity() );
		animHelper.WithLookAt( EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, Controller.IsMechanicActive<CrouchMechanic>() ? 1 : 0, Time.Delta * 10.0f );
		animHelper.VoiceLevel = (Game.IsClient && Client.IsValid()) ? Client.Voice.LastHeard < 0.5f ? Client.Voice.CurrentLevel : 0.0f : 0.0f;
		animHelper.IsGrounded = Controller.GroundEntity != null;
		animHelper.IsSwimming = this.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;

		if ( Controller.IsMechanicActive<JumpMechanic>() ) animHelper.TriggerJump();
	}
}
