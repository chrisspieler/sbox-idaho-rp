using IdahoRP.Api;
using IdahoRP.Bots;
using IdahoRP.Entities;
using IdahoRP.Entities.Hammer;
using IdahoRP.Mechanics;
using IdahoRP.Player.Components;
using IdahoRP.UI;
using Sandbox;
using System;
using System.Linq;
using System.Reflection;

namespace IdahoRP;

[Title( "Idahomie" ), Icon( "emoji_people" )]
/// <summary>
/// A pawn representing a player in IdahoRP.
/// </summary>
public partial class Idahoid : AnimatedEntity
{
	[BindComponent] public PlayerController Controller { get; }

	[Net] public CitizenData Data { get; set; }
	[Net] public MapRegion CurrentRegion { get; set; } = null;

	public Idahoid()
	{
		InitializeStats();
	}

	public Idahoid(IClient cl) : this()
	{
		Data = CitizenData.GetData( cl.SteamId );
		if ( cl.IsBot )
		{
			Log.Trace( $"Configuring pawn for citizen bot named: {Data.Name}" );
		}
		else
		{
			string avatarData = cl.GetClientData( "avatar" );
			Log.Trace( $"{cl} - Loaded avatar data: {avatarData}" );
			Data.DefaultOutfit.Deserialize( avatarData );
		}
		CurrentJob = Data.CurrentJob;
		CreateInfoPanel();
		Data.DefaultOutfit.DressEntity( this );
		var spPlayer = Data.Gender.SubjectPronoun.ToCapitalized();
		var svSmell = Data.Gender.GetSubjectVerb( "smells", "smell" );
		Log.Info( $"Say hello to {Data.Name}! {spPlayer} {svSmell} nice!" );
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

	[ClientRpc]
	public void CreateInfoPanel()
	{
		Log.Trace( $"Creating info panel for {Data.Name}" );
		var worldInfoPanel = new WorldPlayerInfo()
		{
			Player = this
		};
		float panelZOffset = Controller.CurrentEyeHeight;
		WorldPanelTracker.AddWorldPanel( worldInfoPanel, this, Vector3.Up * panelZOffset );
	}

	public void Respawn(Vector3? position = null)
	{
		Position = position == null ? GetRandomSpawnPoint() : position.Value;

		SetupPhysicsFromOrientedCapsule(
			PhysicsMotionType.Keyframed,
			Capsule.FromHeightAndRadius( 72, Controller.BodyGirth / 2 )
		);

		Health = 100;
		LifeState = LifeState.Alive;
		EnableAllCollisions = true;

		Children.OfType<ModelEntity>()
			.ToList()
			.ForEach( x => x.EnableDrawing = true );

		ResetInterpolation();

		Data.DefaultOutfit.DressEntity( this );

		Vector3 GetRandomSpawnPoint()
		{
			var allSpawnPoints = Entity.All.OfType<SpawnPoint>();
			var randomSpawnPoint = allSpawnPoints.Random();
			return randomSpawnPoint.Position.WithZ( randomSpawnPoint.Position.z + 32f );
		}
	}

	private void CreateComponents()
	{
		_flashlightComponent = Components.Create<Flashlight>();

		Components.Create<PlayerController>();

		Components.RemoveAny<PlayerControllerMechanic>();

		Components.Create<WalkMechanic>();
		Components.Create<JumpMechanic>();
		Components.Create<AirMoveMechanic>();
		Components.Create<SprintMechanic>();
		Components.Create<CrouchMechanic>();
		Components.Create<InteractionMechanic>();
	}

	[ClockEvent.MinuteElapsed]
	public void MinuteTick(int hour, int minute)
	{
		if (!Client.IsBot)
			UpdateTime( hour, minute );
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

	private Flashlight _flashlightComponent;

	public override void Simulate( IClient cl )
	{
		Controller?.Simulate( cl );
		_flashlightComponent?.Simulate();

		TickRegen();
		TickStatChanges();
		SimulateMagic();
		SimulateAnimation();
		SimulateHover();
	}

	private TimeUntil _canLeftClick;
	private TimeUntil _canRightClick;

	const string MODEL_CITIZEN = "models/citizen/citizen.vmdl";

	private void SimulateMagic()
	{
		if ( Game.IsServer )
		{
			if ( Input.Pressed( InputButton.PrimaryAttack ) && _canLeftClick && TrySpendStat( PlayerStat.Magic, 25.0f ) )
			{
				// Melon value should increase gradually over time.
				float melonValue = 1f * (Time.Now / 1000);
				// Each melon is worth at least five cents.
				melonValue = MathF.Max( 0.05f, melonValue );
				LaunchMoneyMelon( melonValue );
				_canLeftClick = 1.0f;
			}
			if ( Input.Pressed( InputButton.SecondaryAttack ) && _canRightClick && TrySpendStat( PlayerStat.Magic, 50.0f ) )
			{
				Reproduce();
				_canRightClick = 1.0f;
			}
		}

		void LaunchModel(string modelPath )
		{
			var model = new ModelEntity();
			model.SetModel( modelPath );
			model.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			LaunchEntity( model );
		}

		void LaunchMoneyMelon(float melonValue )
		{
			var melon = new MoneyMelon();
			melon.BaseValue = melonValue;
			LaunchEntity( melon );
		}

		void LaunchEntity(ModelEntity entity)
		{
			entity.Position = EyePosition + EyeRotation.Forward * 40;
			entity.Rotation = Rotation.LookAt( Vector3.Random.Normal );
			entity.PhysicsGroup.Velocity = EyeRotation.Forward * 1000;
		}

		void Reproduce()
		{
			var newBot = BotManager.AddCitizenBot();
			var pawn = newBot.Client.Pawn;
			var tryDistance = 200.0f;
			var tryPosition = EyePosition + EyeRotation.Forward * tryDistance;
			TraceResult tr = Trace.Ray( EyePosition, tryPosition )
				.Ignore(this)
				.Run();
			var finalDistance = tryDistance;
			if ( tr.Hit )
			{
				finalDistance -= tr.Distance - Controller.BodyGirth;
			}

			pawn.Position = EyePosition + EyeRotation.Forward * finalDistance;
			pawn.Rotation = Rotation.FromYaw( Rotation.Yaw() + 90.0f );

			newBot.CurrentAction = new GoToAction( newBot, Vector3.Zero );
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

	protected override void OnDestroy()
	{
		WorldPanelTracker.DestroyWorldPanels( this );
	}
}
